import 'dart:async';
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:geolocator/geolocator.dart';
import 'package:intl/intl.dart';
import 'package:permission_handler/permission_handler.dart'
    as permission_handler;
import '../../../../core/theme/app_theme.dart';
import '../../../../core/constants/enums.dart';
import '../../../../core/utils/geo.dart';
import '../../../../core/providers/analytics_providers.dart';
import '../../../../core/providers/location_provider.dart';
import '../../../../core/services/location_service.dart';
import '../../../profile/presentation/providers/cached_worker_profile_provider.dart';
import '../../domain/entities/job_details.dart';
import '../../domain/entities/punch_status.dart';
import '../../domain/usecases/get_punch_status.dart';
import '../../domain/usecases/submit_timesheet.dart';
import '../providers/timesheet_providers.dart';
import 'job_header_card.dart';

enum _PrecisionAction { cancel, continueAnyway, retry }

class PunchCardTab extends ConsumerStatefulWidget {
  final JobDetails jobDetails;
  final Future<void> Function()? onRefreshCheckpoint;

  const PunchCardTab({
    super.key,
    required this.jobDetails,
    this.onRefreshCheckpoint,
  });

  String get jobId => jobDetails.id;
  int get jobNumberId => jobDetails.numberId;
  double? get checkpointLatitude => jobDetails.checkpointLatitude;
  double? get checkpointLongitude => jobDetails.checkpointLongitude;
  double? get checkpointRadiusMeters => jobDetails.checkpointRadiusMeters;

  bool get hasCheckpoint =>
      checkpointLatitude != null &&
      checkpointLongitude != null &&
      checkpointRadiusMeters != null;

  @override
  ConsumerState<PunchCardTab> createState() => _PunchCardTabState();
}

class _PunchCardTabState extends ConsumerState<PunchCardTab> {
  static const double _preciseAccuracyMeters = 20;
  static const double _maxUsableAccuracyMeters = 50;
  static const Duration _refreshFixAfter = Duration(seconds: 10);
  static const Duration _maxSubmitFixAge = Duration(seconds: 60);
  static const String _locationError =
      'Could not get your location. Tap to retry.';

  DateTime _currentTime = DateTime.now();
  Timer? _timer;
  ClockType _clockType = ClockType.none;
  bool _isLoadingClockType = true;
  String? _clockTypeError;
  bool _isSubmitting = false;
  int _clockTypeRequestId = 0;
  StreamSubscription<Position>? _positionSubscription;
  Position? _livePosition;
  String? _punchRejectedMessage;
  String? _punchRejectedReference;
  PunchStatus _punchStatus = PunchStatus.none;

  @override
  void initState() {
    super.initState();
    _timer = Timer.periodic(const Duration(seconds: 1), (timer) {
      if (mounted) {
        setState(() {
          _currentTime = DateTime.now();
        });
      }
    });
    _loadClockType();
    _syncPositionWatch();
  }

  @override
  void didUpdateWidget(covariant PunchCardTab oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.hasCheckpoint != widget.hasCheckpoint) {
      _syncPositionWatch();
    }
  }

  void _syncPositionWatch() {
    if (widget.hasCheckpoint) {
      _positionSubscription ??= ref
          .read(locationServiceProvider)
          .watchPosition()
          .listen(_onLivePosition);
    } else {
      _positionSubscription?.cancel();
      _positionSubscription = null;
    }
  }

  void _onLivePosition(Position position) {
    if (!mounted) return;
    setState(() {
      _livePosition = position;
    });
  }

  Future<void> _loadClockType({bool refreshCheckpoint = false}) async {
    final requestId = ++_clockTypeRequestId;
    setState(() {
      _isLoadingClockType = true;
      _clockTypeError = null;
      _punchRejectedMessage = null;
      _punchRejectedReference = null;
    });
    if (refreshCheckpoint) {
      await widget.onRefreshCheckpoint?.call();
      if (!mounted || requestId != _clockTypeRequestId) return;
    }
    final useCase = ref.read(getPunchStatusUseCaseProvider);
    final position = await ref
        .read(locationServiceProvider)
        .getCurrentPosition();

    if (!mounted || requestId != _clockTypeRequestId) return;

    if (position == null) {
      setState(() {
        _clockType = ClockType.none;
        _isLoadingClockType = false;
        _clockTypeError = _locationError;
      });
      _showErrorSnackBar(
        'Failed to get location. Please enable location services.',
        action: SnackBarAction(
          label: 'Retry',
          textColor: Colors.white,
          onPressed: _loadClockType,
        ),
      );
      return;
    }

    _livePosition ??= position;

    final result = await useCase(
      GetPunchStatusParams(
        requestId: widget.jobId,
        latitude: position.latitude,
        longitude: position.longitude,
      ),
    );

    if (!mounted || requestId != _clockTypeRequestId) return;

    setState(() {
      _isLoadingClockType = false;
      result.fold(
        (failure) {
          _clockType = ClockType.none;
          _clockTypeError = 'Could not check your shift status. Tap to retry.';
        },
        (status) {
          _punchStatus = status;
          _clockType = status.clockType;
          _clockTypeError = null;
        },
      );
    });
  }

  @override
  void dispose() {
    _timer?.cancel();
    _positionSubscription?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      physics: const ClampingScrollPhysics(),
      padding: const EdgeInsets.only(top: 12, bottom: 24),
      child: _buildClockCard(),
    );
  }

  Widget _buildClockCard() {
    return Column(
      children: [
        JobHeaderCard(jobDetails: widget.jobDetails),
        const SizedBox(height: 12),
        Card(
          margin: const EdgeInsets.symmetric(horizontal: 16),
          child: Padding(
            padding: const EdgeInsets.all(20),
            child: Column(
              children: [
                Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(12),
                      decoration: BoxDecoration(
                        gradient: const LinearGradient(
                          colors: [AppTheme.primaryBlue, AppTheme.tertiaryBlue],
                        ),
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: const Icon(
                        Icons.access_time,
                        color: Colors.white,
                        size: 28,
                      ),
                    ),
                    const SizedBox(width: 16),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text(
                            'Current Time',
                            style: TextStyle(
                              fontSize: 14,
                              color: Colors.grey,
                              fontWeight: FontWeight.w500,
                            ),
                          ),
                          const SizedBox(height: 4),
                          Text(
                            DateFormat('hh:mm:ss a').format(_currentTime),
                            style: const TextStyle(
                              fontSize: 28,
                              fontWeight: FontWeight.bold,
                              color: AppTheme.textDark,
                              letterSpacing: 1,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 16),
                const Divider(),
                const SizedBox(height: 12),
                Row(
                  children: [
                    Icon(
                      Icons.calendar_today,
                      size: 18,
                      color: AppTheme.primaryBlue,
                    ),
                    const SizedBox(width: 8),
                    Text(
                      DateFormat('EEEE, MMMM dd, yyyy').format(_currentTime),
                      style: const TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                        color: AppTheme.textDark,
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ),
        if (widget.hasCheckpoint) ...[
          const SizedBox(height: 12),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            child: _buildCheckpointStrip(),
          ),
        ],
        const SizedBox(height: 24),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16),
          child: SizedBox(
            width: double.infinity,
            height: 56,
            child: _buildClockButton(),
          ),
        ),
        if (_clockTypeError != null) ...[
          const SizedBox(height: 8),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            child: Text(
              _clockTypeError!,
              textAlign: TextAlign.center,
              style: TextStyle(fontSize: 12, color: Colors.grey.shade700),
            ),
          ),
        ],
        if (_punchRejectedMessage != null) ...[
          const SizedBox(height: 12),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            child: _buildPunchRejectedCard(),
          ),
        ],
        const SizedBox(height: 16),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16),
          child: _buildTodayStamps(),
        ),
        const SizedBox(height: 8),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 20),
          child: Align(
            alignment: Alignment.centerRight,
            child: Text(
              'This week: ${_formatHours(_punchStatus.weekHours)} h',
              style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildTodayStamps() {
    return Card(
      margin: EdgeInsets.zero,
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 14),
        child: IntrinsicHeight(
          child: Row(
            children: [
              Expanded(child: _buildStamp('CLOCK IN', _punchStatus.clockIn)),
              VerticalDivider(
                width: 1,
                thickness: 1,
                color: Colors.grey.shade300,
              ),
              Expanded(
                child: _buildStamp('CLOCK OUT', _punchStatus.clockOut),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildStamp(String label, DateTime? value) {
    return Column(
      children: [
        Text(
          label,
          style: TextStyle(
            fontSize: 11,
            fontWeight: FontWeight.w600,
            letterSpacing: 0.6,
            color: Colors.grey.shade600,
          ),
        ),
        const SizedBox(height: 2),
        Text(
          value != null ? DateFormat('h:mm a').format(value) : '--:--',
          style: TextStyle(
            fontSize: 20,
            fontWeight: FontWeight.w600,
            color: value != null ? AppTheme.textDark : Colors.grey.shade400,
            fontFeatures: const [FontFeature.tabularFigures()],
          ),
        ),
      ],
    );
  }

  String _formatHours(double hours) {
    if (hours == hours.roundToDouble()) return hours.round().toString();
    return hours.toStringAsFixed(2);
  }

  Widget _buildPunchRejectedCard() {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: AppTheme.errorRed.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppTheme.errorRed.withValues(alpha: 0.3)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Icon(Icons.location_off, size: 20, color: AppTheme.errorRed),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Too far from workplace',
                  style: TextStyle(
                    fontSize: 13,
                    fontWeight: FontWeight.w600,
                    color: AppTheme.errorRed,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  _punchRejectedMessage!,
                  style: const TextStyle(fontSize: 13, color: AppTheme.textDark),
                ),
                if (_punchRejectedReference != null) ...[
                  const SizedBox(height: 4),
                  Text(
                    _punchRejectedReference!,
                    style: TextStyle(fontSize: 11, color: Colors.grey.shade700),
                  ),
                ],
              ],
            ),
          ),
          IconButton(
            padding: EdgeInsets.zero,
            constraints: const BoxConstraints(),
            icon: Icon(Icons.close, size: 18, color: Colors.grey.shade600),
            onPressed: _clearPunchRejected,
          ),
        ],
      ),
    );
  }

  void _clearPunchRejected() {
    if (_punchRejectedMessage == null) return;
    setState(() {
      _punchRejectedMessage = null;
      _punchRejectedReference = null;
    });
  }

  Widget _buildCheckpointStrip() {
    final position = _livePosition;
    final Color color;
    final IconData icon;
    final String title;
    String? subtitle;
    final String? coordinates = position == null
        ? null
        : '${position.latitude.toStringAsFixed(6)}, '
              '${position.longitude.toStringAsFixed(6)} '
              '(±${position.accuracy.round()} m)';

    if (position == null) {
      color = Colors.grey.shade600;
      if (_clockTypeError == _locationError) {
        icon = Icons.location_disabled;
        title = 'Turn on location to see your distance to the check point.';
      } else {
        icon = Icons.location_searching;
        title = 'Checking your distance to the check point...';
      }
    } else {
      final meters = distanceInMeters(
        widget.checkpointLatitude!,
        widget.checkpointLongitude!,
        position.latitude,
        position.longitude,
      );
      final accuracy = position.accuracy;
      final radius = widget.checkpointRadiusMeters!;
      if (meters + accuracy <= radius) {
        color = AppTheme.successGreen;
        icon = Icons.check_circle;
        title = 'At the check point';
      } else if (meters <= radius) {
        color = AppTheme.warningOrange;
        icon = Icons.location_searching;
        title =
            'About ${meters.round()} m from the check point (GPS ±${accuracy.round()} m)';
        subtitle = 'Move a bit closer to be safe.';
      } else {
        final direction = cardinalDirection(
          widget.checkpointLatitude!,
          widget.checkpointLongitude!,
          position.latitude,
          position.longitude,
        );
        color = AppTheme.errorRed;
        icon = Icons.location_off;
        title = 'About ${formatDistance(meters)} $direction of the check point';
        subtitle = 'Get closer before you clock in.';
      }
    }

    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: color.withValues(alpha: 0.3)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, size: 20, color: color),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: TextStyle(
                    fontSize: 13,
                    fontWeight: FontWeight.w600,
                    color: color,
                  ),
                ),
                if (subtitle != null) ...[
                  const SizedBox(height: 2),
                  Text(
                    subtitle,
                    style: TextStyle(fontSize: 12, color: Colors.grey.shade700),
                  ),
                ],
                if (coordinates != null) ...[
                  const SizedBox(height: 4),
                  Row(
                    children: [
                      Icon(
                        Icons.gps_fixed,
                        size: 14,
                        color: Colors.grey.shade600,
                      ),
                      const SizedBox(width: 4),
                      Expanded(
                        child: Text(
                          coordinates,
                          style: TextStyle(
                            fontSize: 12,
                            color: Colors.grey.shade600,
                            fontFeatures: const [FontFeature.tabularFigures()],
                          ),
                        ),
                      ),
                    ],
                  ),
                ],
              ],
            ),
          ),
        ],
      ),
    );
  }

  bool get _isShiftCompleted =>
      _clockType == ClockType.none &&
      _punchStatus.clockIn != null &&
      _punchStatus.clockOut != null;

  Widget _buildClockButton() {
    final isBusy = _isSubmitting || _isLoadingClockType;
    final isRetry = !isBusy && _clockTypeError != null;
    final backgroundColor = isRetry
        ? AppTheme.warningOrange
        : _clockType == ClockType.clockIn
        ? AppTheme.successGreen
        : _clockType == ClockType.clockOut
        ? AppTheme.errorRed
        : Colors.grey;
    final onPressed = isBusy
        ? null
        : isRetry
        ? () => _loadClockType(refreshCheckpoint: true)
        : _clockType == ClockType.none
        ? null
        : _onClockTapped;

    return ElevatedButton(
      onPressed: onPressed,
      style: ElevatedButton.styleFrom(
        backgroundColor: backgroundColor,
        foregroundColor: Colors.white,
        elevation: 4,
        shadowColor: AppTheme.primaryBlue.withValues(alpha: 0.4),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        disabledBackgroundColor: _isLoadingClockType
            ? AppTheme.primaryBlue.withValues(alpha: 0.6)
            : _isShiftCompleted
            ? AppTheme.successGreen.withValues(alpha: 0.55)
            : Colors.grey.shade400,
        disabledForegroundColor: Colors.white,
      ),
      child: isBusy
          ? Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const SizedBox(
                  height: 24,
                  width: 24,
                  child: CircularProgressIndicator(
                    strokeWidth: 2.5,
                    valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                  ),
                ),
                if (_isLoadingClockType && !_isSubmitting) ...[
                  const SizedBox(width: 12),
                  const Text(
                    'CHECKING...',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      letterSpacing: 1,
                    ),
                  ),
                ],
              ],
            )
          : Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(
                  isRetry
                      ? Icons.refresh
                      : _clockType == ClockType.clockIn
                      ? Icons.login
                      : _clockType == ClockType.clockOut
                      ? Icons.logout
                      : _isShiftCompleted
                      ? Icons.check_circle
                      : Icons.block,
                  size: 24,
                ),
                const SizedBox(width: 12),
                Text(
                  isRetry
                      ? 'RETRY'
                      : _clockType == ClockType.clockIn
                      ? 'CLOCK IN'
                      : _clockType == ClockType.clockOut
                      ? 'CLOCK OUT'
                      : _isShiftCompleted
                      ? 'SHIFT COMPLETED'
                      : 'UNAVAILABLE',
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    letterSpacing: 1,
                  ),
                ),
              ],
            ),
    );
  }

  Future<void> _onClockTapped() async {
    setState(() {
      _isSubmitting = true;
      _punchRejectedMessage = null;
      _punchRejectedReference = null;
    });

    try {
      final locationService = ref.read(locationServiceProvider);

      if (!await locationService.ensurePermissions()) {
        if (!mounted) return;
        setState(() {
          _isSubmitting = false;
        });
        _showErrorSnackBar(
          'Failed to get location. Please enable location services.',
          action: SnackBarAction(
            label: 'Settings',
            textColor: Colors.white,
            onPressed: _openLocationSettings,
          ),
        );
        return;
      }

      final precision = await locationService.resolveLocationPrecision();
      if (precision == LocationPrecision.reduced) {
        if (!mounted) return;
        setState(() {
          _isSubmitting = false;
        });
        await _showLocationPrecisionWarning(allowContinue: false);
        return;
      }

      final position = await locationService.getFreshPosition();

      if (position == null) {
        if (!mounted) return;
        setState(() {
          _isSubmitting = false;
        });
        _showErrorSnackBar(
          'Failed to get location. Please enable location services.',
          action: SnackBarAction(
            label: 'Settings',
            textColor: Colors.white,
            onPressed: _openLocationSettings,
          ),
        );
        return;
      }

      if (!mounted) return;
      final locationAccuracy = position.accuracy;
      final isPrecise = locationAccuracy <= _preciseAccuracyMeters;
      if (!isPrecise) {
        setState(() {
          _isSubmitting = false;
        });
        final isUsable = locationAccuracy <= _maxUsableAccuracyMeters;
        if (!isUsable) {
          _logLocationBlocked(
            reason: 'accuracy_too_low',
            accuracy: locationAccuracy,
          );
        }
        final action = await _showLocationPrecisionWarning(
          accuracy: locationAccuracy,
          allowContinue: isUsable,
          allowRetry: !isUsable,
        );
        if (action == _PrecisionAction.retry) {
          await _onClockTapped();
          return;
        }
        if (action != _PrecisionAction.continueAnyway) {
          return;
        }
        setState(() {
          _isSubmitting = true;
        });
      }

      if (!mounted) return;
      setState(() {
        _isSubmitting = false;
      });
      final confirmed = await _showConfirmationDialog(position, isPrecise);
      if (confirmed != true) {
        return;
      }
      setState(() {
        _isSubmitting = true;
      });

      var submitPosition = position;
      if (_fixAge(submitPosition) > _refreshFixAfter) {
        final refreshed = await locationService.getFreshPosition();
        if (refreshed != null &&
            _isBetterForSubmit(refreshed, submitPosition)) {
          submitPosition = refreshed;
        }
      }

      if (!mounted) return;
      setState(() {
        _livePosition = submitPosition;
      });
      if (_fixAge(submitPosition) > _maxSubmitFixAge ||
          submitPosition.accuracy > _maxUsableAccuracyMeters) {
        setState(() {
          _isSubmitting = false;
        });
        _logLocationBlocked(
          reason: _fixAge(submitPosition) > _maxSubmitFixAge
              ? 'fix_expired'
              : 'accuracy_too_low',
          accuracy: submitPosition.accuracy,
          fixAge: _fixAge(submitPosition),
        );
        _showErrorSnackBar(
          'Could not get a current, accurate location. Please try again.',
          action: SnackBarAction(
            label: 'Retry',
            textColor: Colors.white,
            onPressed: _onClockTapped,
          ),
        );
        return;
      }

      final useCase = ref.read(submitTimesheetUseCaseProvider);
      final result = await useCase(
        SubmitTimesheetParams(
          jobId: widget.jobId,
          latitude: submitPosition.latitude,
          longitude: submitPosition.longitude,
        ),
      );

      if (!mounted) return;

      await result.fold<Future<void>>(
        (failure) async {
          setState(() {
            _isSubmitting = false;
          });
          final errorMessage = failure.message.toLowerCase();

          // Check for "too far from checkpoint" error
          final isTooFarFromCheckpoint =
              errorMessage.contains('too far') ||
              errorMessage.contains('check point') ||
              errorMessage.contains('checkpoint') ||
              errorMessage.contains('get closer');

          // Check for 3-minute restriction error
          final is3MinuteRestriction =
              (failure.message.contains('400') ||
                  errorMessage.contains('bad request') ||
                  errorMessage.contains('too soon') ||
                  errorMessage.contains('wait')) &&
              _clockType == ClockType.clockOut;

          if (isTooFarFromCheckpoint) {
            final fixAgeMs = _fixAge(submitPosition).inMilliseconds;
            // Track location error for monitoring
            ref
                .read(analyticsServiceProvider)
                .logEvent(
                  name: 'clock_location_error',
                  parameters: {
                    'job_id': widget.jobId,
                    'error_type': 'too_far_from_checkpoint',
                    'location_accuracy_m': submitPosition.accuracy,
                    'fix_age_ms': fixAgeMs < 0 ? 0 : fixAgeMs,
                    'is_mocked': submitPosition.isMocked,
                  },
                );
            setState(() {
              _punchRejectedMessage = failure.message;
              _punchRejectedReference = _punchReference();
            });
          } else if (is3MinuteRestriction) {
            // Track 3-minute restriction hit for monitoring
            ref
                .read(analyticsServiceProvider)
                .logEvent(
                  name: 'clock_out_restriction_hit',
                  parameters: {
                    'job_id': widget.jobId,
                    'error_type': '3_minute_restriction',
                  },
                );
            _showClockOutRestrictionError();
          } else {
            _showErrorSnackBar(failure.message);
          }
        },
        (response) async {
          final fixAgeMs = _fixAge(submitPosition).inMilliseconds;
          ref
              .read(analyticsServiceProvider)
              .logEvent(
                name: _clockType == ClockType.clockIn
                    ? 'clock_in_success'
                    : 'clock_out_success',
                parameters: {
                  'job_id': widget.jobId,
                  'location_accuracy_m': submitPosition.accuracy,
                  'fix_age_ms': fixAgeMs < 0 ? 0 : fixAgeMs,
                  'is_mocked': submitPosition.isMocked,
                },
              );
          _showSuccessSnackBar(response.workerFullName, response.finish);
          await _loadClockType(refreshCheckpoint: true);
          if (!mounted) return;
          setState(() {
            _isSubmitting = false;
          });
        },
      );
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _isSubmitting = false;
      });
      _showErrorSnackBar('An error occurred: $e');
    }
  }

  Duration _fixAge(Position position) =>
      DateTime.now().difference(position.timestamp);

  bool _isBetterForSubmit(Position candidate, Position current) =>
      candidate.accuracy <= _preciseAccuracyMeters ||
      candidate.accuracy <= current.accuracy ||
      _fixAge(current) > _maxSubmitFixAge;

  void _logLocationBlocked({
    required String reason,
    required double accuracy,
    Duration? fixAge,
  }) {
    ref
        .read(analyticsServiceProvider)
        .logEvent(
          name: 'clock_location_blocked',
          parameters: {
            'job_id': widget.jobId,
            'reason': reason,
            'location_accuracy_m': accuracy,
            if (fixAge != null) 'fix_age_ms': fixAge.inMilliseconds,
          },
        );
  }

  void _showSuccessSnackBar(String workerName, bool finish) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Row(
          children: [
            const Icon(Icons.check_circle, color: Colors.white),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    finish
                        ? 'Clocked Out Successfully!'
                        : 'Clocked In Successfully!',
                    style: const TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 16,
                    ),
                  ),
                  Text(workerName, style: const TextStyle(fontSize: 14)),
                ],
              ),
            ),
          ],
        ),
        backgroundColor: AppTheme.successGreen,
        duration: const Duration(seconds: 3),
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      ),
    );
  }

  void _showErrorSnackBar(String message, {SnackBarAction? action}) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Row(
          children: [
            const Icon(Icons.error_outline, color: Colors.white),
            const SizedBox(width: 12),
            Expanded(
              child: Text(message, style: const TextStyle(fontSize: 14)),
            ),
          ],
        ),
        action: action,
        backgroundColor: AppTheme.errorRed,
        duration: const Duration(seconds: 4),
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      ),
    );
  }

  void _showClockOutRestrictionError() {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Row(
          children: [
            const Icon(Icons.access_time, color: Colors.white),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  const Text(
                    'Too Soon to Clock Out',
                    style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    'You must wait at least 3 minutes after clocking in before you can clock out.',
                    style: const TextStyle(fontSize: 13),
                  ),
                ],
              ),
            ),
          ],
        ),
        backgroundColor: AppTheme.warningOrange,
        duration: const Duration(seconds: 5),
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      ),
    );
  }

  String _punchReference() {
    final profile = ref.read(cachedWorkerProfileProvider).value;
    final worker = profile == null
        ? 'Worker: unknown'
        : 'Worker #${profile.numberId ?? '-'} · ${profile.fullName}';
    final when = DateFormat('MMM dd, yyyy hh:mm a').format(DateTime.now());
    return '$worker\nRequest #${widget.jobNumberId} · $when';
  }

  Future<bool?> _showConfirmationDialog(
    Position position,
    bool isPrecise,
  ) async {
    final clockAction = _clockType == ClockType.clockIn
        ? 'Clock In'
        : 'Clock Out';
    final clockActionLower = clockAction.toLowerCase();

    return showDialog<bool>(
      context: context,
      barrierDismissible: false,
      builder: (context) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: Row(
          children: [
            Icon(
              _clockType == ClockType.clockIn ? Icons.login : Icons.logout,
              color: _clockType == ClockType.clockIn
                  ? AppTheme.successGreen
                  : AppTheme.errorRed,
              size: 28,
            ),
            const SizedBox(width: 12),
            Text('Confirm $clockAction'),
          ],
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Are you sure you want to $clockActionLower?',
              style: const TextStyle(fontSize: 16),
            ),
            const SizedBox(height: 16),
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: Colors.grey.shade100,
                borderRadius: BorderRadius.circular(8),
              ),
              child: Column(
                children: [
                  Row(
                    children: [
                      const Icon(
                        Icons.access_time,
                        size: 18,
                        color: Colors.grey,
                      ),
                      const SizedBox(width: 8),
                      Text(
                        DateFormat('hh:mm:ss a').format(DateTime.now()),
                        style: const TextStyle(
                          fontWeight: FontWeight.bold,
                          fontSize: 14,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      Icon(
                        isPrecise
                            ? Icons.location_on
                            : Icons.location_searching,
                        size: 18,
                        color: isPrecise
                            ? AppTheme.successGreen
                            : AppTheme.warningOrange,
                      ),
                      const SizedBox(width: 8),
                      Expanded(
                        child: Text(
                          isPrecise
                              ? 'Location: Precise (±${position.accuracy.toStringAsFixed(0)}m)'
                              : 'Location: Approximate (±${position.accuracy.toStringAsFixed(0)}m)',
                          style: TextStyle(
                            fontSize: 13,
                            color: isPrecise
                                ? Colors.black87
                                : AppTheme.warningOrange,
                            fontWeight: isPrecise
                                ? FontWeight.normal
                                : FontWeight.bold,
                          ),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text(
              'Cancel',
              style: TextStyle(color: Colors.grey, fontSize: 16),
            ),
          ),
          ElevatedButton(
            onPressed: () => Navigator.of(context).pop(true),
            style: ElevatedButton.styleFrom(
              backgroundColor: _clockType == ClockType.clockIn
                  ? AppTheme.successGreen
                  : AppTheme.errorRed,
              foregroundColor: Colors.white,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(8),
              ),
            ),
            child: Text(
              clockAction,
              style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
            ),
          ),
        ],
      ),
    );
  }

  Future<_PrecisionAction?> _showLocationPrecisionWarning({
    double? accuracy,
    bool allowContinue = true,
    bool allowRetry = false,
  }) async {
    final isBlockedByAccuracy = accuracy != null && !allowContinue;
    return showDialog<_PrecisionAction>(
      context: context,
      barrierDismissible: false,
      builder: (context) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: Row(
          children: [
            const Icon(
              Icons.warning_amber_rounded,
              color: AppTheme.warningOrange,
              size: 28,
            ),
            const SizedBox(width: 12),
            Text(
              accuracy == null
                  ? 'Precise Location Off'
                  : isBlockedByAccuracy
                  ? 'Location Too Imprecise'
                  : 'Location Not Precise',
            ),
          ],
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              accuracy == null
                  ? 'Precise Location is turned off for this app. Clock in/out requires your exact location.'
                  : isBlockedByAccuracy
                  ? 'Your location accuracy is too low for clock in/out. Wait a moment for a better GPS signal and try again.'
                  : 'Your location accuracy is not precise enough for clock in/out.',
              style: const TextStyle(fontSize: 16),
            ),
            if (accuracy != null) ...[
              const SizedBox(height: 12),
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: AppTheme.warningOrange.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(
                    color: AppTheme.warningOrange.withValues(alpha: 0.3),
                  ),
                ),
                child: Row(
                  children: [
                    const Icon(
                      Icons.location_searching,
                      color: AppTheme.warningOrange,
                      size: 20,
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        'Current accuracy: ±${accuracy.toStringAsFixed(0)} meters',
                        style: const TextStyle(
                          fontWeight: FontWeight.bold,
                          color: AppTheme.warningOrange,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ],
            const SizedBox(height: 12),
            const Text(
              'For accurate attendance tracking, please:',
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14),
            ),
            const SizedBox(height: 8),
            Text(
              accuracy == null
                  ? (Platform.isIOS
                        ? '• Open Settings › Privacy & Security › Location Services › SIGOOK\n• Turn on Precise Location'
                        : '• Open App info › Permissions › Location\n• Turn on Use precise location')
                  : '• Enable High Accuracy location mode\n• Ensure GPS is enabled\n• Move to an open area with clear sky view',
              style: const TextStyle(fontSize: 13, height: 1.5),
            ),
          ],
        ),
        actionsPadding: const EdgeInsets.fromLTRB(24, 0, 24, 20),
        actions: [
          Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              ElevatedButton.icon(
                onPressed: () async {
                  Navigator.of(context).pop(_PrecisionAction.cancel);
                  await _openLocationSettings();
                },
                icon: const Icon(Icons.settings, size: 20),
                label: const Text(
                  'Open Location Settings',
                  style: TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
                ),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppTheme.primaryBlue,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 14),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  elevation: 2,
                ),
              ),
              if (allowRetry) ...[
                const SizedBox(height: 12),
                OutlinedButton.icon(
                  onPressed: () =>
                      Navigator.of(context).pop(_PrecisionAction.retry),
                  icon: const Icon(Icons.refresh, size: 20),
                  label: const Text(
                    'Try Again',
                    style: TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
                  ),
                  style: OutlinedButton.styleFrom(
                    foregroundColor: AppTheme.primaryBlue,
                    side: const BorderSide(
                      color: AppTheme.primaryBlue,
                      width: 2,
                    ),
                    padding: const EdgeInsets.symmetric(vertical: 14),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                ),
              ],
              if (allowContinue) ...[
                const SizedBox(height: 12),
                OutlinedButton(
                  onPressed: () => Navigator.of(
                    context,
                  ).pop(_PrecisionAction.continueAnyway),
                  style: OutlinedButton.styleFrom(
                    foregroundColor: AppTheme.warningOrange,
                    side: const BorderSide(
                      color: AppTheme.warningOrange,
                      width: 2,
                    ),
                    padding: const EdgeInsets.symmetric(vertical: 14),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                  child: const Text(
                    'Continue Anyway',
                    style: TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
                  ),
                ),
              ],
              const SizedBox(height: 8),
              TextButton(
                onPressed: () =>
                    Navigator.of(context).pop(_PrecisionAction.cancel),
                style: TextButton.styleFrom(
                  foregroundColor: Colors.grey,
                  padding: const EdgeInsets.symmetric(vertical: 12),
                ),
                child: const Text('Cancel', style: TextStyle(fontSize: 15)),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Future<void> _openLocationSettings() async {
    try {
      if (Platform.isAndroid) {
        // Try to open app-specific location settings first
        final opened = await permission_handler.openAppSettings();
        if (!opened) {
          // Fallback to general location settings
          await Geolocator.openLocationSettings();
        }
      } else if (Platform.isIOS) {
        // iOS opens app-specific settings
        await permission_handler.openAppSettings();
      }
    } catch (e) {
      debugPrint('Failed to open location settings: $e');
      if (mounted) {
        _showErrorSnackBar(
          'Could not open settings. Please manually enable precise location.',
        );
      }
    }
  }
}

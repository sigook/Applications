import 'dart:async';
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:geolocator/geolocator.dart';
import 'package:intl/intl.dart';
import 'package:table_calendar/table_calendar.dart';
import 'package:permission_handler/permission_handler.dart'
    as permission_handler;
import '../../../../core/theme/app_theme.dart';
import '../../../../core/constants/enums.dart';
import '../../../../core/providers/analytics_providers.dart';
import '../../domain/usecases/get_clock_type.dart';
import '../../domain/usecases/submit_timesheet.dart';
import '../providers/timesheet_providers.dart';

class PunchCardTab extends ConsumerStatefulWidget {
  final String jobId;

  const PunchCardTab({super.key, required this.jobId});

  @override
  ConsumerState<PunchCardTab> createState() => _PunchCardTabState();
}

class _PunchCardTabState extends ConsumerState<PunchCardTab> {
  DateTime _focusedDay = DateTime.now();
  DateTime _selectedDay = DateTime.now();
  DateTime _currentTime = DateTime.now();
  Timer? _timer;
  ClockType _clockType = ClockType.none;
  bool _isSubmitting = false;

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
  }

  Future<void> _loadClockType() async {
    final useCase = ref.read(getClockTypeUseCaseProvider);
    final result = await useCase(
      GetClockTypeParams(date: _selectedDay, requestId: widget.jobId),
    );

    if (!mounted) return;

    result.fold(
      (failure) {
        setState(() {
          _clockType = ClockType.none;
        });
      },
      (clockType) {
        setState(() {
          _clockType = clockType;
        });
      },
    );
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      padding: const EdgeInsets.only(bottom: 100),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const SizedBox(height: 12),
          _buildCalendarCard(),
          const SizedBox(height: 12),
          _buildClockCard(),
        ],
      ),
    );
  }

  Widget _buildCalendarCard() {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Select Date',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: AppTheme.textDark,
              ),
            ),
            const SizedBox(height: 16),
            TableCalendar(
              firstDay: DateTime.utc(2020, 1, 1),
              lastDay: DateTime.utc(2030, 12, 31),
              focusedDay: _focusedDay,
              selectedDayPredicate: (day) => isSameDay(_selectedDay, day),
              onDaySelected: (selectedDay, focusedDay) {
                setState(() {
                  _selectedDay = selectedDay;
                  _focusedDay = focusedDay;
                });
                _loadClockType();
              },
              calendarStyle: CalendarStyle(
                todayDecoration: BoxDecoration(
                  color: AppTheme.primaryBlue.withValues(alpha: 0.3),
                  shape: BoxShape.circle,
                ),
                selectedDecoration: const BoxDecoration(
                  color: AppTheme.primaryBlue,
                  shape: BoxShape.circle,
                ),
                selectedTextStyle: const TextStyle(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                ),
                todayTextStyle: const TextStyle(
                  color: AppTheme.primaryBlue,
                  fontWeight: FontWeight.bold,
                ),
                weekendTextStyle: TextStyle(
                  color: AppTheme.errorRed.withValues(alpha: 0.7),
                ),
              ),
              headerStyle: const HeaderStyle(
                formatButtonVisible: false,
                titleCentered: true,
                titleTextStyle: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                  color: AppTheme.textDark,
                ),
              ),
              daysOfWeekStyle: DaysOfWeekStyle(
                weekdayStyle: TextStyle(
                  fontWeight: FontWeight.bold,
                  color: Colors.grey.shade700,
                ),
                weekendStyle: TextStyle(
                  fontWeight: FontWeight.bold,
                  color: AppTheme.errorRed.withValues(alpha: 0.7),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildClockCard() {
    return Column(
      children: [
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
                      DateFormat('EEEE, MMMM dd, yyyy').format(_selectedDay),
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
        const SizedBox(height: 24),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16),
          child: SizedBox(
            width: double.infinity,
            height: 56,
            child: ElevatedButton(
              onPressed: _clockType == ClockType.none || _isSubmitting
                  ? null
                  : _onClockTapped,
              style: ElevatedButton.styleFrom(
                backgroundColor: _clockType == ClockType.clockIn
                    ? AppTheme.successGreen
                    : _clockType == ClockType.clockOut
                    ? AppTheme.errorRed
                    : Colors.grey,
                foregroundColor: Colors.white,
                elevation: 4,
                shadowColor: AppTheme.primaryBlue.withValues(alpha: 0.4),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
                disabledBackgroundColor: Colors.grey.shade400,
              ),
              child: _isSubmitting
                  ? const SizedBox(
                      height: 24,
                      width: 24,
                      child: CircularProgressIndicator(
                        strokeWidth: 2.5,
                        valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                      ),
                    )
                  : Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(
                          _clockType == ClockType.clockIn
                              ? Icons.login
                              : _clockType == ClockType.clockOut
                              ? Icons.logout
                              : Icons.block,
                          size: 24,
                        ),
                        const SizedBox(width: 12),
                        Text(
                          _clockType == ClockType.clockIn
                              ? 'CLOCK IN'
                              : _clockType == ClockType.clockOut
                              ? 'CLOCK OUT'
                              : 'UNAVAILABLE',
                          style: const TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                            letterSpacing: 1,
                          ),
                        ),
                      ],
                    ),
            ),
          ),
        ),
      ],
    );
  }

  Future<void> _onClockTapped() async {
    setState(() {
      _isSubmitting = true;
    });

    try {
      final position = await _getCurrentLocation();

      if (position == null) {
        if (!mounted) return;
        setState(() {
          _isSubmitting = false;
        });
        _showErrorSnackBar(
          'Failed to get location. Please enable location services.',
        );
        return;
      }

      // Check location precision
      if (!mounted) return;
      final locationAccuracy = position.accuracy;
      final isPrecise = locationAccuracy <= 20.0; // 20 meters threshold

      if (!isPrecise) {
        setState(() {
          _isSubmitting = false;
        });
        final shouldContinue = await _showLocationPrecisionWarning(
          locationAccuracy,
        );
        if (shouldContinue != true) {
          return;
        }
        setState(() {
          _isSubmitting = true;
        });
      }

      // Show confirmation modal
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

      final useCase = ref.read(submitTimesheetUseCaseProvider);
      final result = await useCase(
        SubmitTimesheetParams(
          jobId: widget.jobId,
          latitude: position.latitude,
          longitude: position.longitude,
        ),
      );

      if (!mounted) return;

      setState(() {
        _isSubmitting = false;
      });

      result.fold(
        (failure) {
          final is3MinuteRestriction =
              (failure.message.contains('400') ||
                  failure.message.toLowerCase().contains('bad request') ||
                  failure.message.toLowerCase().contains('too soon') ||
                  failure.message.toLowerCase().contains('wait')) &&
              _clockType == ClockType.clockOut;

          if (is3MinuteRestriction) {
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
        (response) {
          _showSuccessSnackBar(response.workerFullName, response.finish);
          _loadClockType();
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

  Future<Position?> _getCurrentLocation() async {
    bool serviceEnabled;
    LocationPermission permission;

    serviceEnabled = await Geolocator.isLocationServiceEnabled();
    if (!serviceEnabled) {
      return null;
    }

    permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
      if (permission == LocationPermission.denied) {
        return null;
      }
    }

    if (permission == LocationPermission.deniedForever) {
      return null;
    }

    return await Geolocator.getCurrentPosition(
      desiredAccuracy: LocationAccuracy.high,
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

  void _showErrorSnackBar(String message) {
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

  Future<bool?> _showLocationPrecisionWarning(double accuracy) async {
    return showDialog<bool>(
      context: context,
      barrierDismissible: false,
      builder: (context) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Row(
          children: [
            Icon(
              Icons.warning_amber_rounded,
              color: AppTheme.warningOrange,
              size: 28,
            ),
            SizedBox(width: 12),
            Text('Location Not Precise'),
          ],
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Your location accuracy is not precise enough for clock in/out.',
              style: TextStyle(fontSize: 16),
            ),
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
            const SizedBox(height: 12),
            const Text(
              'For accurate attendance tracking, please:',
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14),
            ),
            const SizedBox(height: 8),
            const Text(
              '• Enable High Accuracy location mode\n• Ensure GPS is enabled\n• Move to an open area with clear sky view',
              style: TextStyle(fontSize: 13, height: 1.5),
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
                  Navigator.of(context).pop(false);
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
              const SizedBox(height: 12),
              OutlinedButton(
                onPressed: () => Navigator.of(context).pop(true),
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
              const SizedBox(height: 8),
              TextButton(
                onPressed: () => Navigator.of(context).pop(false),
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

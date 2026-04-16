import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../../core/providers/analytics_providers.dart';
import '../../../../../../core/theme/app_theme.dart';

class DiagnosticsSectionCard extends ConsumerStatefulWidget {
  const DiagnosticsSectionCard({super.key});

  @override
  ConsumerState<DiagnosticsSectionCard> createState() =>
      _DiagnosticsSectionCardState();
}

class _DiagnosticsSectionCardState
    extends ConsumerState<DiagnosticsSectionCard> {
  String? _lastResult;
  bool _isBusy = false;

  Future<void> _run(String label, Future<void> Function() action) async {
    setState(() {
      _isBusy = true;
      _lastResult = null;
    });
    try {
      await action();
      if (mounted) {
        setState(() => _lastResult = '$label — sent. Check Azure in ~2 min.');
      }
    } catch (e) {
      if (mounted) setState(() => _lastResult = '$label — local error: $e');
    } finally {
      if (mounted) setState(() => _isBusy = false);
    }
  }

  Future<void> _testConnection() async {
    setState(() {
      _isBusy = true;
      _lastResult = null;
    });
    final client = ref.read(azureAppInsightsClientProvider);
    final status = await client.testConnection();
    if (mounted) {
      setState(() {
        _isBusy = false;
        _lastResult = status != null
            ? 'HTTP $status — ${status == 200 ? 'Azure accepted the payload ✓' : 'unexpected status'}'
            : 'Connection failed — check logs';
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: const BorderSide(color: Color(0xFFFFB300), width: 1.5),
      ),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                const Icon(Icons.bug_report_outlined,
                    color: Color(0xFFFFB300), size: 20),
                const SizedBox(width: 8),
                const Text(
                  'App Insights Diagnostics',
                  style: TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 15,
                    color: AppTheme.textDark,
                  ),
                ),
                const Spacer(),
                Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                  decoration: BoxDecoration(
                    color: const Color(0xFFFFB300).withValues(alpha: 0.15),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: const Text(
                    'STAGING',
                    style: TextStyle(
                      fontSize: 10,
                      fontWeight: FontWeight.bold,
                      color: Color(0xFFFFB300),
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            _DiagButton(
              label: 'Test connection (sync)',
              icon: Icons.wifi_tethering,
              onTap: _isBusy ? null : _testConnection,
            ),
            const SizedBox(height: 8),
            _DiagButton(
              label: 'Send test event',
              icon: Icons.send_outlined,
              onTap: _isBusy
                  ? null
                  : () => _run(
                        'Event',
                        () => ref
                            .read(analyticsServiceProvider)
                            .logEvent(name: 'test_event_from_debug_panel'),
                      ),
            ),
            const SizedBox(height: 8),
            _DiagButton(
              label: 'Send test exception',
              icon: Icons.error_outline,
              onTap: _isBusy
                  ? null
                  : () => _run(
                        'Exception',
                        () => ref
                            .read(crashReportingServiceProvider)
                            .recordError(
                              Exception('Test exception from debug panel'),
                              StackTrace.current,
                              reason: 'Manual diagnostic test',
                            ),
                      ),
            ),
            const SizedBox(height: 8),
            _DiagButton(
              label: 'Send test trace',
              icon: Icons.notes_outlined,
              onTap: _isBusy
                  ? null
                  : () => _run(
                        'Trace',
                        () => ref
                            .read(crashReportingServiceProvider)
                            .log('Test trace from debug panel'),
                      ),
            ),
            if (_lastResult != null) ...[
              const SizedBox(height: 12),
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: Colors.grey.shade100,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    if (_isBusy)
                      const SizedBox(
                        width: 14,
                        height: 14,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    else
                      const Icon(Icons.check_circle_outline,
                          size: 16, color: AppTheme.successGreen),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        _lastResult!,
                        style: const TextStyle(fontSize: 12),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}

class _DiagButton extends StatelessWidget {
  final String label;
  final IconData icon;
  final VoidCallback? onTap;

  const _DiagButton({
    required this.label,
    required this.icon,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: double.infinity,
      child: OutlinedButton.icon(
        onPressed: onTap,
        icon: Icon(icon, size: 18),
        label: Text(label),
        style: OutlinedButton.styleFrom(
          foregroundColor: AppTheme.primaryBlue,
          side: BorderSide(color: Colors.grey.shade300),
          alignment: Alignment.centerLeft,
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
        ),
      ),
    );
  }
}

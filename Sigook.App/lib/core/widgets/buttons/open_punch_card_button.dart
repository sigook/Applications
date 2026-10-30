import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../routing/app_router.dart';
import '../../theme/app_theme.dart';

class OpenPunchCardButton extends StatelessWidget {
  final String jobId;

  const OpenPunchCardButton({super.key, required this.jobId});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: double.infinity,
      height: 48,
      child: ElevatedButton.icon(
        onPressed: () => context.push(
          AppRoutes.jobDetails,
          extra: {'id': jobId, 'tab': JobPageTab.punchCard},
        ),
        style: ElevatedButton.styleFrom(
          backgroundColor: AppTheme.secondaryRed,
          foregroundColor: Colors.white,
          elevation: 0,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
          textStyle: const TextStyle(
            fontSize: 15,
            fontWeight: FontWeight.w700,
            letterSpacing: 0.6,
          ),
        ),
        icon: const Icon(Icons.access_time, size: 20),
        label: const Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text('OPEN PUNCH CARD'),
            SizedBox(width: 4),
            Icon(Icons.chevron_right, size: 20),
          ],
        ),
      ),
    );
  }
}

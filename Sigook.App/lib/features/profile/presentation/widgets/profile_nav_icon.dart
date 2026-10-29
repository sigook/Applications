import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../providers/cached_worker_profile_provider.dart';

class ProfileNavIcon extends ConsumerWidget {
  final IconData icon;

  const ProfileNavIcon({super.key, required this.icon});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final missing =
        ref.watch(cachedWorkerProfileProvider).value?.missingRequirements.length ??
        0;

    return Semantics(
      label: missing > 0 ? '$missing profile items missing' : null,
      child: Badge.count(
        count: missing,
        isLabelVisible: missing > 0,
        backgroundColor: AppTheme.errorRed,
        child: Icon(icon),
      ),
    );
  }
}

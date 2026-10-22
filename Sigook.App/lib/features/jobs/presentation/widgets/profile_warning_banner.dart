import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../profile/domain/entities/worker_profile.dart';
import '../../../profile/domain/entities/worker_requirement.dart';
import '../../../profile/presentation/providers/cached_worker_profile_provider.dart';

class ProfileWarningBanner extends ConsumerWidget {
  const ProfileWarningBanner({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final profile = ref.watch(cachedWorkerProfileProvider).value;
    if (profile == null) return const SizedBox.shrink();

    final showNotApproved = !profile.approvedToWork;
    final showMissingRequirements = profile.hasMissingRequirements;
    if (!showNotApproved && !showMissingRequirements) {
      return const SizedBox.shrink();
    }

    return Column(
      children: [
        if (showNotApproved) const NotApprovedCard(),
        if (showMissingRequirements) _MissingRequirementsCard(profile: profile),
      ],
    );
  }
}

class NotApprovedCard extends StatelessWidget {
  const NotApprovedCard({super.key});

  @override
  Widget build(BuildContext context) {
    return const _WarningCard(
      icon: Icons.hourglass_top_outlined,
      child: Text(
        'You are not approved to work',
        style: TextStyle(
          fontSize: 15,
          fontWeight: FontWeight.w600,
          color: AppTheme.textDark,
        ),
      ),
    );
  }
}

class _MissingRequirementsCard extends StatelessWidget {
  final WorkerProfile profile;

  const _MissingRequirementsCard({required this.profile});

  @override
  Widget build(BuildContext context) {
    return _WarningCard(
      icon: Icons.assignment_late_outlined,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Your profile is incomplete',
            style: TextStyle(
              fontSize: 15,
              fontWeight: FontWeight.w600,
              color: AppTheme.textDark,
            ),
          ),
          const SizedBox(height: 4),
          const Text(
            'Please update the following information to be able to work with us.',
            style: TextStyle(fontSize: 13, color: AppTheme.textMedium),
          ),
          const SizedBox(height: 8),
          for (final requirement in profile.missingRequirements)
            _RequirementRow(requirement: requirement),
          const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            child: ElevatedButton.icon(
              onPressed: () => context.go(AppRoutes.profile),
              style: ElevatedButton.styleFrom(
                padding: const EdgeInsets.symmetric(
                  horizontal: 16,
                  vertical: 12,
                ),
                elevation: 0,
              ),
              icon: const Icon(Icons.person_outline, size: 18),
              label: const Text(
                'Open my profile',
                style: TextStyle(fontSize: 14, fontWeight: FontWeight.w600),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _RequirementRow extends StatelessWidget {
  final WorkerRequirement requirement;

  const _RequirementRow({required this.requirement});

  @override
  Widget build(BuildContext context) {
    final section = requirement.section;
    return Semantics(
      button: section != null ? true : null,
      child: InkWell(
        onTap: section == null ? null : () => _openSection(context, section),
        borderRadius: BorderRadius.circular(8),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 6),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Padding(
                padding: EdgeInsets.only(top: 6),
                child: Icon(
                  Icons.circle,
                  size: 6,
                  color: AppTheme.warningOrange,
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      requirement.label,
                      style: const TextStyle(
                        fontSize: 13,
                        fontWeight: FontWeight.w600,
                        color: AppTheme.textDark,
                      ),
                    ),
                    Text(
                      requirement.hint,
                      style: const TextStyle(
                        fontSize: 12,
                        color: AppTheme.textMedium,
                      ),
                    ),
                  ],
                ),
              ),
              if (section != null)
                const Icon(
                  Icons.chevron_right,
                  size: 18,
                  color: AppTheme.textMedium,
                ),
            ],
          ),
        ),
      ),
    );
  }

  void _openSection(BuildContext context, ProfileSection section) {
    context.go(
      Uri(
        path: AppRoutes.profile,
        queryParameters: {'section': section.name},
      ).toString(),
    );
  }
}

class _WarningCard extends StatelessWidget {
  final IconData icon;
  final Widget child;

  const _WarningCard({required this.icon, required this.child});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppTheme.warningOrange.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: AppTheme.warningOrange.withValues(alpha: 0.5),
        ),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, color: AppTheme.warningOrange, size: 24),
          const SizedBox(width: 12),
          Expanded(child: child),
        ],
      ),
    );
  }
}

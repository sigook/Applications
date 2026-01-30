import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/providers/locale_provider.dart';

class LanguageSettingsPage extends ConsumerWidget {
  const LanguageSettingsPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final currentLocale = ref.watch(localeProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Language'),
        backgroundColor: AppTheme.primaryBlue,
        foregroundColor: Colors.white,
      ),
      body: ListView(
        children: [
          _buildLanguageTile(
            context: context,
            ref: ref,
            locale: const Locale('en'),
            title: 'English',
            subtitle: 'English',
            isSelected: currentLocale.languageCode == 'en',
          ),
          const Divider(height: 1),
          _buildLanguageTile(
            context: context,
            ref: ref,
            locale: const Locale('es'),
            title: 'Español',
            subtitle: 'Spanish',
            isSelected: currentLocale.languageCode == 'es',
          ),
          const Divider(height: 1),
          _buildLanguageTile(
            context: context,
            ref: ref,
            locale: const Locale('fr'),
            title: 'Français',
            subtitle: 'French',
            isSelected: currentLocale.languageCode == 'fr',
          ),
        ],
      ),
    );
  }

  Widget _buildLanguageTile({
    required BuildContext context,
    required WidgetRef ref,
    required Locale locale,
    required String title,
    required String subtitle,
    required bool isSelected,
  }) {
    return ListTile(
      title: Text(
        title,
        style: AppTheme.bodyLarge.copyWith(
          fontWeight: isSelected ? FontWeight.w600 : FontWeight.normal,
        ),
      ),
      subtitle: Text(subtitle, style: AppTheme.bodySmall),
      trailing: isSelected
          ? const Icon(Icons.check_circle, color: AppTheme.primaryBlue)
          : null,
      onTap: () async {
        await ref.read(localeProvider.notifier).setLocale(locale);

        if (context.mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('Language changed to $title'),
              duration: const Duration(seconds: 2),
              backgroundColor: AppTheme.successGreen,
            ),
          );
        }
      },
    );
  }
}

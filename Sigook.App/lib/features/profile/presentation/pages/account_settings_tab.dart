import 'package:flutter/material.dart';
import 'preferences/sections/account_settings_section.dart';

class AccountSettingsTab extends StatelessWidget {
  const AccountSettingsTab({super.key});

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: EdgeInsets.zero,
      children: [
        const SizedBox(height: 16),
        AccountSettingsSectionCard(),
        const SizedBox(height: 24),
      ],
    );
  }
}

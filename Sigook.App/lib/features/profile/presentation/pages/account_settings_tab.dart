import 'package:flutter/material.dart';
import 'preferences/sections/account_settings_section.dart';

class AccountSettingsTab extends StatelessWidget {
  final String appVersion;

  const AccountSettingsTab({super.key, required this.appVersion});

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: EdgeInsets.zero,
      children: [
        const SizedBox(height: 16),
        const AccountSettingsSectionCard(),
        const SizedBox(height: 24),
        if (appVersion.isNotEmpty)
          Padding(
            padding: const EdgeInsets.only(bottom: 24),
            child: Center(
              child: Text(
                appVersion,
                style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
              ),
            ),
          ),
      ],
    );
  }
}

import 'package:flutter/material.dart';

class ServiceItem {
  final IconData icon;
  final String title;
  final String description;
  final Color color;

  const ServiceItem({
    required this.icon,
    required this.title,
    required this.description,
    required this.color,
  });
}

class IndustryItem {
  final IconData icon;
  final String label;

  const IndustryItem({required this.icon, required this.label});
}

class CategoryItem {
  final IconData icon;
  final String title;
  final String subtitle;

  const CategoryItem({
    required this.icon,
    required this.title,
    required this.subtitle,
  });
}

const kAboutServices = [
  ServiceItem(
    icon: Icons.access_time_rounded,
    title: 'Temporary',
    description: 'Short-term placements to cover seasonal or project-based needs.',
    color: Color(0xFF1565C0),
  ),
  ServiceItem(
    icon: Icons.sync_alt_rounded,
    title: 'Temp-to-Hire',
    description: 'Start temporary and transition to a permanent role.',
    color: Color(0xFF0277BD),
  ),
  ServiceItem(
    icon: Icons.verified_user_rounded,
    title: 'Permanent',
    description: 'Direct permanent placements for long-term positions.',
    color: Color(0xFF43A047),
  ),
  ServiceItem(
    icon: Icons.assignment_rounded,
    title: 'Contract',
    description: 'Fixed-term contracts for specialized project work.',
    color: Color(0xFFE53935),
  ),
];

const kAboutIndustries = [
  IndustryItem(icon: Icons.factory_rounded, label: 'Light Industry'),
  IndustryItem(icon: Icons.business_center_rounded, label: 'Clerical'),
  IndustryItem(icon: Icons.construction_rounded, label: 'Skilled Trades'),
  IndustryItem(icon: Icons.computer_rounded, label: 'Information Technology'),
];

const kAboutCategories = [
  CategoryItem(
    icon: Icons.admin_panel_settings_rounded,
    title: 'Administrative',
    subtitle: 'Office support, data entry, coordination',
  ),
  CategoryItem(
    icon: Icons.headset_mic_rounded,
    title: 'Customer Service',
    subtitle: 'Call center, support, client relations',
  ),
  CategoryItem(
    icon: Icons.warehouse_rounded,
    title: 'Warehouse & Supply Chain',
    subtitle: 'Logistics, forklift, inventory management',
  ),
  CategoryItem(
    icon: Icons.precision_manufacturing_rounded,
    title: 'Industrial & Manufacturing',
    subtitle: 'Assembly, production, quality control',
  ),
  CategoryItem(
    icon: Icons.code_rounded,
    title: 'Technology & IT',
    subtitle: 'Software, networking, technical support',
  ),
];

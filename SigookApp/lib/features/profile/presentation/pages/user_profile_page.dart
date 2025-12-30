import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/widgets/profile_section_card.dart';
import '../../../../core/widgets/profile_info_row.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../widgets/profile_header.dart';

class UserProfilePage extends ConsumerStatefulWidget {
  const UserProfilePage({super.key});

  @override
  ConsumerState<UserProfilePage> createState() => _UserProfilePageState();
}

class _UserProfilePageState extends ConsumerState<UserProfilePage> {
  bool _isEditing = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        backgroundColor: AppTheme.primaryBlue,
        foregroundColor: Colors.white,
        elevation: 0,
        title: const Text(
          'My Profile',
          style: TextStyle(fontSize: 20, fontWeight: FontWeight.w600),
        ),
        actions: [
          IconButton(
            icon: Icon(_isEditing ? Icons.close : Icons.edit),
            onPressed: () {
              setState(() {
                _isEditing = !_isEditing;
              });
            },
          ),
        ],
      ),
      body: SingleChildScrollView(
        child: Column(
          children: [
            ProfileHeader(
              name: 'Juan Betancur',
              email: 'juanm@sigook.com',
              isEditing: _isEditing,
            ),
            const SizedBox(height: 16),
            _buildPersonalInfoSection(),
            const SizedBox(height: 12),
            _buildContactSection(),
            const SizedBox(height: 12),
            _buildLocationSection(),
            const SizedBox(height: 12),
            _buildPreferencesSection(),
            const SizedBox(height: 12),
            _buildDocumentsSection(),
            const SizedBox(height: 12),
            _buildCommentsSection(),
            const SizedBox(height: 24),
            _buildActionButtons(),
            const SizedBox(height: 32),
          ],
        ),
      ),
    );
  }

  Widget _buildPersonalInfoSection() {
    return ProfileSectionCard(
      title: 'Personal Information',
      icon: Icons.person_outline,
      iconGradient: const [AppTheme.primaryBlue, AppTheme.tertiaryBlue],
      children: [
        ProfileInfoRow(
          label: 'First Name',
          value: 'Juan',
          icon: Icons.badge_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'Last Name',
          value: 'Betancur',
          icon: Icons.badge_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'Date of Birth',
          value: 'January 15, 1990',
          icon: Icons.cake_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'Gender',
          value: 'Male',
          icon: Icons.wc_outlined,
          isEditing: _isEditing,
        ),
      ],
    );
  }

  Widget _buildContactSection() {
    return ProfileSectionCard(
      title: 'Contact Information',
      icon: Icons.contact_phone_outlined,
      iconGradient: const [Color(0xFF4CAF50), Color(0xFF81C784)],
      children: [
        ProfileInfoRow(
          label: 'Mobile Number',
          value: '+1 (555) 123-4567',
          icon: Icons.phone_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'Email',
          value: 'juanm@sigook.com',
          icon: Icons.email_outlined,
          isEditing: _isEditing,
        ),
      ],
    );
  }

  Widget _buildLocationSection() {
    return ProfileSectionCard(
      title: 'Location Details',
      icon: Icons.location_on_outlined,
      iconGradient: const [Color(0xFFFF9800), Color(0xFFFFB74D)],
      children: [
        ProfileInfoRow(
          label: 'Country',
          value: 'United States',
          icon: Icons.flag_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'State/Province',
          value: 'California',
          icon: Icons.map_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'City',
          value: 'Los Angeles',
          icon: Icons.location_city_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'Address',
          value: '123 Main Street, Apt 4B',
          icon: Icons.home_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'Zip Code',
          value: '90001',
          icon: Icons.markunread_mailbox_outlined,
          isEditing: _isEditing,
        ),
      ],
    );
  }

  Widget _buildPreferencesSection() {
    return ProfileSectionCard(
      title: 'Work Preferences',
      icon: Icons.work_outline,
      iconGradient: const [Color(0xFF9C27B0), Color(0xFFBA68C8)],
      children: [
        ProfileInfoRow(
          label: 'Availability',
          value: 'Full-time',
          icon: Icons.schedule_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'Available Days',
          value: 'Mon, Tue, Wed, Thu, Fri',
          icon: Icons.calendar_today_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'Preferred Time',
          value: 'Morning, Afternoon',
          icon: Icons.access_time_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'Lifting Capacity',
          value: 'Up to 50 lbs',
          icon: Icons.fitness_center_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'Has Vehicle',
          value: 'Yes',
          icon: Icons.directions_car_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'Languages',
          value: 'English, Spanish',
          icon: Icons.language_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'Skills',
          value: 'Customer Service, Warehouse, Forklift',
          icon: Icons.stars_outlined,
          isEditing: _isEditing,
        ),
      ],
    );
  }

  Widget _buildDocumentsSection() {
    return ProfileSectionCard(
      title: 'Documents & Account',
      icon: Icons.description_outlined,
      iconGradient: const [Color(0xFFF44336), Color(0xFFE57373)],
      children: [
        ProfileInfoRow(
          label: 'ID Type',
          value: 'Driver\'s License',
          icon: Icons.credit_card_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'ID Number',
          value: '****6789',
          icon: Icons.numbers_outlined,
          isEditing: _isEditing,
        ),
        ProfileInfoRow(
          label: 'Username',
          value: 'juan_betancur',
          icon: Icons.account_circle_outlined,
          isEditing: _isEditing,
        ),
      ],
    );
  }

  Widget _buildCommentsSection() {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 10,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  gradient: const LinearGradient(
                    colors: [Color(0xFF2196F3), Color(0xFF64B5F6)],
                  ),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(
                  Icons.comment_outlined,
                  color: Colors.white,
                  size: 20,
                ),
              ),
              const SizedBox(width: 12),
              const Text(
                'Additional Comments',
                style: TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                  color: AppTheme.textDark,
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          if (_isEditing)
            TextField(
              maxLines: 4,
              decoration: InputDecoration(
                hintText: 'Add any additional information...',
                filled: true,
                fillColor: Colors.grey.shade50,
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: BorderSide(color: Colors.grey.shade300),
                ),
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: BorderSide(color: Colors.grey.shade300),
                ),
                focusedBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: const BorderSide(
                    color: AppTheme.primaryBlue,
                    width: 2,
                  ),
                ),
              ),
            )
          else
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: Colors.grey.shade50,
                borderRadius: BorderRadius.circular(12),
                border: Border.all(color: Colors.grey.shade200),
              ),
              child: Text(
                'Available for flexible work schedules. Prefer morning shifts but can accommodate afternoon and evening shifts as needed.',
                style: TextStyle(
                  fontSize: 14,
                  color: Colors.grey.shade700,
                  height: 1.5,
                ),
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildActionButtons() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Column(
        children: [
          if (_isEditing)
            SizedBox(
              width: double.infinity,
              height: 56,
              child: ElevatedButton(
                onPressed: () {
                  setState(() {
                    _isEditing = false;
                  });
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(
                      content: Text('Profile updated successfully!'),
                      backgroundColor: AppTheme.successGreen,
                      behavior: SnackBarBehavior.floating,
                    ),
                  );
                },
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppTheme.primaryBlue,
                  foregroundColor: Colors.white,
                  elevation: 2,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(16),
                  ),
                ),
                child: const Text(
                  'Save Changes',
                  style: TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    letterSpacing: 0.5,
                  ),
                ),
              ),
            ),
          if (_isEditing) const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            height: 56,
            child: OutlinedButton(
              onPressed: () => _showLogoutDialog(),
              style: OutlinedButton.styleFrom(
                foregroundColor: AppTheme.secondaryRed,
                side: const BorderSide(color: AppTheme.secondaryRed, width: 2),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
              ),
              child: const Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.logout, size: 22),
                  SizedBox(width: 12),
                  Text(
                    'Logout',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      letterSpacing: 0.5,
                    ),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            height: 56,
            child: OutlinedButton(
              onPressed: () => _showDeleteAccountDialog(),
              style: OutlinedButton.styleFrom(
                foregroundColor: AppTheme.errorRed,
                side: const BorderSide(color: AppTheme.errorRed, width: 2),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
              ),
              child: const Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.delete_forever, size: 22),
                  SizedBox(width: 12),
                  Text(
                    'Delete Account',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      letterSpacing: 0.5,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  void _showLogoutDialog() async {
    final shouldLogout = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Logout'),
        content: const Text('Are you sure you want to logout?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.of(context).pop(true),
            style: ElevatedButton.styleFrom(
              backgroundColor: AppTheme.secondaryRed,
            ),
            child: const Text('Logout'),
          ),
        ],
      ),
    );

    if (shouldLogout == true && mounted) {
      await ref.read(authViewModelProvider.notifier).logout();
      if (mounted) {
        Navigator.of(context).pushReplacementNamed(AppRoutes.signIn);
      }
    }
  }

  void _showDeleteAccountDialog() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Row(
          children: [
            Icon(Icons.warning, color: AppTheme.errorRed),
            SizedBox(width: 8),
            Text('Delete Account'),
          ],
        ),
        content: const Text(
          'This action cannot be undone. All your data will be permanently deleted. Are you sure you want to delete your account?',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () {
              Navigator.of(context).pop();
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('Account deletion feature coming soon!'),
                  behavior: SnackBarBehavior.floating,
                ),
              );
            },
            style: ElevatedButton.styleFrom(backgroundColor: AppTheme.errorRed),
            child: const Text('Delete'),
          ),
        ],
      ),
    );
  }
}

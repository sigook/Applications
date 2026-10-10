import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/core/theme/app_theme.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/basic_info.dart';
import 'package:sigook_app_flutter/features/registration/domain/usecases/pick_profile_photo.dart';
import '../../domain/entities/value_objects/profile_photo.dart';
import '../providers/registration_providers.dart';

class ProfilePhotoPicker extends ConsumerWidget {
  final double size;
  final String? errorText;
  final bool showError;

  const ProfilePhotoPicker({
    super.key,
    this.size = 120,
    this.errorText,
    this.showError = false,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final basicInfo = ref.watch(registrationViewModelProvider).basicInfo;
    final photo = basicInfo?.profilePhoto ?? ProfilePhoto.empty();

    return Column(
      children: [
        GestureDetector(
          onTap: () => _showPhotoSourceBottomSheet(context, ref),
          child: Container(
            width: size,
            height: size,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              border: Border.all(
                color: showError && !photo.hasPhoto
                    ? AppTheme.errorRed
                    : AppTheme.primaryBlue,
                width: showError && !photo.hasPhoto ? 3 : 3,
              ),
              image: photo.hasPhoto
                  ? DecorationImage(
                      image: FileImage(File(photo.path)),
                      fit: BoxFit.cover,
                    )
                  : null,
              color: photo.hasPhoto ? null : Colors.grey.shade200,
            ),
            child: !photo.hasPhoto
                ? Icon(
                    Icons.person,
                    size: size * 0.5,
                    color: Colors.grey.shade600,
                  )
                : null,
          ),
        ),

        const SizedBox(height: 16),

        OutlinedButton.icon(
          onPressed: () => _showPhotoSourceBottomSheet(context, ref),
          icon: const Icon(Icons.camera_alt, size: 18),
          label: Text(photo.hasPhoto ? 'Change Photo' : 'Add Profile Photo'),
          style: OutlinedButton.styleFrom(
            foregroundColor: AppTheme.primaryBlue,
            side: const BorderSide(color: AppTheme.primaryBlue),
            padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
          ),
        ),

        if (showError && errorText != null && !photo.hasPhoto)
          Padding(
            padding: const EdgeInsets.only(top: 8),
            child: Text(
              errorText!,
              style: TextStyle(color: AppTheme.errorRed, fontSize: 12),
              textAlign: TextAlign.center,
            ),
          ),
      ],
    );
  }

  Future<void> _showPhotoSourceBottomSheet(
    BuildContext context,
    WidgetRef ref,
  ) async {
    final source = await showModalBottomSheet<PhotoSource>(
      context: context,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (_) => SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            ListTile(
              leading: const Icon(Icons.camera_alt),
              title: const Text('Take Photo'),
              onTap: () => Navigator.pop(context, PhotoSource.camera),
            ),
            ListTile(
              leading: const Icon(Icons.photo_library),
              title: const Text('Choose from Gallery'),
              onTap: () => Navigator.pop(context, PhotoSource.gallery),
            ),
            const Divider(height: 1),
            ListTile(
              leading: const Icon(Icons.cancel, color: Colors.grey),
              title: const Text('Cancel', style: TextStyle(color: Colors.grey)),
              onTap: () => Navigator.pop(context),
            ),
          ],
        ),
      ),
    );

    if (source == null) return;

    final picker = ref.read(pickProfilePhotoProvider);
    final result = await picker(source);

    result.fold(
      (failure) {
        // Show permission dialog for PermissionFailure, SnackBar for others
        if (failure is PermissionFailure) {
          _showPermissionDeniedDialog(context, source);
        } else if (failure is! UserCancelledFailure) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text(failure.message),
              backgroundColor: AppTheme.errorRed,
            ),
          );
        }
      },
      (profilePhoto) {
        final notifier = ref.read(registrationViewModelProvider.notifier);
        final currentInfo = ref.read(registrationViewModelProvider).basicInfo;

        if (currentInfo == null) {
          BasicInfo basicInfo = BasicInfo.empty();
          notifier.updateBasicInfo(
            basicInfo.copyWith(profilePhoto: profilePhoto),
          );
          return;
        }
        notifier.updateBasicInfo(
          currentInfo.copyWith(profilePhoto: profilePhoto),
        );
      },
    );
  }

  /// Shows a dialog explaining how to grant camera/photo permissions
  /// with a button to open the app settings.
  void _showPermissionDeniedDialog(BuildContext context, PhotoSource source) {
    final isCamera = source == PhotoSource.camera;
    final permissionType = isCamera ? 'Camera' : 'Photo Library';

    showDialog(
      context: context,
      builder: (BuildContext dialogContext) => AlertDialog(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(16),
        ),
        title: Row(
          children: [
            Icon(
              Icons.no_photography_outlined,
              color: AppTheme.errorRed,
              size: 28,
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Text(
                '$permissionType Access Required',
                style: const TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ],
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'SIGOOK needs access to your ${isCamera ? 'camera' : 'photo library'} to ${isCamera ? 'take' : 'select'} a profile picture for your worker registration.',
              style: const TextStyle(fontSize: 14),
            ),
            const SizedBox(height: 16),
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: Colors.blue.shade50,
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: Colors.blue.shade100),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'How to enable $permissionType access:',
                    style: TextStyle(
                      fontWeight: FontWeight.w600,
                      color: Colors.blue.shade800,
                      fontSize: 13,
                    ),
                  ),
                  const SizedBox(height: 8),
                  _buildInstructionStep('1', 'Tap "Open Settings" below'),
                  _buildInstructionStep('2', 'Find "$permissionType" in the list'),
                  _buildInstructionStep(
                    '3',
                    isCamera
                        ? 'Toggle the switch to enable access'
                        : 'Select "Full Access" or "Add Photos Only"',
                  ),
                  _buildInstructionStep('4', 'Return to SIGOOK and try again'),
                ],
              ),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(dialogContext).pop(),
            child: Text(
              'Cancel',
              style: TextStyle(color: Colors.grey.shade600),
            ),
          ),
          ElevatedButton.icon(
            onPressed: () {
              Navigator.of(dialogContext).pop();
              openAppSettings();
            },
            icon: const Icon(Icons.settings, size: 18),
            label: const Text('Open Settings'),
            style: ElevatedButton.styleFrom(
              backgroundColor: AppTheme.primaryBlue,
              foregroundColor: Colors.white,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(8),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildInstructionStep(String number, String text) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 20,
            height: 20,
            decoration: BoxDecoration(
              color: Colors.blue.shade700,
              shape: BoxShape.circle,
            ),
            child: Center(
              child: Text(
                number,
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 11,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              text,
              style: TextStyle(
                fontSize: 12,
                color: Colors.blue.shade900,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

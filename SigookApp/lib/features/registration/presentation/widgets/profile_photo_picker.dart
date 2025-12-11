import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
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
        // Avatar
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

        // Botón
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

        // Error message
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
      (failure) => ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(failure.message),
          backgroundColor: AppTheme.errorRed,
        ),
      ),
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
}

import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';

class ProfileHeader extends StatelessWidget {
  final String name;
  final String email;
  final String? photoUrl;
  final bool isEditing;
  final VoidCallback? onPhotoTap;

  /// 1.0 = fully expanded, 0.0 = fully collapsed (invisible).
  final double collapseRatio;

  const ProfileHeader({
    super.key,
    required this.name,
    required this.email,
    this.photoUrl,
    this.isEditing = false,
    this.onPhotoTap,
    this.collapseRatio = 1.0,
  });

  @override
  Widget build(BuildContext context) {
    final t = collapseRatio.clamp(0.0, 1.0);

    // Nothing to render when fully (or nearly) collapsed.
    if (t < 0.05) {
      return Container(color: Colors.white);
    }

    final avatarRadius = 28.0 + (50.0 - 28.0) * t;
    final nameSize = 16.0 + (24.0 - 16.0) * t;
    // Email fades in only during the second half of expansion.
    final emailOpacity = ((t * 2.0) - 1.0).clamp(0.0, 1.0);

    final avatar = Stack(
      clipBehavior: Clip.none,
      children: [
        CircleAvatar(
          radius: avatarRadius,
          backgroundColor: AppTheme.primaryBlue.withValues(alpha: 0.1),
          backgroundImage: photoUrl != null ? NetworkImage(photoUrl!) : null,
          child: photoUrl == null
              ? Text(
                  _getInitials(name),
                  style: TextStyle(
                    fontSize: avatarRadius * 0.72,
                    fontWeight: FontWeight.bold,
                    color: AppTheme.primaryBlue,
                  ),
                )
              : null,
        ),
        if (isEditing)
          Positioned(
            right: 0,
            bottom: 0,
            child: GestureDetector(
              onTap: onPhotoTap,
              child: Container(
                padding: const EdgeInsets.all(6),
                decoration: BoxDecoration(
                  color: AppTheme.secondaryRed,
                  shape: BoxShape.circle,
                  border: Border.all(color: Colors.white, width: 2),
                ),
                child: const Icon(
                  Icons.camera_alt,
                  size: 14,
                  color: Colors.white,
                ),
              ),
            ),
          ),
      ],
    );

    // The background fills the full SliverAppBar extent which includes the
    // TabBar (48 px) at the bottom. Padding keeps content above the TabBar.
    // ClipRect prevents overflow when the avatar is larger than the shrinking
    // content area during the collapse animation.
    return ClipRect(
      child: Opacity(
        opacity: t,
        child: Container(
          width: double.infinity,
          color: Colors.white,
          padding: const EdgeInsets.only(bottom: 48.0),
          child: Center(
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 24.0),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  avatar,
                  SizedBox(height: 12.0 * t),
                  Text(
                    name,
                    style: TextStyle(
                      fontSize: nameSize,
                      fontWeight: FontWeight.bold,
                      color: AppTheme.textDark,
                    ),
                    textAlign: TextAlign.center,
                  ),
                  if (emailOpacity > 0) ...[
                    const SizedBox(height: 4),
                    Opacity(
                      opacity: emailOpacity,
                      child: Text(
                        email,
                        style: TextStyle(
                          fontSize: 16,
                          color: Colors.grey.shade600,
                        ),
                      ),
                    ),
                  ],
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  String _getInitials(String? name) {
    if (name == null || name.isEmpty) return 'U';
    final parts = name.trim().split(' ');
    if (parts.length >= 2) {
      return '${parts[0][0]}${parts[1][0]}'.toUpperCase();
    }
    return name[0].toUpperCase();
  }
}

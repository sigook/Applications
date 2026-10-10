import '../../domain/entities/app_version_requirement.dart';

class AppVersionRequirementModel extends AppVersionRequirement {
  const AppVersionRequirementModel({
    required super.minimumVersion,
    required super.androidStoreUrl,
    required super.iosStoreUrl,
  });

  factory AppVersionRequirementModel.fromJson(Map<String, dynamic> json) {
    return AppVersionRequirementModel(
      minimumVersion: (json['minimumVersion'] ?? '').toString(),
      androidStoreUrl: (json['androidStoreUrl'] ?? '').toString(),
      iosStoreUrl: (json['iosStoreUrl'] ?? '').toString(),
    );
  }

  AppVersionRequirement toEntity() => AppVersionRequirement(
        minimumVersion: minimumVersion,
        androidStoreUrl: androidStoreUrl,
        iosStoreUrl: iosStoreUrl,
      );
}

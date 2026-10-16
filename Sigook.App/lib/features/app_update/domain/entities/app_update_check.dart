import 'package:equatable/equatable.dart';
import 'app_version_requirement.dart';

class AppUpdateCheck extends Equatable {
  final String currentVersion;
  final AppVersionRequirement requirement;

  const AppUpdateCheck({
    required this.currentVersion,
    required this.requirement,
  });

  bool get updateRequired => !requirement.isSatisfiedBy(currentVersion);

  @override
  List<Object?> get props => [currentVersion, requirement];
}

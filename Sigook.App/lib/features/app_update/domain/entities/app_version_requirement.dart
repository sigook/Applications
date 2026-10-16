import 'package:equatable/equatable.dart';

class AppVersionRequirement extends Equatable {
  final String minimumVersion;
  final String androidStoreUrl;
  final String iosStoreUrl;

  const AppVersionRequirement({
    required this.minimumVersion,
    required this.androidStoreUrl,
    required this.iosStoreUrl,
  });

  bool isSatisfiedBy(String version) =>
      minimumVersion.trim().isEmpty ||
      compareVersions(version, minimumVersion) >= 0;

  static int compareVersions(String a, String b) {
    final left = _segments(a);
    final right = _segments(b);
    final length = left.length > right.length ? left.length : right.length;
    for (var i = 0; i < length; i++) {
      final l = i < left.length ? left[i] : 0;
      final r = i < right.length ? right[i] : 0;
      if (l != r) return l.compareTo(r);
    }
    return 0;
  }

  static List<int> _segments(String version) {
    final core = version.trim().split(RegExp(r'[+-]')).first;
    return core
        .split('.')
        .map((s) => int.tryParse(s.trim()) ?? 0)
        .toList(growable: false);
  }

  @override
  List<Object?> get props => [minimumVersion, androidStoreUrl, iosStoreUrl];
}

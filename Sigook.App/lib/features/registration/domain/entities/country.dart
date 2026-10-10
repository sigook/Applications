import 'package:equatable/equatable.dart';

class Country extends Equatable {
  final String? id;
  final String value;
  final String? code;

  const Country({this.id, required this.value, this.code});

  /// Mapping of known country names to ISO 3166-1 alpha-2 codes.
  static const _nameToIsoCode = {
    'canada': 'CA',
    'ca': 'CA',
    'united states': 'US',
    'united states of america': 'US',
    'usa': 'US',
    'us': 'US',
  };

  /// Returns the ISO country code, resolving from the explicit [code]
  /// field first, then falling back to matching the country [value] name.
  String? get isoCode => toIsoCode(code) ?? toIsoCode(value);

  static String? toIsoCode(String? codeOrName) {
    if (codeOrName == null || codeOrName.trim().isEmpty) return null;
    final key = codeOrName.toLowerCase().trim();
    return _nameToIsoCode[key] ?? (key.length == 2 ? key.toUpperCase() : null);
  }

  Map<String, dynamic> toJson() {
    return {'id': id, 'value': value, 'code': code};
  }

  @override
  List<Object?> get props => [id, value, code];
}

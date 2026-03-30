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
  String? get isoCode {
    if (code != null && code!.isNotEmpty) return code!.toUpperCase();
    return _nameToIsoCode[value.toLowerCase().trim()];
  }

  Map<String, dynamic> toJson() {
    return {'id': id, 'value': value, 'code': code};
  }

  @override
  List<Object?> get props => [id, value, code];
}

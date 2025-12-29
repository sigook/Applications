import 'package:dartz/dartz.dart';
import 'package:equatable/equatable.dart';

class ZipCode extends Equatable {
  final String value;
  final String countryCode;
  final String? provinceCode;

  const ZipCode._({
    required this.value,
    required this.countryCode,
    this.provinceCode,
  });

  static const ZipCode emptyUS = ZipCode._(value: '', countryCode: 'US');
  static const ZipCode emptyCA = ZipCode._(value: '', countryCode: 'CA');

  static Either<String, ZipCode> parse({
    required String input,
    required String countryCode,
    String? provinceCode,
  }) {
    final cleaned = input.trim();
    final error = _validate(cleaned, countryCode, provinceCode);
    if (error != null) {
      return Left(error);
    }
    return Right(
      ZipCode._(
        value: _format(cleaned, countryCode),
        countryCode: countryCode.toUpperCase(),
        provinceCode: provinceCode?.toUpperCase(),
      ),
    );
  }

  static String? _validate(
    String input,
    String countryCode,
    String? provinceCode,
  ) {
    final code = countryCode.toUpperCase();
    final cleaned = input.replaceAll(' ', '').toUpperCase();

    if (cleaned.isEmpty) return 'ZIP code is required';

    return switch (code) {
      'US' => _validateUS(cleaned, provinceCode),
      'CA' => _validateCA(cleaned, provinceCode),
      _ => null,
    };
  }

  static String? _validateUS(String cleaned, String? stateCode) {
    final parts = cleaned.split('-');
    if (!RegExp(r'^\d{5}$').hasMatch(parts[0])) {
      return 'US ZIP must be 5 digits (optional -XXXX)';
    }
    if (parts.length > 1 && !RegExp(r'^\d{4}$').hasMatch(parts[1])) {
      return 'ZIP+4 must be 4 digits';
    }

    if (stateCode != null) {
      final zip5 = int.parse(parts[0]);
      final ranges = _usStateToZipRanges[stateCode];
      if (ranges == null || !ranges.any((r) => zip5 >= r.$1 && zip5 <= r.$2)) {
        return 'ZIP does not match selected state';
      }
    }
    return null;
  }

  static String? _validateCA(String cleaned, String? provinceCode) {
    if (!RegExp(r'^[A-Z]\d[A-Z]\d[A-Z]\d$').hasMatch(cleaned)) {
      return 'Canadian postal code must be A1A1A1';
    }
    if (provinceCode != null) {
      final firstLetter = cleaned[0];
      final prefixes = _caProvinceToPrefixes[provinceCode];
      if (prefixes == null || !prefixes.contains(firstLetter)) {
        return 'Postal code does not match province';
      }
    }
    return null;
  }

  static String _format(String input, String countryCode) {
    final code = countryCode.toUpperCase();
    final cleaned = input.replaceAll(' ', '').toUpperCase();

    return switch (code) {
      'US' =>
        cleaned.length > 5
            ? '${cleaned.substring(0, 5)}-${cleaned.substring(5)}'
            : cleaned,
      'CA' =>
        cleaned.length > 3
            ? '${cleaned.substring(0, 3)} ${cleaned.substring(3)}'
            : cleaned,
      _ => cleaned,
    };
  }

  static const Map<String, Set<String>> _caProvinceToPrefixes = {
    'NL': {'A'},
    'NS': {'B'},
    'PE': {'C'},
    'NB': {'E'},
    'QC': {'G', 'H', 'J'},
    'ON': {'K', 'L', 'M', 'N', 'P'},
    'MB': {'R'},
    'SK': {'S'},
    'AB': {'T'},
    'BC': {'V'},
    'NT': {'X'},
    'NU': {'X'},
    'YT': {'Y'},
  };

  static const Map<String, List<(int, int)>> _usStateToZipRanges = {
    'AK': [(99501, 99950)],
    'AL': [(35004, 36925)],
    'AR': [(71601, 72959)],
    'AZ': [(85001, 86556)],
    'CA': [(90001, 96162)],
    'CO': [(80001, 81658)],
    'CT': [(6001, 6928)],
    'DE': [(19701, 19980)],
    'FL': [(32004, 34997)],
    'GA': [(30001, 31999)],
    'HI': [(96701, 96898)],
    'ID': [(83201, 83876)],
    'IL': [(60001, 62999)],
    'IN': [(46001, 47997)],
    'IA': [(50001, 52809)],
    'KS': [(66002, 67954)],
    'KY': [(40003, 42788)],
    'LA': [(70001, 71497)],
    'ME': [(3901, 4992)],
    'MD': [(20331, 21930)],
    'MA': [(1001, 2791)],
    'MI': [(48001, 49971)],
    'MN': [(55001, 56763)],
    'MS': [(38601, 39776)],
    'MO': [(63001, 65899)],
    'MT': [(59001, 59937)],
    'NE': [(68001, 69367)],
    'NV': [(88901, 89883)],
    'NH': [(3031, 3897)],
    'NJ': [(7001, 8989)],
    'NM': [(87001, 88441)],
    'NY': [(10001, 14975)],
    'NC': [(27006, 28909)],
    'ND': [(58001, 58856)],
    'OH': [(43001, 45999)],
    'OK': [(73001, 74966)],
    'OR': [(97001, 97920)],
    'PA': [(15001, 19640)],
    'RI': [(2801, 2940)],
    'SC': [(29001, 29948)],
    'SD': [(57001, 57799)],
    'TN': [(37010, 38589)],
    'TX': [(75001, 79999)],
    'UT': [(84001, 84784)],
    'VT': [(5001, 5907)],
    'VA': [(20167, 24658)],
    'WA': [(98001, 99403)],
    'WV': [(24701, 26886)],
    'WI': [(53001, 54990)],
    'WY': [(82001, 83128)],
  };

  @override
  List<Object?> get props => [value, countryCode, provinceCode];
}

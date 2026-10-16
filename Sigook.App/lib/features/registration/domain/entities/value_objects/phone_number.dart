import 'package:equatable/equatable.dart';

class PhoneNumber extends Equatable {
  final String value;
  final String? countryCode;
  final String? internationalFormat;
  final String? nationalFormat;
  final String? errorMessage;

  const PhoneNumber({
    required this.value,
    this.countryCode,
    this.internationalFormat,
    this.nationalFormat,
    this.errorMessage,
  });

  factory PhoneNumber.empty() {
    return const PhoneNumber(value: '');
  }

  factory PhoneNumber.invalid(String value, String error) {
    return PhoneNumber(value: value, errorMessage: error);
  }

  factory PhoneNumber.valid({
    required String value,
    required String countryCode,
    required String internationalFormat,
    required String nationalFormat,
  }) {
    return PhoneNumber(
      value: value,
      countryCode: countryCode,
      internationalFormat: internationalFormat,
      nationalFormat: nationalFormat,
    );
  }

  bool get isValid => errorMessage == null && value.isNotEmpty;
  bool get isEmpty => value.isEmpty;

  String get displayFormat {
    if (nationalFormat != null && nationalFormat!.isNotEmpty) {
      return nationalFormat!;
    }
    if (internationalFormat != null && internationalFormat!.isNotEmpty) {
      return internationalFormat!;
    }
    return value;
  }

  String get e164Format {
    return internationalFormat?.replaceAll(RegExp(r'[^\d+]'), '') ?? value;
  }

  @override
  List<Object?> get props => [
    value,
    countryCode,
    internationalFormat,
    nationalFormat,
    errorMessage,
  ];

  @override
  String toString() => displayFormat;
}

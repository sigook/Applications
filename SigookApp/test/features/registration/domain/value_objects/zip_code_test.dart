import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/value_objects/zip_code.dart';

void main() {
  String? parse(String input, String country, [String? province]) =>
      ZipCode.parse(input: input, countryCode: country, provinceCode: province)
          .fold((error) => error, (zip) => zip.value);

  test('US ZIP+4 with dash is formatted with a single dash', () {
    expect(parse('90210-1234', 'US'), '90210-1234');
  });

  test('US ZIP+4 without dash is accepted and formatted', () {
    expect(parse('902101234', 'US'), '90210-1234');
  });

  test('US ZIP must match the selected state', () {
    expect(parse('10001', 'US', 'CA'), 'ZIP does not match selected state');
  });

  test('Canadian postal code is formatted with a space', () {
    expect(parse('m5v3l9', 'CA'), 'M5V 3L9');
  });
}

import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/value_objects/identification_number.dart';

void main() {
  const sin = IdentificationNumber.sinSsnTypeCode;

  test('number is required for any type', () {
    expect(IdentificationNumber.validate('  '), isNotNull);
  });

  test('non-SIN types only require a value', () {
    expect(IdentificationNumber.validate('A1', typeCode: 6), isNull);
  });

  test('SIN must be between 9 and 15 characters', () {
    expect(IdentificationNumber.validate('12345678', typeCode: sin), isNotNull);
    expect(IdentificationNumber.validate('123456789', typeCode: sin), isNull);
    expect(IdentificationNumber.validate('1' * 16, typeCode: sin), isNotNull);
  });
}

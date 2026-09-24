import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/features/profile/domain/validators/profile_validators.dart';

void main() {
  test('first name accepts 1 to 20 characters', () {
    expect(ProfileValidators.firstName('A'), isNull);
    expect(ProfileValidators.firstName('A' * 21), isNotNull);
  });

  test('last name requires at least 2 characters', () {
    expect(ProfileValidators.lastName('B'), isNotNull);
    expect(ProfileValidators.lastName('Bo'), isNull);
  });

  test('address rejects characters the API does not accept', () {
    expect(ProfileValidators.address('123 Main St, Apt #4'), isNull);
    expect(ProfileValidators.address("123 O'Connor St"), isNotNull);
  });

  test('postal code resolves USA code to US rules', () {
    expect(ProfileValidators.postalCode('90210', 'USA'), isNull);
    expect(ProfileValidators.postalCode('M5V 3L9', 'USA'), isNotNull);
    expect(ProfileValidators.postalCode('M5V 3L9', 'CA'), isNull);
  });

  test('SIN is optional but must be 9 to 15 characters', () {
    expect(ProfileValidators.socialInsurance(''), isNull);
    expect(ProfileValidators.socialInsurance('12345678'), isNotNull);
    expect(ProfileValidators.socialInsurance('123456789'), isNull);
  });

  test('SIN due date is required when it expires', () {
    expect(
      ProfileValidators.sinDueDate(
          socialInsurance: '123456789', expires: true, dueDate: null),
      isNotNull,
    );
    expect(
      ProfileValidators.sinDueDate(
          socialInsurance: '123456789', expires: false, dueDate: null),
      isNull,
    );
  });

  test('phone must have 10 digits when present', () {
    expect(ProfileValidators.phone('', 'Phone'), isNull);
    expect(ProfileValidators.phone('', 'Phone', required: true), isNotNull);
    expect(ProfileValidators.phone('416 555-12', 'Phone'), isNotNull);
    expect(ProfileValidators.phone('416 555-1234', 'Phone'), isNull);
  });

  test('email must be between 6 and 100 characters', () {
    expect(ProfileValidators.email('a@b.c'), isNotNull);
    expect(ProfileValidators.email('john@mail.com'), isNull);
  });

  test('end date cannot be before start date', () {
    final start = DateTime(2024, 5, 10);
    expect(
      ProfileValidators.endDate(
          start: start, end: DateTime(2024, 5, 9), isCurrent: false),
      isNotNull,
    );
    expect(
      ProfileValidators.endDate(start: start, end: null, isCurrent: true),
      isNull,
    );
  });

  test('start date cannot be in the future', () {
    expect(
      ProfileValidators.startDate(DateTime.now().add(const Duration(days: 2))),
      isNotNull,
    );
  });
}

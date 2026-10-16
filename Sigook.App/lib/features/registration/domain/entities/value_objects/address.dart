import 'package:equatable/equatable.dart';

class Address extends Equatable {
  final String address;
  final String city;
  final String provinceState;
  final String country;
  final String zipCode;

  const Address({
    required this.address,
    required this.city,
    required this.provinceState,
    required this.country,
    required this.zipCode,
  });

  @override
  List<Object?> get props => [address, city, provinceState, country, zipCode];
}

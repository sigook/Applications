import 'package:equatable/equatable.dart';
import 'city.dart';

/// Location entity that combines city (with province and country) and address
class Location extends Equatable {
  final City city;
  final String address;
  final String postalCode;

  const Location({
    required this.city,
    required this.address,
    required this.postalCode,
  });

  Map<String, dynamic> toJson() {
    return {
      'city': city.toJson(),
      'address': address,
      'postalCode': postalCode,
    };
  }

  @override
  List<Object?> get props => [city, address, postalCode];
}

import 'package:equatable/equatable.dart';
import 'city.dart';

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

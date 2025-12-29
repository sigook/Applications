import 'package:equatable/equatable.dart';

class AddressInfo extends Equatable {
  final String country;
  final String provinceState;
  final String city;
  final String address;
  final String zipCode;

  const AddressInfo({
    required this.country,
    required this.provinceState,
    required this.city,
    required this.address,
    required this.zipCode,
  });

  bool get isValid {
    return country.isNotEmpty &&
        provinceState.isNotEmpty &&
        city.isNotEmpty &&
        address.isNotEmpty &&
        address.length >= 5 &&
        zipCode.isNotEmpty;
  }

  String? get countryError => country.isEmpty ? 'Country is required' : null;
  String? get provinceStateError =>
      provinceState.isEmpty ? 'Province/State is required' : null;
  String? get cityError => city.isEmpty ? 'City is required' : null;
  String? get addressError {
    if (address.isEmpty) return 'Address is required';
    if (address.length < 5) return 'Address must be at least 5 characters';
    return null;
  }

  String? get zipCodeError => zipCode.isEmpty ? 'ZIP Code is required' : null;

  AddressInfo copyWith({
    String? country,
    String? provinceState,
    String? city,
    String? address,
    String? zipCode,
  }) {
    return AddressInfo(
      country: country ?? this.country,
      provinceState: provinceState ?? this.provinceState,
      city: city ?? this.city,
      address: address ?? this.address,
      zipCode: zipCode ?? this.zipCode,
    );
  }

  @override
  List<Object?> get props => [country, provinceState, city, address, zipCode];
}

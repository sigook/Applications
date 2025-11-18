import 'package:dio/dio.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/api_client.dart';
import '../models/catalog_item_model.dart';

/// Remote data source for catalog data
/// Handles all HTTP requests to the catalog API endpoints
abstract class CatalogRemoteDataSource {
  Future<List<CatalogItemModel>> getAvailability();
  Future<List<CatalogItemModel>> getAvailabilityTime();
  Future<List<CatalogItemModel>> getCountries();
  Future<List<CatalogItemModel>> getProvinces(String countryId);
  Future<List<CatalogItemModel>> getCities(String provinceId);
  Future<List<CatalogItemModel>> getGenders();
  Future<List<CatalogItemModel>> getIdentificationTypes();
  Future<List<CatalogItemModel>> getLanguages();
  Future<List<CatalogItemModel>> getSkills();
  Future<List<CatalogItemModel>> getLiftingCapacities();
  Future<List<CatalogItemModel>> getDaysOfWeek();
}

class CatalogRemoteDataSourceImpl implements CatalogRemoteDataSource {
  final ApiClient apiClient;

  CatalogRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<List<CatalogItemModel>> getAvailability() async {
    return _getCatalogItems('/Catalog/availability');
  }

  @override
  Future<List<CatalogItemModel>> getAvailabilityTime() async {
    return _getCatalogItems('/Catalog/availabilityTime');
  }

  @override
  Future<List<CatalogItemModel>> getCountries() async {
    return _getCatalogItems('/Catalog/country');
  }

  @override
  Future<List<CatalogItemModel>> getProvinces(String countryId) async {
    return _getCatalogItems('/Catalog/province/$countryId');
  }

  @override
  Future<List<CatalogItemModel>> getCities(String provinceId) async {
    return _getCatalogItems('/Catalog/city/$provinceId');
  }

  @override
  Future<List<CatalogItemModel>> getGenders() async {
    return _getCatalogItems('/Catalog/gender');
  }

  @override
  Future<List<CatalogItemModel>> getIdentificationTypes() async {
    return _getCatalogItems('/Catalog/identificationType');
  }

  @override
  Future<List<CatalogItemModel>> getLanguages() async {
    return _getCatalogItems('/Catalog/language');
  }

  @override
  Future<List<CatalogItemModel>> getSkills() async {
    return _getCatalogItems('/Catalog/skills');
  }

  @override
  Future<List<CatalogItemModel>> getLiftingCapacities() async {
    return _getCatalogItems('/Catalog/lift');
  }

  @override
  Future<List<CatalogItemModel>> getDaysOfWeek() async {
    return _getCatalogItems('/Catalog/day');
  }

  /// Generic method to fetch catalog items
  Future<List<CatalogItemModel>> _getCatalogItems(String endpoint) async {
    try {
      final response = await apiClient.get(endpoint);

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = response.data as List<dynamic>;

        // Debug: Show first item from API
        if (jsonList.isNotEmpty && endpoint.contains('skill')) {
          print('═══ DEBUG: Skills API Response ═══');
          print('First item: ${jsonList.first}');
          print('═══════════════════════════════');
        }

        // Use safe parsing and filter out invalid items
        final items = jsonList
            .map(
              (json) =>
                  CatalogItemModel.fromJsonSafe(json as Map<String, dynamic>),
            )
            .whereType<CatalogItemModel>() // Filter out nulls
            .toList();

        if (items.isEmpty && jsonList.isNotEmpty) {
          throw ParseException(
            'All catalog items were invalid. '
            'Check API data quality for endpoint: $endpoint',
          );
        }

        return items;
      } else {
        throw ServerException(
          message: 'Failed to load catalog data',
          statusCode: response.statusCode,
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException(
          'Request timeout. Please check your internet connection.',
        );
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException(
          'Cannot connect to server. Please check your internet connection.',
        );
      } else if (e.error?.toString().contains('SocketException') == true) {
        throw NetworkException(
          'Cannot reach server. Please check your network connection or VPN.',
        );
      } else {
        throw ServerException(
          message: e.message ?? 'Server error occurred',
          statusCode: e.response?.statusCode,
        );
      }
    } on NetworkException {
      rethrow; // Re-throw NetworkException as-is
    } on ServerException {
      rethrow; // Re-throw ServerException as-is
    } on ParseException {
      rethrow; // Re-throw ParseException as-is
    } catch (e) {
      // Catch any other errors including SocketException
      if (e.toString().contains('SocketException')) {
        throw NetworkException(
          'Cannot reach server. Please check your network connection or VPN.',
        );
      }
      throw ParseException('Failed to parse catalog data: ${e.toString()}');
    }
  }
}

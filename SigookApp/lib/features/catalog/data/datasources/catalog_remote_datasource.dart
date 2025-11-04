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
  Future<List<CatalogItemModel>> getGenders();
  Future<List<CatalogItemModel>> getIdentificationTypes();
  Future<List<CatalogItemModel>> getLanguages();
  Future<List<CatalogItemModel>> getSkills();
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

  /// Generic method to fetch catalog items
  Future<List<CatalogItemModel>> _getCatalogItems(String endpoint) async {
    try {
      final response = await apiClient.get(endpoint);

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = response.data as List<dynamic>;
        return jsonList
            .map((json) => CatalogItemModel.fromJson(json as Map<String, dynamic>))
            .toList();
      } else {
        throw ServerException(
          message: 'Failed to load catalog data',
          statusCode: response.statusCode,
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException('Request timeout');
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException('No internet connection');
      } else {
        throw ServerException(
          message: e.message ?? 'Server error occurred',
          statusCode: e.response?.statusCode,
        );
      }
    } catch (e) {
      throw ParseException('Failed to parse catalog data: ${e.toString()}');
    }
  }
}

import 'dart:typed_data';
import 'package:dio/dio.dart';
import '../../../../../core/network/api_client.dart';
import '../../../data/datasources/activity_base_datasource.dart';
import '../models/pay_stub_summary_model.dart';

class PayStubListResponse {
  final List<PayStubSummaryModel> items;
  final int pageIndex;
  final int totalPages;
  final int totalItems;

  const PayStubListResponse({
    required this.items,
    required this.pageIndex,
    required this.totalPages,
    required this.totalItems,
  });
}

abstract class PayStubsRemoteDataSource {
  Future<List<int>> getYears();
  Future<PayStubListResponse> getPayStubs({
    required int year,
    required int pageIndex,
    required int pageSize,
  });
  Future<double> getYtdNet(int year);
  Future<Uint8List> getPdf(String payStubId);
}

class PayStubsRemoteDataSourceImpl extends ActivityBaseDatasource
    implements PayStubsRemoteDataSource {
  @override
  final ApiClient apiClient;

  PayStubsRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<List<int>> getYears() => execute(() async {
        final response = await apiClient.dio.get('/worker/paystubs/years');
        return (response.data as List<dynamic>)
            .map((e) => (e as num).toInt())
            .toList();
      });

  @override
  Future<PayStubListResponse> getPayStubs({
    required int year,
    required int pageIndex,
    required int pageSize,
  }) =>
      execute(() async {
        final response = await apiClient.dio.get(
          '/worker/paystubs',
          queryParameters: {
            'year': year,
            'pageIndex': pageIndex,
            'pageSize': pageSize,
          },
        );
        final data = response.data as Map<String, dynamic>;
        return PayStubListResponse(
          items: (data['items'] as List<dynamic>? ?? [])
              .map((e) => PayStubSummaryModel.fromJson(e as Map<String, dynamic>))
              .toList(),
          pageIndex: (data['pageIndex'] as num?)?.toInt() ?? pageIndex,
          totalPages: (data['totalPages'] as num?)?.toInt() ?? 1,
          totalItems: (data['totalItems'] as num?)?.toInt() ?? 0,
        );
      });

  @override
  Future<double> getYtdNet(int year) => execute(() async {
        final response = await apiClient.dio.get('/worker/paystubs/ytd/$year');
        final data = response.data as Map<String, dynamic>;
        return (data['totalPaid'] as num?)?.toDouble() ?? 0;
      });

  @override
  Future<Uint8List> getPdf(String payStubId) => execute(() async {
        final response = await apiClient.dio.get<List<int>>(
          '/worker/paystubs/$payStubId/pdf',
          options: Options(responseType: ResponseType.bytes),
        );
        return Uint8List.fromList(response.data ?? []);
      });
}

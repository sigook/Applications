import 'package:intl/intl.dart';
import '../../../../../core/network/api_client.dart';
import '../../../data/datasources/activity_base_datasource.dart';
import '../models/week_time_sheets_model.dart';

abstract class WeekHoursRemoteDataSource {
  Future<WeekTimeSheetsModel> getWeek(DateTime weekStart);
}

class WeekHoursRemoteDataSourceImpl extends ActivityBaseDatasource
    implements WeekHoursRemoteDataSource {
  @override
  final ApiClient apiClient;

  WeekHoursRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<WeekTimeSheetsModel> getWeek(DateTime weekStart) => execute(() async {
        final response = await apiClient.dio.get(
          '/worker/activity/timesheets',
          queryParameters: {'weekStart': DateFormat('yyyy-MM-dd').format(weekStart)},
        );
        return WeekTimeSheetsModel.fromJson(response.data as Map<String, dynamic>);
      });
}

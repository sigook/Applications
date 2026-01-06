import '../../domain/entities/paginated_timesheet.dart';
import 'timesheet_entry_model.dart';

class PaginatedTimesheetModel extends PaginatedTimesheet {
  const PaginatedTimesheetModel({
    required super.items,
    required super.pageIndex,
    required super.totalPages,
    required super.totalItems,
  });

  factory PaginatedTimesheetModel.fromJson(Map<String, dynamic> json) {
    return PaginatedTimesheetModel(
      items: (json['items'] as List<dynamic>)
          .map(
            (item) =>
                TimesheetEntryModel.fromJson(item as Map<String, dynamic>),
          )
          .toList(),
      pageIndex: json['pageIndex'] as int,
      totalPages: json['totalPages'] as int,
      totalItems: json['totalItems'] as int,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'items': items
          .map((item) => (item as TimesheetEntryModel).toJson())
          .toList(),
      'pageIndex': pageIndex,
      'totalPages': totalPages,
      'totalItems': totalItems,
    };
  }

  PaginatedTimesheet toEntity() {
    return PaginatedTimesheet(
      items: items
          .map((item) => (item as TimesheetEntryModel).toEntity())
          .toList(),
      pageIndex: pageIndex,
      totalPages: totalPages,
      totalItems: totalItems,
    );
  }
}

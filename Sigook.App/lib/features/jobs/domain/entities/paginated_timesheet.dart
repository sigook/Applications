import 'package:equatable/equatable.dart';
import 'timesheet_entry.dart';

class PaginatedTimesheet extends Equatable {
  final List<TimesheetEntry> items;
  final int pageIndex;
  final int totalPages;
  final int totalItems;

  const PaginatedTimesheet({
    required this.items,
    required this.pageIndex,
    required this.totalPages,
    required this.totalItems,
  });

  @override
  List<Object?> get props => [items, pageIndex, totalPages, totalItems];
}

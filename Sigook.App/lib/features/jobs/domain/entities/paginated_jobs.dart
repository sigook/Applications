import 'package:equatable/equatable.dart';
import 'job.dart';

class PaginatedJobs extends Equatable {
  final List<Job> items;
  final int pageIndex;
  final int totalPages;
  final int totalItems;

  const PaginatedJobs({
    required this.items,
    required this.pageIndex,
    required this.totalPages,
    required this.totalItems,
  });

  bool get hasMore => pageIndex < totalPages;

  @override
  List<Object?> get props => [items, pageIndex, totalPages, totalItems];
}

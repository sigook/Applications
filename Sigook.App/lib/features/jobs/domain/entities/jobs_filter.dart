import 'package:equatable/equatable.dart';

enum JobSortBy { newest, oldest, highestRate, lowestRate }

class JobsFilter extends Equatable {
  final JobSortBy sortBy;
  final String search;

  const JobsFilter({this.sortBy = JobSortBy.newest, this.search = ''});

  static const JobsFilter defaults = JobsFilter();

  bool get isDefault => this == defaults;

  bool get hasSearch => search.trim().isNotEmpty;

  JobsFilter copyWith({JobSortBy? sortBy, String? search}) {
    return JobsFilter(
      sortBy: sortBy ?? this.sortBy,
      search: search ?? this.search,
    );
  }

  @override
  List<Object?> get props => [sortBy, search];
}

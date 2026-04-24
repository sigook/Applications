import 'package:equatable/equatable.dart';

class JobExperience extends Equatable {
  final String? id;
  final String company;
  final String? supervisor;
  final String? duties;
  final String? startDate;
  final String? endDate;
  final bool isCurrentJobPosition;

  const JobExperience({
    this.id,
    required this.company,
    this.supervisor,
    this.duties,
    this.startDate,
    this.endDate,
    this.isCurrentJobPosition = false,
  });

  factory JobExperience.fromJson(Map<String, dynamic> json) => JobExperience(
        id: json['id'] as String?,
        company: json['company'] as String? ?? '',
        supervisor: json['supervisor'] as String?,
        duties: json['duties'] as String?,
        startDate: json['startDate'] as String?,
        endDate: json['endDate'] as String?,
        isCurrentJobPosition: json['isCurrentJobPosition'] as bool? ?? false,
      );

  String get formattedStartDate => _formatDate(startDate);
  String get formattedEndDate =>
      isCurrentJobPosition ? 'Present' : _formatDate(endDate);

  static String _formatDate(String? dateStr) {
    if (dateStr == null || dateStr.isEmpty) return 'N/A';
    try {
      final date = DateTime.parse(dateStr);
      const months = [
        'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
        'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec',
      ];
      return '${months[date.month - 1]} ${date.year}';
    } catch (_) {
      return dateStr;
    }
  }

  @override
  List<Object?> get props =>
      [id, company, supervisor, duties, startDate, endDate, isCurrentJobPosition];
}

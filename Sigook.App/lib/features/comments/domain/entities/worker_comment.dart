import 'package:equatable/equatable.dart';

class WorkerComment extends Equatable {
  final String id;
  final String comment;
  final double rate;
  final String? logo;
  final int numberId;
  final DateTime createdAt;

  const WorkerComment({
    required this.id,
    required this.comment,
    required this.rate,
    this.logo,
    required this.numberId,
    required this.createdAt,
  });

  String get formattedDate {
    const months = [
      'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
      'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec',
    ];
    return '${months[createdAt.month - 1]} ${createdAt.day}, ${createdAt.year}';
  }

  @override
  List<Object?> get props => [id, comment, rate, logo, numberId, createdAt];
}

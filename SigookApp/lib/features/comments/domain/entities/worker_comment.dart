import 'package:equatable/equatable.dart';

class WorkerComment extends Equatable {
  final String id;
  final String authorName;
  final String content;
  final String? createdAt;

  const WorkerComment({
    required this.id,
    required this.authorName,
    required this.content,
    this.createdAt,
  });

  @override
  List<Object?> get props => [id, authorName, content, createdAt];
}

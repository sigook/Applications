import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/worker_comment.dart';

part 'worker_comment_model.freezed.dart';
part 'worker_comment_model.g.dart';

@freezed
abstract class WorkerCommentModel with _$WorkerCommentModel {
  const WorkerCommentModel._();

  const factory WorkerCommentModel({
    required String id,
    required String comment,
    required double rate,
    String? logo,
    required int numberId,
    required DateTime createdAt,
  }) = _WorkerCommentModel;

  factory WorkerCommentModel.fromJson(Map<String, dynamic> json) =>
      _$WorkerCommentModelFromJson(json);

  WorkerComment toEntity() => WorkerComment(
        id: id,
        comment: comment,
        rate: rate,
        logo: logo,
        numberId: numberId,
        createdAt: createdAt,
      );
}

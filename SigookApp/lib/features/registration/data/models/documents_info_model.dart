import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/documents_info.dart';

part 'documents_info_model.freezed.dart';
part 'documents_info_model.g.dart';

@freezed
class DocumentsInfoModel with _$DocumentsInfoModel {
  const DocumentsInfoModel._();

  const factory DocumentsInfoModel({
    required List<String> documents,
    String? resume,
  }) = _DocumentsInfoModel;

  /// Convert from domain entity
  factory DocumentsInfoModel.fromEntity(DocumentsInfo entity) {
    return DocumentsInfoModel(
      documents: entity.documents,
      resume: entity.resume,
    );
  }

  /// Convert to domain entity
  DocumentsInfo toEntity() {
    return DocumentsInfo(documents: documents, resume: resume);
  }

  /// From JSON
  factory DocumentsInfoModel.fromJson(Map<String, dynamic> json) =>
      _$DocumentsInfoModelFromJson(json);
}

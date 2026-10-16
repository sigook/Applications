import '../../domain/entities/documents_info.dart';
import '../../domain/entities/uploaded_file.dart';

/// Documents info model for serialization
/// Note: This is a simple data holder, serialization to API format
/// is handled in WorkerRegistrationRequest
class DocumentsInfoModel {
  final IdentificationDocument? identification1;
  final IdentificationDocument? identification2;
  final UploadedFile? resume;

  const DocumentsInfoModel({
    this.identification1,
    this.identification2,
    this.resume,
  });

  /// Convert from domain entity
  factory DocumentsInfoModel.fromEntity(DocumentsInfo entity) {
    return DocumentsInfoModel(
      identification1: entity.identification1,
      identification2: entity.identification2,
      resume: entity.resume,
    );
  }

  /// Convert to domain entity
  DocumentsInfo toEntity() {
    return DocumentsInfo(
      identification1: identification1,
      identification2: identification2,
      resume: resume,
    );
  }

  /// From JSON (simplified for local storage)
  factory DocumentsInfoModel.fromJson(Map<String, dynamic> json) {
    return DocumentsInfoModel(
      identification1: json['identification1'] != null
          ? _identificationDocumentFromJson(
              json['identification1'] as Map<String, dynamic>,
            )
          : null,
      identification2: json['identification2'] != null
          ? _identificationDocumentFromJson(
              json['identification2'] as Map<String, dynamic>,
            )
          : null,
      resume: json['resume'] != null
          ? _uploadedFileFromJson(json['resume'] as Map<String, dynamic>)
          : null,
    );
  }

  /// To JSON (simplified for local storage)
  Map<String, dynamic> toJson() {
    return {
      'identification1': identification1 != null
          ? _identificationDocumentToJson(identification1!)
          : null,
      'identification2': identification2 != null
          ? _identificationDocumentToJson(identification2!)
          : null,
      'resume': resume?.toJson(),
    };
  }

  static IdentificationDocument _identificationDocumentFromJson(
    Map<String, dynamic> json,
  ) {
    return IdentificationDocument(
      identificationTypeId: json['identificationTypeId'] as String,
      identificationTypeValue: json['identificationTypeValue'] as String,
      identificationNumber: json['identificationNumber'] as String,
      file: _uploadedFileFromJson(json['file'] as Map<String, dynamic>),
    );
  }

  static Map<String, dynamic> _identificationDocumentToJson(
    IdentificationDocument doc,
  ) {
    return {
      'identificationTypeId': doc.identificationTypeId,
      'identificationTypeValue': doc.identificationTypeValue,
      'identificationNumber': doc.identificationNumber,
      'file': doc.file.toJson(),
    };
  }

  static UploadedFile _uploadedFileFromJson(Map<String, dynamic> json) {
    return UploadedFile(
      fileName: json['fileName'] as String,
      description: json['description'] as String? ?? '',
      filePath: json['filePath'] as String?,
    );
  }
}

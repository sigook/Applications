import 'package:equatable/equatable.dart';
import 'uploaded_file.dart';

/// Documents information entity
/// Identification is required (at least one), resume is optional.
class DocumentsInfo extends Equatable {
  /// Primary identification document (required)
  final IdentificationDocument? identification1;

  /// Secondary identification document (optional)
  final IdentificationDocument? identification2;

  /// Optional resume file
  final UploadedFile? resume;

  const DocumentsInfo({
    this.identification1,
    this.identification2,
    this.resume,
  });

  /// Valid when at least one identification document is present
  bool get isValid => identification1 != null;

  /// Check if any documents are present
  bool get hasDocuments =>
      identification1 != null || identification2 != null || resume != null;

  /// Get list of all identifications for display
  List<IdentificationDocument> get identifications {
    final list = <IdentificationDocument>[];
    if (identification1 != null) list.add(identification1!);
    if (identification2 != null) list.add(identification2!);
    return list;
  }

  /// Creates a copy with updated fields
  DocumentsInfo copyWith({
    IdentificationDocument? identification1,
    IdentificationDocument? identification2,
    UploadedFile? resume,
    bool clearIdentification2 = false,
    bool clearResume = false,
  }) {
    return DocumentsInfo(
      identification1: identification1 ?? this.identification1,
      identification2: clearIdentification2
          ? null
          : (identification2 ?? this.identification2),
      resume: clearResume ? null : (resume ?? this.resume),
    );
  }

  /// Convert to JSON for debugging/logging purposes
  Map<String, dynamic> toJson() {
    return {
      if (identification1 != null) 'identification1': identification1!.toJson(),
      if (identification2 != null) 'identification2': identification2!.toJson(),
      if (resume != null) 'resume': resume!.toJson(),
    };
  }

  @override
  List<Object?> get props => [identification1, identification2, resume];
}

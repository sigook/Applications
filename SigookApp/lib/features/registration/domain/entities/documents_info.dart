import 'package:equatable/equatable.dart';

/// Documents information entity
/// Identification is required, resume is optional.
class DocumentsInfo extends Equatable {
  /// Required identification documents
  final List<String> documents;

  /// Optional resume file path (single file)
  final String? resume;

  const DocumentsInfo({required this.documents, this.resume});

  /// Valid when at least one identification document is present
  bool get isValid => documents.isNotEmpty;

  /// Creates a copy with updated fields
  DocumentsInfo copyWith({List<String>? documents, String? resume}) {
    return DocumentsInfo(
      documents: documents ?? this.documents,
      resume: resume ?? this.resume,
    );
  }

  @override
  List<Object?> get props => [documents, resume];
}

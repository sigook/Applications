import 'package:equatable/equatable.dart';

/// Documents information entity
/// Handles file uploads for various document types
class DocumentsInfo extends Equatable {
  final List<String> documents; // General documents (file paths)
  final List<String> licenses; // License files (file paths)
  final List<String> certificates; // Certificate files (file paths)
  final String? resume; // Resume file path (single file)

  const DocumentsInfo({
    required this.documents,
    required this.licenses,
    required this.certificates,
    this.resume,
  });

  /// Validates that at least resume is provided
  /// Other documents are optional
  bool get isValid {
    return resume != null && resume!.isNotEmpty;
  }

  /// Validation error messages
  String? get resumeError =>
      (resume == null || resume!.isEmpty) ? 'Resume is required' : null;

  /// Creates a copy with updated fields
  DocumentsInfo copyWith({
    List<String>? documents,
    List<String>? licenses,
    List<String>? certificates,
    String? resume,
  }) {
    return DocumentsInfo(
      documents: documents ?? this.documents,
      licenses: licenses ?? this.licenses,
      certificates: certificates ?? this.certificates,
      resume: resume ?? this.resume,
    );
  }

  @override
  List<Object?> get props => [documents, licenses, certificates, resume];
}

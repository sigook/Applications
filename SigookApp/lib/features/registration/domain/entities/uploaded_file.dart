import 'package:equatable/equatable.dart';

/// Represents an uploaded file with metadata
class UploadedFile extends Equatable {
  final String fileName;
  final String description;
  final String? filePath; // Local path for display/preview

  const UploadedFile({
    required this.fileName,
    this.description = '',
    this.filePath,
  });

  UploadedFile copyWith({
    String? fileName,
    String? description,
    String? filePath,
  }) {
    return UploadedFile(
      fileName: fileName ?? this.fileName,
      description: description ?? this.description,
      filePath: filePath ?? this.filePath,
    );
  }

  Map<String, dynamic> toJson() {
    return {'fileName': fileName, 'description': description};
  }

  @override
  List<Object?> get props => [fileName, description, filePath];
}

/// Represents an identification document with type, number, and file
class IdentificationDocument extends Equatable {
  final String identificationTypeId;
  final String identificationTypeValue;
  final String identificationNumber;
  final UploadedFile file;

  const IdentificationDocument({
    required this.identificationTypeId,
    required this.identificationTypeValue,
    required this.identificationNumber,
    required this.file,
  });

  /// Display string for UI
  String get displayName =>
      '$identificationTypeValue #$identificationNumber - ${file.fileName}';

  IdentificationDocument copyWith({
    String? identificationTypeId,
    String? identificationTypeValue,
    String? identificationNumber,
    UploadedFile? file,
  }) {
    return IdentificationDocument(
      identificationTypeId: identificationTypeId ?? this.identificationTypeId,
      identificationTypeValue:
          identificationTypeValue ?? this.identificationTypeValue,
      identificationNumber: identificationNumber ?? this.identificationNumber,
      file: file ?? this.file,
    );
  }

  /// Convert to JSON for debugging/logging purposes
  Map<String, dynamic> toJson() {
    return {
      'identificationTypeId': identificationTypeId,
      'identificationTypeValue': identificationTypeValue,
      'identificationNumber': identificationNumber,
      'file': file.toJson(),
    };
  }

  @override
  List<Object?> get props => [
    identificationTypeId,
    identificationTypeValue,
    identificationNumber,
    file,
  ];
}

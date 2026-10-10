import 'package:equatable/equatable.dart';

class UploadedFile extends Equatable {
  final String fileName;
  final String description;
  final String? filePath;

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
    return {
      'fileName': fileName,
      'description': description,
      'filePath': filePath,
    };
  }

  @override
  List<Object?> get props => [fileName, description, filePath];
}

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

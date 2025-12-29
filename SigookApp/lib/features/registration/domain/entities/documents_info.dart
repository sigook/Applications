import 'package:equatable/equatable.dart';
import 'uploaded_file.dart';

class DocumentsInfo extends Equatable {
  final IdentificationDocument? identification1;
  final IdentificationDocument? identification2;
  final UploadedFile? resume;

  const DocumentsInfo({
    this.identification1,
    this.identification2,
    this.resume,
  });

  bool get isValid => identification1 != null;

  bool get hasDocuments =>
      identification1 != null || identification2 != null || resume != null;

  List<IdentificationDocument> get identifications {
    final list = <IdentificationDocument>[];
    if (identification1 != null) list.add(identification1!);
    if (identification2 != null) list.add(identification2!);
    return list;
  }

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

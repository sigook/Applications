import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../services/file_picker_service.dart';
import '../services/file_picker_service_impl.dart';

final filePickerServiceProvider = Provider<FilePickerService>((ref) {
  return FilePickerServiceImpl();
});

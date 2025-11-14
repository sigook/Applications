import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../services/file_picker_service.dart';
import '../services/file_picker_service_impl.dart';

/// Provider for FilePickerService
/// Following Dependency Inversion Principle - depends on abstraction not implementation
final filePickerServiceProvider = Provider<FilePickerService>((ref) {
  return FilePickerServiceImpl();
});

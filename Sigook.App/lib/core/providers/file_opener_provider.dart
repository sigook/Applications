import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../services/file_opener_service.dart';
import '../services/file_opener_service_impl.dart';

final fileOpenerServiceProvider = Provider<FileOpenerService>((ref) {
  return FileOpenerServiceImpl();
});

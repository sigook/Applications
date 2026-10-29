import 'package:open_filex/open_filex.dart';
import 'file_opener_service.dart';

class FileOpenerServiceImpl implements FileOpenerService {
  @override
  Future<bool> open(String path) async {
    final result = await OpenFilex.open(path);
    return result.type == ResultType.done;
  }
}

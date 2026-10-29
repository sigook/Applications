import 'dart:io';
import 'dart:typed_data';

import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:path_provider/path_provider.dart';
import 'package:pdfx/pdfx.dart';
import 'package:share_plus/share_plus.dart';
import '../../theme/app_theme.dart';

enum _PreviewKind { pdf, image, other }

class DocumentPreviewPage extends StatefulWidget {
  static const String _shareFolder = 'preview';
  static const Set<String> _imageExtensions = {'jpg', 'jpeg', 'png', 'gif'};

  final String url;
  final String title;

  const DocumentPreviewPage({
    super.key,
    required this.url,
    required this.title,
  });

  static Future<void> open(
    BuildContext context, {
    required String url,
    required String title,
  }) => Navigator.of(context).push(
    MaterialPageRoute<void>(
      builder: (_) => DocumentPreviewPage(url: url, title: title),
    ),
  );

  static Future<void> clearShareCache() async {
    try {
      final directory = Directory(
        '${(await getTemporaryDirectory()).path}/$_shareFolder',
      );
      if (await directory.exists()) await directory.delete(recursive: true);
    } catch (_) {}
  }

  @override
  State<DocumentPreviewPage> createState() => _DocumentPreviewPageState();
}

class _DocumentPreviewPageState extends State<DocumentPreviewPage> {
  final GlobalKey _shareButtonKey = GlobalKey();
  Uint8List? _bytes;
  PdfControllerPinch? _pdfController;
  bool _isLoading = true;
  bool _hasError = false;
  bool _isSharing = false;

  String get _extension {
    final path = Uri.tryParse(widget.url)?.path ?? widget.url;
    final dot = path.lastIndexOf('.');
    return dot < 0 ? '' : path.substring(dot + 1).toLowerCase();
  }

  _PreviewKind get _kind => _extension == 'pdf'
      ? _PreviewKind.pdf
      : DocumentPreviewPage._imageExtensions.contains(_extension)
      ? _PreviewKind.image
      : _PreviewKind.other;

  String get _shareFileName {
    final base = widget.title.replaceAll(RegExp(r'[^A-Za-z0-9 _-]'), '').trim();
    final name = base.isEmpty ? 'Document' : base;
    return _extension.isEmpty ? name : '$name.$_extension';
  }

  @override
  void initState() {
    super.initState();
    _load();
  }

  @override
  void dispose() {
    _pdfController?.dispose();
    super.dispose();
  }

  Future<void> _load() async {
    setState(() {
      _isLoading = true;
      _hasError = false;
    });
    try {
      final response = await Dio().get<List<int>>(
        widget.url,
        options: Options(responseType: ResponseType.bytes),
      );
      final bytes = Uint8List.fromList(response.data ?? const []);
      if (!mounted) return;
      _pdfController?.dispose();
      setState(() {
        _bytes = bytes;
        _pdfController = _kind == _PreviewKind.pdf
            ? PdfControllerPinch(document: PdfDocument.openData(bytes))
            : null;
        _isLoading = false;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() {
        _isLoading = false;
        _hasError = true;
      });
    }
  }

  Future<void> _share() async {
    final bytes = _bytes;
    if (bytes == null || _isSharing) return;
    final box =
        _shareButtonKey.currentContext?.findRenderObject() as RenderBox?;
    final origin = box != null && box.hasSize
        ? box.localToGlobal(Offset.zero) & box.size
        : null;
    setState(() => _isSharing = true);
    try {
      final directory = Directory(
        '${(await getTemporaryDirectory()).path}/${DocumentPreviewPage._shareFolder}',
      );
      await directory.create(recursive: true);
      final file = File('${directory.path}/$_shareFileName');
      await file.writeAsBytes(bytes, flush: true);
      await SharePlus.instance.share(
        ShareParams(files: [XFile(file.path)], sharePositionOrigin: origin),
      );
    } catch (_) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Could not share the document')),
        );
      }
    } finally {
      if (mounted) setState(() => _isSharing = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(widget.title),
        backgroundColor: AppTheme.primaryBlue,
        foregroundColor: Colors.white,
        actions: [
          IconButton(
            key: _shareButtonKey,
            onPressed: _bytes == null || _isSharing ? null : _share,
            icon: const Icon(Icons.share_outlined),
            tooltip: 'Share',
          ),
        ],
      ),
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isLoading) return const Center(child: CircularProgressIndicator());
    if (_hasError) {
      return _Message(
        icon: Icons.error_outline,
        text: 'Could not load the document',
        action: TextButton(onPressed: _load, child: const Text('Retry')),
      );
    }
    return switch (_kind) {
      _PreviewKind.pdf => PdfViewPinch(controller: _pdfController!),
      _PreviewKind.image => InteractiveViewer(
        maxScale: 5,
        child: Center(child: Image.memory(_bytes!)),
      ),
      _PreviewKind.other => const _Message(
        icon: Icons.insert_drive_file_outlined,
        text:
            'Preview not available for this file type.\nUse Share to open it in another app.',
      ),
    };
  }
}

class _Message extends StatelessWidget {
  final IconData icon;
  final String text;
  final Widget? action;

  const _Message({required this.icon, required this.text, this.action});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(icon, size: 48, color: Colors.grey),
            const SizedBox(height: 12),
            Text(
              text,
              textAlign: TextAlign.center,
              style: const TextStyle(fontSize: 14, color: Colors.grey),
            ),
            if (action != null) ...[const SizedBox(height: 8), action!],
          ],
        ),
      ),
    );
  }
}

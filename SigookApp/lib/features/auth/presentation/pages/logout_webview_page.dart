import 'package:flutter/material.dart';
import 'package:webview_flutter/webview_flutter.dart';
import '../../../../core/config/environment.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/navigation/navbar_logo.dart';

class LogoutWebviewPage extends StatefulWidget {
  final String? idToken;

  const LogoutWebviewPage({super.key, this.idToken});

  @override
  State<LogoutWebviewPage> createState() => _LogoutWebviewPageState();
}

class _LogoutWebviewPageState extends State<LogoutWebviewPage> {
  late final WebViewController _controller;
  late final String _identityServerHost;
  bool _isLoading = true;
  bool _popped = false;

  @override
  void initState() {
    super.initState();

    _identityServerHost = Uri.parse(EnvironmentConfig.authority).host;

    final baseUri = Uri.parse(
      EnvironmentConfig.authority,
    ).resolve('connect/endsession');
    final uri = widget.idToken != null
        ? baseUri.replace(queryParameters: {'id_token_hint': widget.idToken!})
        : baseUri;

    _controller = WebViewController()
      ..setJavaScriptMode(JavaScriptMode.unrestricted)
      ..setNavigationDelegate(
        NavigationDelegate(
          onPageStarted: (url) {
            // We always start at the identity server, so any URL that does NOT
            // belong to it means the logout redirect has occurred.
            if (!url.contains(_identityServerHost)) {
              _closeIfNotPopped();
              return;
            }
            if (mounted) setState(() => _isLoading = true);
          },
          onPageFinished: (_) {
            if (mounted) setState(() => _isLoading = false);
          },
          onUrlChange: (UrlChange change) {
            // Secondary detection: catches URL-bar changes that may not
            // trigger onPageStarted (e.g. JS-driven or same-document navigations).
            final url = change.url ?? '';
            if (url.isNotEmpty && !url.contains(_identityServerHost)) {
              _closeIfNotPopped();
            }
          },
        ),
      )
      ..loadRequest(uri);
  }

  void _closeIfNotPopped() {
    if (_popped) return;
    _popped = true;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) Navigator.of(context).pop(true);
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        automaticallyImplyLeading: false,
        backgroundColor: AppTheme.secondaryRed,
        title: const NavbarLogo(),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancel', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
      body: Stack(
        children: [
          WebViewWidget(controller: _controller),
          if (_isLoading)
            const LinearProgressIndicator(
              backgroundColor: Colors.transparent,
              color: AppTheme.primaryBlue,
            ),
        ],
      ),
    );
  }
}

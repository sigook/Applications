import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/routing/app_router.dart';
import '../../domain/entities/auth_token.dart';
import '../viewmodels/auth_viewmodel.dart';

/// Token Info Page
/// Displays token information after successful authentication
class TokenInfoPage extends ConsumerStatefulWidget {
  const TokenInfoPage({super.key});

  @override
  ConsumerState<TokenInfoPage> createState() => _TokenInfoPageState();
}

class _TokenInfoPageState extends ConsumerState<TokenInfoPage> {
  AuthToken? _originalToken;
  AuthToken? _refreshedToken;
  bool _hasRefreshed = false;

  Future<void> _logout() async {
    await ref.read(authViewModelProvider.notifier).logout();
  }

  Future<void> _refreshToken() async {
    final authViewModel = ref.read(authViewModelProvider.notifier);
    await authViewModel.refreshAuthToken();
  }

  @override
  Widget build(BuildContext context) {
    final authState = ref.watch(authViewModelProvider);
    final token = authState.token;

    // Track original token when first loaded
    if (_originalToken == null && token != null) {
      _originalToken = token;
    }

    // Listen to auth state changes
    ref.listen(authViewModelProvider, (previous, next) {
      if (!next.isAuthenticated && next.token == null) {
        // User has logged out successfully
        if (previous?.isAuthenticated == true) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Logged out successfully'),
              backgroundColor: Colors.green,
              duration: Duration(seconds: 2),
            ),
          );
        }
        // Navigate back to welcome
        context.go(AppRoutes.welcome);
      }

      // Show error message if operation failed (but still authenticated)
      if (next.error != null &&
          previous?.isLoading == true &&
          next.isLoading == false &&
          next.isAuthenticated) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(next.error!), backgroundColor: Colors.orange),
        );
      }

      // Check if token was refreshed (different from original)
      if (next.isAuthenticated &&
          next.token != null &&
          _originalToken != null) {
        if (next.token != _originalToken && !_hasRefreshed) {
          setState(() {
            _refreshedToken = next.token;
            _hasRefreshed = true;
          });
        }
      }
    });

    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        backgroundColor: Colors.white,
        elevation: 0,
        title: const Text(
          'Token Information',
          style: TextStyle(color: Colors.black, fontWeight: FontWeight.bold),
        ),
        centerTitle: true,
      ),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: token == null
              ? const Center(
                  child: Text(
                    'No token available',
                    style: TextStyle(fontSize: 18, color: Colors.grey),
                  ),
                )
              : Column(
                  children: [
                    Expanded(
                      child: SingleChildScrollView(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            // Original Token Section
                            _buildSectionHeader('Original Token Information'),
                            const SizedBox(height: 16),
                            if (_originalToken != null) ...[
                              _buildTokenSection(
                                'Access Token',
                                _originalToken!.accessToken ?? 'Not available',
                              ),
                              const SizedBox(height: 16),
                              _buildTokenSection(
                                'ID Token',
                                _originalToken!.idToken ?? 'Not available',
                              ),
                              const SizedBox(height: 16),
                              _buildTokenSection(
                                'Refresh Token',
                                _originalToken!.refreshToken ?? 'Not available',
                              ),
                              const SizedBox(height: 16),
                              _buildTokenSection(
                                'Expiration DateTime',
                                _originalToken!.expirationDateTime
                                        ?.toString() ??
                                    'Not available',
                              ),
                              const SizedBox(height: 16),
                              _buildTokenSection(
                                'Scopes',
                                _originalToken!.scopes?.join(', ') ??
                                    'Not available',
                              ),
                              const SizedBox(height: 16),
                              if (_originalToken!.userInfo != null) ...[
                                const Text(
                                  'User Info:',
                                  style: TextStyle(
                                    fontSize: 16,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                                const SizedBox(height: 8),
                                Text(
                                  _originalToken!.userInfo.toString(),
                                  style: const TextStyle(
                                    fontSize: 14,
                                    color: Colors.black87,
                                  ),
                                ),
                              ],
                            ],

                            // Refreshed Token Section
                            if (_hasRefreshed && _refreshedToken != null) ...[
                              const SizedBox(height: 32),
                              const Divider(thickness: 2, color: Colors.grey),
                              const SizedBox(height: 16),
                              _buildSectionHeader(
                                'Refreshed Token Information',
                              ),
                              const SizedBox(height: 16),
                              _buildTokenSection(
                                'Access Token',
                                _refreshedToken!.accessToken ?? 'Not available',
                                isRefreshed: true,
                              ),
                              const SizedBox(height: 16),
                              _buildTokenSection(
                                'ID Token',
                                _refreshedToken!.idToken ?? 'Not available',
                                isRefreshed: true,
                              ),
                              const SizedBox(height: 16),
                              _buildTokenSection(
                                'Refresh Token',
                                _refreshedToken!.refreshToken ??
                                    'Not available',
                                isRefreshed: true,
                              ),
                              const SizedBox(height: 16),
                              _buildTokenSection(
                                'Expiration DateTime',
                                _refreshedToken!.expirationDateTime
                                        ?.toString() ??
                                    'Not available',
                                isRefreshed: true,
                              ),
                              const SizedBox(height: 16),
                              _buildTokenSection(
                                'Scopes',
                                _refreshedToken!.scopes?.join(', ') ??
                                    'Not available',
                                isRefreshed: true,
                              ),
                              const SizedBox(height: 16),
                              if (_refreshedToken!.userInfo != null) ...[
                                const Text(
                                  'User Info:',
                                  style: TextStyle(
                                    fontSize: 16,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                                const SizedBox(height: 8),
                                Text(
                                  _refreshedToken!.userInfo.toString(),
                                  style: const TextStyle(
                                    fontSize: 14,
                                    color: Colors.black87,
                                  ),
                                ),
                              ],
                            ],
                          ],
                        ),
                      ),
                    ),
                    const SizedBox(height: 24),
                    Row(
                      children: [
                        Expanded(
                          child: SizedBox(
                            height: 56,
                            child: ElevatedButton(
                              onPressed:
                                  (authState.isLoading ||
                                      token.refreshToken == null)
                                  ? null
                                  : _refreshToken,
                              style: ElevatedButton.styleFrom(
                                backgroundColor: Colors.blue,
                                foregroundColor: Colors.white,
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(12),
                                ),
                                elevation: 3,
                              ),
                              child: authState.isLoading
                                  ? const SizedBox(
                                      height: 24,
                                      width: 24,
                                      child: CircularProgressIndicator(
                                        strokeWidth: 2.5,
                                        valueColor:
                                            AlwaysStoppedAnimation<Color>(
                                              Colors.white,
                                            ),
                                      ),
                                    )
                                  : const Text(
                                      'Refresh Token',
                                      style: TextStyle(
                                        fontSize: 16,
                                        fontWeight: FontWeight.bold,
                                        letterSpacing: 0.5,
                                      ),
                                    ),
                            ),
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: SizedBox(
                            height: 56,
                            child: ElevatedButton(
                              onPressed: authState.isLoading ? null : _logout,
                              style: ElevatedButton.styleFrom(
                                backgroundColor: Colors.red,
                                foregroundColor: Colors.white,
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(12),
                                ),
                                elevation: 3,
                              ),
                              child: authState.isLoading
                                  ? const SizedBox(
                                      height: 24,
                                      width: 24,
                                      child: CircularProgressIndicator(
                                        strokeWidth: 2.5,
                                        valueColor:
                                            AlwaysStoppedAnimation<Color>(
                                              Colors.white,
                                            ),
                                      ),
                                    )
                                  : const Text(
                                      'Logout',
                                      style: TextStyle(
                                        fontSize: 16,
                                        fontWeight: FontWeight.bold,
                                        letterSpacing: 0.5,
                                      ),
                                    ),
                            ),
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
        ),
      ),
    );
  }

  Widget _buildTokenSection(
    String title,
    String content, {
    bool isRefreshed = false,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          title,
          style: TextStyle(
            fontSize: 16,
            fontWeight: FontWeight.bold,
            color: isRefreshed ? Colors.blue.shade700 : Colors.black,
          ),
        ),
        const SizedBox(height: 8),
        Container(
          width: double.infinity,
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            color: isRefreshed ? Colors.blue.shade50 : Colors.grey.shade100,
            borderRadius: BorderRadius.circular(8),
            border: isRefreshed
                ? Border.all(color: Colors.blue.shade200, width: 1)
                : null,
          ),
          child: Text(
            content,
            style: TextStyle(fontSize: 14, color: Colors.black87),
          ),
        ),
      ],
    );
  }

  Widget _buildSectionHeader(String title) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 16),
      decoration: BoxDecoration(
        color: Colors.black87,
        borderRadius: BorderRadius.circular(8),
      ),
      child: Text(
        title,
        style: const TextStyle(
          fontSize: 18,
          fontWeight: FontWeight.bold,
          color: Colors.white,
        ),
        textAlign: TextAlign.center,
      ),
    );
  }
}

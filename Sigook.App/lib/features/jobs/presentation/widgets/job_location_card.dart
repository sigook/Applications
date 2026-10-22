import 'package:flutter/material.dart';
import 'package:url_launcher/url_launcher.dart';
import '../../../../core/config/environment.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/utils/map_links.dart';
import '../../domain/entities/job_details.dart';

class JobLocationCard extends StatelessWidget {
  final JobDetails jobDetails;

  const JobLocationCard({super.key, required this.jobDetails});

  @override
  Widget build(BuildContext context) {
    final latitude = jobDetails.latitude!;
    final longitude = jobDetails.longitude!;
    final mapUri = staticMapUri(
      latitude,
      longitude,
      apiKey: EnvironmentConfig.googleMapsApiKey,
    );

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Location',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: AppTheme.textDark,
              ),
            ),
            const SizedBox(height: 16),
            if (mapUri != null) ...[
              ClipRRect(
                borderRadius: BorderRadius.circular(12),
                child: Semantics(
                  button: true,
                  label: 'Open location in Maps',
                  child: InkWell(
                    onTap: () => _openMaps(context, latitude, longitude),
                    child: AspectRatio(
                      aspectRatio: 2,
                      child: Image.network(
                        mapUri.toString(),
                        fit: BoxFit.cover,
                        loadingBuilder: (context, child, progress) =>
                            progress == null
                            ? child
                            : _buildPlaceholder(
                                const CircularProgressIndicator(strokeWidth: 2),
                              ),
                        errorBuilder: (context, error, stackTrace) =>
                            _buildPlaceholder(
                              Icon(
                                Icons.map_outlined,
                                size: 40,
                                color: Colors.grey.shade500,
                              ),
                            ),
                      ),
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 12),
            ],
            if (jobDetails.location != null) ...[
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Icon(
                    Icons.location_on,
                    size: 18,
                    color: Colors.grey.shade600,
                  ),
                  const SizedBox(width: 6),
                  Expanded(
                    child: Text(
                      jobDetails.location!,
                      style: const TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.w600,
                        color: AppTheme.textDark,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
            ],
            SizedBox(
              width: double.infinity,
              child: OutlinedButton.icon(
                onPressed: () => _openMaps(context, latitude, longitude),
                icon: const Icon(Icons.directions_outlined),
                label: const Text('Open in Maps'),
                style: OutlinedButton.styleFrom(
                  foregroundColor: AppTheme.primaryBlue,
                  side: BorderSide(
                    color: AppTheme.primaryBlue.withValues(alpha: 0.5),
                  ),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  padding: const EdgeInsets.symmetric(vertical: 12),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPlaceholder(Widget child) {
    return Container(
      color: Colors.grey.shade200,
      alignment: Alignment.center,
      child: child,
    );
  }

  Future<void> _openMaps(
    BuildContext context,
    double latitude,
    double longitude,
  ) async {
    var opened = false;
    try {
      opened = await launchUrl(
        mapsSearchUri(latitude, longitude),
        mode: LaunchMode.externalApplication,
      );
    } catch (_) {
      opened = false;
    }
    if (opened || !context.mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('Could not open the maps app.'),
        backgroundColor: AppTheme.errorRed,
      ),
    );
  }
}

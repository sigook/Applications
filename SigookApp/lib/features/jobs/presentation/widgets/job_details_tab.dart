import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../domain/entities/job_details.dart';
import '../../domain/usecases/apply_to_job.dart';
import '../providers/jobs_providers.dart';
import 'job_header_card.dart';

class JobDetailsTab extends ConsumerStatefulWidget {
  final JobDetails jobDetails;
  final VoidCallback? onApplySuccess;

  const JobDetailsTab({
    super.key,
    required this.jobDetails,
    this.onApplySuccess,
  });

  @override
  ConsumerState<JobDetailsTab> createState() => _JobDetailsTabState();
}

class _JobDetailsTabState extends ConsumerState<JobDetailsTab> {
  bool _isApplying = false;

  JobDetails get jobDetails => widget.jobDetails;

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        SingleChildScrollView(
          padding: const EdgeInsets.only(bottom: 100),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const SizedBox(height: 12),
              JobHeaderCard(jobDetails: jobDetails),
              const SizedBox(height: 12),
              if (jobDetails.description != null) ...[
                _buildDescriptionCard(),
                const SizedBox(height: 12),
              ],
              _buildInfoCard(),
              const SizedBox(height: 12),
              _buildDatesCard(),
              const SizedBox(height: 12),
              _buildCompensationCard(),
              const SizedBox(height: 12),
              _buildBenefitsCard(),
              const SizedBox(height: 12),
              if (jobDetails.requirements != null) ...[
                _buildRequirementsCard(),
                const SizedBox(height: 12),
              ],
              _buildLocationCard(),
            ],
          ),
        ),
        if (jobDetails.shouldShowApplyButton)
          Positioned(
            bottom: 16,
            left: 16,
            right: 16,
            child: _buildActionButton(context),
          ),
      ],
    );
  }

  Widget _buildInfoCard() {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Job Information',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: AppTheme.textDark,
              ),
            ),
            const SizedBox(height: 16),
            if (jobDetails.jobPosition != null) ...[
              _buildInfoRow(
                Icons.work_outline,
                'Position',
                jobDetails.jobPosition!,
              ),
              const SizedBox(height: 12),
            ],
            _buildInfoRow(
              Icons.people_outline,
              'Workers Needed',
              '${jobDetails.workersQuantity} workers',
            ),
            const SizedBox(height: 12),
            _buildInfoRow(
              Icons.access_time,
              'Duration',
              jobDetails.durationTerm ?? 'N/A',
            ),
            if (jobDetails.requestStatus != null) ...[
              const SizedBox(height: 12),
              _buildInfoRow(
                Icons.pending_actions,
                'Request Status',
                jobDetails.requestStatus!,
              ),
            ],
            if (jobDetails.workerApprovedToWork != null) ...[
              const SizedBox(height: 12),
              _buildInfoRow(
                jobDetails.workerApprovedToWork!
                    ? Icons.check_circle_outline
                    : Icons.cancel_outlined,
                'Approval Status',
                jobDetails.workerApprovedToWork!
                    ? 'Approved to Work'
                    : 'Not Approved',
                valueColor: jobDetails.workerApprovedToWork!
                    ? AppTheme.successGreen
                    : AppTheme.errorRed,
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildCompensationCard() {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Compensation',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: AppTheme.textDark,
              ),
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: Container(
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      color: AppTheme.primaryBlue.withValues(alpha: 0.1),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Column(
                      children: [
                        const Icon(
                          Icons.attach_money,
                          color: AppTheme.primaryBlue,
                          size: 32,
                        ),
                        const SizedBox(height: 8),
                        Text(
                          '\$${jobDetails.workerRate.toStringAsFixed(2)}',
                          style: const TextStyle(
                            fontSize: 24,
                            fontWeight: FontWeight.bold,
                            color: AppTheme.primaryBlue,
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'per hour',
                          style: TextStyle(
                            fontSize: 14,
                            color: Colors.grey.shade600,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
                if (jobDetails.workerSalary != null) ...[
                  const SizedBox(width: 12),
                  Expanded(
                    child: Container(
                      padding: const EdgeInsets.all(16),
                      decoration: BoxDecoration(
                        color: AppTheme.successGreen.withValues(alpha: 0.1),
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Column(
                        children: [
                          const Icon(
                            Icons.account_balance_wallet,
                            color: AppTheme.successGreen,
                            size: 32,
                          ),
                          const SizedBox(height: 8),
                          Text(
                            '\$${jobDetails.workerSalary!.toStringAsFixed(2)}',
                            style: const TextStyle(
                              fontSize: 24,
                              fontWeight: FontWeight.bold,
                              color: AppTheme.successGreen,
                            ),
                          ),
                          const SizedBox(height: 4),
                          Text(
                            'estimated',
                            style: TextStyle(
                              fontSize: 14,
                              color: Colors.grey.shade600,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildLocationCard() {
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
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: AppTheme.primaryBlue.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: const Icon(
                    Icons.location_on,
                    color: AppTheme.primaryBlue,
                    size: 28,
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        jobDetails.location ?? 'N/A',
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w600,
                          color: AppTheme.textDark,
                        ),
                      ),
                      if (jobDetails.entrance != null) ...[
                        const SizedBox(height: 4),
                        Text(
                          'Entrance: ${jobDetails.entrance}',
                          style: TextStyle(
                            fontSize: 14,
                            color: Colors.grey.shade600,
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildDescriptionCard() {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Row(
              children: [
                Icon(Icons.description, color: AppTheme.primaryBlue, size: 22),
                SizedBox(width: 8),
                Text(
                  'Description',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: AppTheme.textDark,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            Text(
              _stripHtmlTags(jobDetails.description!),
              style: TextStyle(
                fontSize: 15,
                color: Colors.grey.shade700,
                height: 1.5,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildRequirementsCard() {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Row(
              children: [
                Icon(Icons.checklist, color: AppTheme.primaryBlue, size: 22),
                SizedBox(width: 8),
                Text(
                  'Requirements',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: AppTheme.textDark,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            Text(
              _stripHtmlTags(jobDetails.requirements!),
              style: TextStyle(
                fontSize: 15,
                color: Colors.grey.shade700,
                height: 1.5,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildDatesCard() {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Schedule',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: AppTheme.textDark,
              ),
            ),
            const SizedBox(height: 16),
            _buildInfoRow(
              Icons.calendar_today,
              'Start Date',
              _formatDate(jobDetails.startAt),
            ),
            if (jobDetails.finishAt != null) ...[
              const SizedBox(height: 12),
              _buildInfoRow(
                Icons.event,
                'End Date',
                _formatDate(jobDetails.finishAt!),
              ),
            ],
            const SizedBox(height: 12),
            _buildInfoRow(
              Icons.add_circle_outline,
              'Created',
              _formatDate(jobDetails.createdAt),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildBenefitsCard() {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Benefits & Perks',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: AppTheme.textDark,
              ),
            ),
            const SizedBox(height: 16),
            _buildInfoRow(
              jobDetails.holidayIsPaid ? Icons.check_circle : Icons.cancel,
              'Paid Holidays',
              jobDetails.holidayIsPaid ? 'Yes' : 'No',
              valueColor: jobDetails.holidayIsPaid
                  ? AppTheme.successGreen
                  : Colors.grey.shade600,
            ),
            const SizedBox(height: 12),
            _buildInfoRow(
              jobDetails.breakIsPaid ? Icons.check_circle : Icons.cancel,
              'Paid Breaks',
              jobDetails.breakIsPaid ? 'Yes' : 'No',
              valueColor: jobDetails.breakIsPaid
                  ? AppTheme.successGreen
                  : Colors.grey.shade600,
            ),
            if (jobDetails.durationBreak != null &&
                jobDetails.durationBreak != '00:00:00') ...[
              const SizedBox(height: 12),
              _buildInfoRow(
                Icons.free_breakfast,
                'Break Duration',
                _formatBreakDuration(jobDetails.durationBreak!),
              ),
            ],
            if (jobDetails.incentive != null) ...[
              const SizedBox(height: 12),
              _buildInfoRow(
                Icons.card_giftcard,
                'Incentive',
                jobDetails.incentive!,
              ),
            ],
            if (jobDetails.incentiveDescription != null) ...[
              const SizedBox(height: 8),
              Padding(
                padding: const EdgeInsets.only(left: 32),
                child: Text(
                  jobDetails.incentiveDescription!,
                  style: TextStyle(
                    fontSize: 14,
                    color: Colors.grey.shade600,
                    fontStyle: FontStyle.italic,
                  ),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildInfoRow(
    IconData icon,
    String label,
    String value, {
    Color? valueColor,
  }) {
    return Row(
      children: [
        Icon(icon, size: 20, color: Colors.grey.shade600),
        const SizedBox(width: 12),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                label,
                style: TextStyle(fontSize: 13, color: Colors.grey.shade600),
              ),
              const SizedBox(height: 2),
              Text(
                value,
                style: TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.w600,
                  color: valueColor ?? AppTheme.textDark,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Future<void> _handleApply() async {
    setState(() {
      _isApplying = true;
    });

    final useCase = ref.read(applyToJobUseCaseProvider);
    final result = await useCase(ApplyToJobParams(jobId: jobDetails.id));

    if (!mounted) return;

    setState(() {
      _isApplying = false;
    });

    result.fold(
      (failure) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(failure.message),
            backgroundColor: AppTheme.errorRed,
            behavior: SnackBarBehavior.floating,
          ),
        );
      },
      (_) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Successfully applied to job!'),
            backgroundColor: AppTheme.successGreen,
            behavior: SnackBarBehavior.floating,
          ),
        );
        widget.onApplySuccess?.call();
      },
    );
  }

  Widget _buildActionButton(BuildContext context) {
    return Container(
      height: 56,
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [AppTheme.primaryBlue, AppTheme.tertiaryBlue],
        ),
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: AppTheme.primaryBlue.withValues(alpha: 0.4),
            blurRadius: 12,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: ElevatedButton(
        onPressed: _isApplying ? null : _handleApply,
        style: ElevatedButton.styleFrom(
          backgroundColor: Colors.transparent,
          shadowColor: Colors.transparent,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(16),
          ),
        ),
        child: _isApplying
            ? const SizedBox(
                height: 24,
                width: 24,
                child: CircularProgressIndicator(
                  strokeWidth: 2.5,
                  valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                ),
              )
            : const Text(
                'Apply Now',
                style: TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                  color: Colors.white,
                  letterSpacing: 0.5,
                ),
              ),
      ),
    );
  }

  String _stripHtmlTags(String html) {
    return html.replaceAll(RegExp(r'<[^>]*>'), '').trim();
  }

  String _formatDate(DateTime date) {
    return '${date.year}-${date.month.toString().padLeft(2, '0')}-${date.day.toString().padLeft(2, '0')} at ${date.hour.toString().padLeft(2, '0')}:${date.minute.toString().padLeft(2, '0')}';
  }

  String _formatBreakDuration(String duration) {
    final parts = duration.split(':');
    if (parts.length != 3) return duration;

    final hours = int.tryParse(parts[0]) ?? 0;
    final minutes = int.tryParse(parts[1]) ?? 0;

    if (hours > 0 && minutes > 0) {
      return '$hours hour${hours > 1 ? 's' : ''} $minutes min${minutes > 1 ? 's' : ''}';
    } else if (hours > 0) {
      return '$hours hour${hours > 1 ? 's' : ''}';
    } else if (minutes > 0) {
      return '$minutes minute${minutes > 1 ? 's' : ''}';
    }
    return 'None';
  }
}

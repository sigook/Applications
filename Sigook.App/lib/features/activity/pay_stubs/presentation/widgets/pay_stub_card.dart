import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../../../../core/theme/app_theme.dart';
import '../../../presentation/widgets/activity_detail_row.dart';
import '../../domain/entities/pay_stub_summary.dart';

class PayStubCard extends StatelessWidget {
  final PayStubSummary payStub;
  final bool isOpening;
  final VoidCallback onOpenPdf;

  const PayStubCard({
    super.key,
    required this.payStub,
    required this.isOpening,
    required this.onOpenPdf,
  });

  @override
  Widget build(BuildContext context) {
    final money = NumberFormat.simpleCurrency(name: 'USD');
    final week = DateFormat('dd-MMM-yyyy').format(payStub.weekEnding);
    final subtitle = [
      payStub.number,
      if (payStub.companies.isNotEmpty) payStub.companies.join(', '),
    ].join(' · ');

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: AppTheme.borderLight),
      ),
      child: Column(
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Week ending $week',
                      style: const TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.w600,
                        color: AppTheme.textDark,
                      ),
                    ),
                    Text(
                      subtitle,
                      style: const TextStyle(fontSize: 13, color: AppTheme.textMedium),
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 8),
              OutlinedButton.icon(
                onPressed: isOpening ? null : onOpenPdf,
                style: OutlinedButton.styleFrom(
                  visualDensity: VisualDensity.compact,
                  padding: const EdgeInsets.symmetric(horizontal: 10),
                ),
                icon: isOpening
                    ? const SizedBox(
                        width: 14,
                        height: 14,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.download, size: 16),
                label: const Text('PDF'),
              ),
            ],
          ),
          const SizedBox(height: 8),
          ActivityDetailRow(label: 'Hours', value: payStub.hours.toStringAsFixed(1)),
          ActivityDetailRow(label: 'Gross', value: money.format(payStub.gross)),
          ActivityDetailRow(label: 'Deductions', value: '−${money.format(payStub.deductions)}'),
          ActivityDetailRow(label: 'Net', value: money.format(payStub.net), isBold: true),
        ],
      ),
    );
  }
}

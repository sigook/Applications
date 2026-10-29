import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import '../../../../../core/theme/app_theme.dart';
import '../../../../../core/widgets/feedback/empty_state_widget.dart';
import '../../../../../core/widgets/feedback/error_state_widget.dart';
import '../viewmodels/pay_stubs_viewmodel.dart';
import '../widgets/pay_stub_card.dart';

class PayStubsTab extends ConsumerStatefulWidget {
  const PayStubsTab({super.key});

  @override
  ConsumerState<PayStubsTab> createState() => _PayStubsTabState();
}

class _PayStubsTabState extends ConsumerState<PayStubsTab>
    with AutomaticKeepAliveClientMixin {
  final ScrollController _scrollController = ScrollController();

  @override
  bool get wantKeepAlive => true;

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) ref.read(payStubsViewModelProvider.notifier).load();
    });
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    if (_scrollController.position.pixels >=
        _scrollController.position.maxScrollExtent * 0.8) {
      ref.read(payStubsViewModelProvider.notifier).loadMore();
    }
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);
    final state = ref.watch(payStubsViewModelProvider);
    final viewModel = ref.read(payStubsViewModelProvider.notifier);

    ref.listen(payStubsViewModelProvider.select((s) => s.pdfError), (_, error) {
      if (error != null) {
        ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(error)));
      }
    });

    if (state.isLoading && state.items.isEmpty) {
      return const Center(
        child: CircularProgressIndicator(color: AppTheme.primaryBlue),
      );
    }
    if (state.error != null && state.items.isEmpty) {
      return ErrorStateWidget(
        title: 'Unable to Load Pay Stubs',
        message: state.error!,
        onRetry: viewModel.load,
      );
    }
    if (state.years.isEmpty) {
      return const EmptyStateWidget(
        icon: Icons.receipt_long_outlined,
        title: 'No pay stubs yet',
        message: 'Your pay stubs will appear here once payroll is processed',
      );
    }

    return Column(
      children: [
        _YtdHeader(
          years: state.years,
          year: state.year,
          ytdNet: state.ytdNet,
          onYearChanged: viewModel.selectYear,
        ),
        Expanded(
          child: RefreshIndicator(
            onRefresh: viewModel.load,
            color: AppTheme.primaryBlue,
            child: ListView.builder(
              controller: _scrollController,
              physics: const AlwaysScrollableScrollPhysics(),
              padding: const EdgeInsets.only(bottom: 24),
              itemCount: state.items.length + (state.isLoadingMore ? 1 : 0),
              itemBuilder: (context, index) {
                if (index == state.items.length) {
                  return const Padding(
                    padding: EdgeInsets.all(16),
                    child: Center(
                      child: CircularProgressIndicator(color: AppTheme.primaryBlue),
                    ),
                  );
                }
                final payStub = state.items[index];
                return PayStubCard(
                  payStub: payStub,
                  isOpening: state.openingId == payStub.id,
                  onOpenPdf: () => viewModel.openPdf(payStub),
                );
              },
            ),
          ),
        ),
      ],
    );
  }
}

class _YtdHeader extends StatelessWidget {
  final List<int> years;
  final int? year;
  final double ytdNet;
  final ValueChanged<int> onYearChanged;

  const _YtdHeader({
    required this.years,
    required this.year,
    required this.ytdNet,
    required this.onYearChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 8, 16, 4),
      child: Row(
        children: [
          Semantics(
            label: 'Year',
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 12),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: AppTheme.borderLight),
              ),
              child: DropdownButton<int>(
                value: year,
                underline: const SizedBox.shrink(),
                items: [
                  for (final option in years)
                    DropdownMenuItem(value: option, child: Text('$option')),
                ],
                onChanged: (value) {
                  if (value != null) onYearChanged(value);
                },
              ),
            ),
          ),
          const SizedBox(width: 12),
          Text.rich(
            TextSpan(
              text: 'YTD net: ',
              style: const TextStyle(fontSize: 13, color: AppTheme.textMedium),
              children: [
                TextSpan(
                  text: NumberFormat.simpleCurrency(name: 'USD').format(ytdNet),
                  style: const TextStyle(fontWeight: FontWeight.w700, color: AppTheme.textDark),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/theme/app_theme.dart';
import '../../../../../core/widgets/feedback/empty_state_widget.dart';
import '../../../../../core/widgets/feedback/error_state_widget.dart';
import '../viewmodels/history_viewmodel.dart';
import '../widgets/assignment_card.dart';

class HistoryTab extends ConsumerStatefulWidget {
  const HistoryTab({super.key});

  @override
  ConsumerState<HistoryTab> createState() => _HistoryTabState();
}

class _HistoryTabState extends ConsumerState<HistoryTab>
    with AutomaticKeepAliveClientMixin {
  final ScrollController _scrollController = ScrollController();

  @override
  bool get wantKeepAlive => true;

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) ref.read(assignmentHistoryViewModelProvider.notifier).load();
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
      ref.read(assignmentHistoryViewModelProvider.notifier).loadMore();
    }
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);
    final state = ref.watch(assignmentHistoryViewModelProvider);
    final viewModel = ref.read(assignmentHistoryViewModelProvider.notifier);

    return Column(
      children: [
        SizedBox(
          height: 56,
          child: ListView(
            scrollDirection: Axis.horizontal,
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
            children: [
              _filterChip('All', state.status == null, () => viewModel.filterBy(null)),
              for (final status in historyStatuses)
                _filterChip(status, state.status == status, () => viewModel.filterBy(status)),
            ],
          ),
        ),
        Expanded(child: _buildList(state, viewModel)),
      ],
    );
  }

  Widget _filterChip(String label, bool selected, VoidCallback onTap) {
    return Padding(
      padding: const EdgeInsets.only(right: 8),
      child: ChoiceChip(
        label: Text(label),
        selected: selected,
        onSelected: (_) => onTap(),
        selectedColor: AppTheme.primaryBlue.withValues(alpha: 0.12),
        labelStyle: TextStyle(
          color: selected ? AppTheme.primaryBlue : AppTheme.textMedium,
          fontWeight: selected ? FontWeight.w600 : FontWeight.w500,
        ),
        showCheckmark: false,
      ),
    );
  }

  Widget _buildList(AssignmentHistoryState state, AssignmentHistoryViewModel viewModel) {
    if (state.isLoading && state.items.isEmpty) {
      return const Center(
        child: CircularProgressIndicator(color: AppTheme.primaryBlue),
      );
    }
    if (state.error != null && state.items.isEmpty) {
      return ErrorStateWidget(
        title: 'Unable to Load History',
        message: state.error!,
        onRetry: viewModel.load,
      );
    }
    if (state.items.isEmpty) {
      return const EmptyStateWidget(
        icon: Icons.history_rounded,
        title: 'No past assignments',
        message: 'Assignments you finish will appear here',
      );
    }
    return RefreshIndicator(
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
          return AssignmentCard(assignment: state.items[index]);
        },
      ),
    );
  }
}

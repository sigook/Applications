import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import '../../../../../core/theme/app_theme.dart';
import '../../../../../core/widgets/feedback/empty_state_widget.dart';
import '../../../../../core/widgets/feedback/error_state_widget.dart';
import '../viewmodels/hours_viewmodel.dart';
import '../widgets/day_time_sheet_row.dart';

class HoursTab extends ConsumerStatefulWidget {
  const HoursTab({super.key});

  @override
  ConsumerState<HoursTab> createState() => _HoursTabState();
}

class _HoursTabState extends ConsumerState<HoursTab> with AutomaticKeepAliveClientMixin {
  @override
  bool get wantKeepAlive => true;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) ref.read(hoursViewModelProvider.notifier).load();
    });
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);
    final state = ref.watch(hoursViewModelProvider);
    final viewModel = ref.read(hoursViewModelProvider.notifier);

    return Column(
      children: [
        _WeekHeader(state: state, onMove: viewModel.moveWeek),
        Expanded(child: _buildBody(state, viewModel)),
      ],
    );
  }

  Widget _buildBody(HoursState state, HoursViewModel viewModel) {
    if (state.isLoading && state.week == null) {
      return const Center(child: CircularProgressIndicator(color: AppTheme.primaryBlue));
    }
    if (state.error != null) {
      return ErrorStateWidget(
        title: 'Unable to Load Timesheets',
        message: state.error!,
        onRetry: viewModel.load,
      );
    }
    final days = state.week?.days ?? [];
    return RefreshIndicator(
      onRefresh: viewModel.load,
      color: AppTheme.primaryBlue,
      child: days.isEmpty
          ? ListView(
              physics: const AlwaysScrollableScrollPhysics(),
              children: const [
                SizedBox(height: 48),
                EmptyStateWidget(
                  icon: Icons.schedule_outlined,
                  title: 'No hours recorded this week',
                  message: 'Your clock-ins will appear here',
                ),
              ],
            )
          : ListView.builder(
              physics: const AlwaysScrollableScrollPhysics(),
              padding: const EdgeInsets.only(bottom: 24),
              itemCount: days.length,
              itemBuilder: (_, index) => DayTimeSheetRow(day: days[index]),
            ),
    );
  }
}

class _WeekHeader extends StatelessWidget {
  final HoursState state;
  final ValueChanged<int> onMove;

  const _WeekHeader({required this.state, required this.onMove});

  @override
  Widget build(BuildContext context) {
    final start = state.weekStart;
    final end = DateTime(start.year, start.month, start.day + 6);
    final total = state.week?.totalHours ?? 0;

    return Padding(
      padding: const EdgeInsets.fromLTRB(8, 8, 16, 4),
      child: Wrap(
        crossAxisAlignment: WrapCrossAlignment.center,
        spacing: 8,
        children: [
          Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              IconButton(
                tooltip: 'Previous week',
                icon: const Icon(Icons.chevron_left),
                onPressed: () => onMove(-1),
              ),
              Text(
                '${DateFormat('MMM d').format(start)} – ${DateFormat('MMM d, yyyy').format(end)}',
                style: const TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.w600,
                  color: AppTheme.textDark,
                ),
              ),
              IconButton(
                tooltip: 'Next week',
                icon: const Icon(Icons.chevron_right),
                onPressed: state.isCurrentWeek ? null : () => onMove(1),
              ),
            ],
          ),
          Text.rich(
            TextSpan(
              text: 'Week total: ',
              style: const TextStyle(fontSize: 13, color: AppTheme.textMedium),
              children: [
                TextSpan(
                  text: '${total.toStringAsFixed(1)} h',
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

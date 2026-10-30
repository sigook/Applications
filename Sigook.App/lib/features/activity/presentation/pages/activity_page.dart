import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/navigation/app_tab_bar.dart';
import '../../../../core/widgets/navigation/navbar_logo.dart';
import '../../../profile/presentation/providers/cached_worker_profile_provider.dart';
import '../../history/presentation/pages/history_tab.dart';
import '../../hours/presentation/pages/hours_tab.dart';
import '../../pay_stubs/presentation/pages/pay_stubs_tab.dart';

class ActivityPage extends ConsumerWidget {
  const ActivityPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final showPayStubs =
        !(ref.watch(cachedWorkerProfileProvider).value?.isUsa ?? false);

    return DefaultTabController(
      key: ValueKey(showPayStubs),
      length: showPayStubs ? 3 : 2,
      child: Scaffold(
        backgroundColor: AppTheme.surfaceGrey,
        appBar: AppBar(
          backgroundColor: AppTheme.secondaryRed,
          foregroundColor: Colors.white,
          iconTheme: const IconThemeData(color: Colors.white),
          elevation: 0,
          automaticallyImplyLeading: false,
          title: const NavbarLogo(),
          bottom: AppTabBar(
            tabs: [
              if (showPayStubs)
                const AppTabItem(icon: Icons.receipt_long, label: 'PAY STUBS'),
              const AppTabItem(icon: Icons.schedule, label: 'TIMESHEETS'),
              const AppTabItem(icon: Icons.history, label: 'HISTORY'),
            ],
          ),
        ),
        body: TabBarView(
          children: [
            if (showPayStubs) const PayStubsTab(),
            const HoursTab(),
            const HistoryTab(),
          ],
        ),
      ),
    );
  }
}

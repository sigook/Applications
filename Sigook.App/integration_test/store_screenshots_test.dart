import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:integration_test/integration_test.dart';
import 'package:sigook_app_flutter/features/jobs/presentation/widgets/job_card.dart';
import 'package:sigook_app_flutter/main_common.dart';

const String _user = String.fromEnvironment('SCREENSHOT_USER');
const String _password = String.fromEnvironment('SCREENSHOT_PASSWORD');

void main() {
  final binding = IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets('capture store screenshots', (tester) async {
    expect(
      _user.isNotEmpty && _password.isNotEmpty,
      isTrue,
      reason: 'Pass --dart-define=SCREENSHOT_USER and SCREENSHOT_PASSWORD',
    );

    final testErrorHandler = FlutterError.onError;
    await mainCommon();
    FlutterError.onError = testErrorHandler;
    await _pump(tester, const Duration(seconds: 2));
    if (Platform.isAndroid) await binding.convertFlutterSurfaceToImage();

    final welcome = find.text('Sign In');
    final jobsPage = find.text('Jobs');
    final filterJobs = find.byTooltip('Filter jobs');
    await _waitFor(tester, _any([welcome, jobsPage]));

    if (welcome.evaluate().isNotEmpty) {
      await _pump(tester, const Duration(milliseconds: 1500));
      await _shoot(binding, tester, '01_welcome');

      await tester.tap(welcome);
      await _waitFor(tester, find.text('Welcome Back'));
      await _pump(tester, const Duration(milliseconds: 600));

      final fields = find.byType(TextFormField);
      await tester.enterText(fields.at(0), _user);
      await tester.enterText(fields.at(1), _password);
      await _pump(tester, const Duration(milliseconds: 300));
      await tester.tap(find.widgetWithText(ElevatedButton, 'Sign In'));
      await _waitFor(tester, jobsPage, timeout: const Duration(seconds: 60));
    }

    final openPunchCard = find.text('OPEN PUNCH CARD');
    await _waitFor(
      tester,
      _any([
        find.byType(JobCard),
        find.text('No jobs available'),
        openPunchCard,
      ]),
      timeout: const Duration(seconds: 60),
    );
    await _pump(tester, const Duration(seconds: 5));
    await _shoot(binding, tester, '02_jobs');

    if (openPunchCard.evaluate().isNotEmpty) {
      await tester.tap(openPunchCard.first);
      await _waitFor(tester, find.text('DETAIL'));
      await _pump(tester, const Duration(seconds: 2));
      await _waitUntilGone(tester, find.text('CHECKING...'));
      await _pump(tester, const Duration(seconds: 2));
      await _shoot(binding, tester, '04_punch_card');

      await tester.tap(find.text('DETAIL'));
      await _pump(tester, const Duration(seconds: 4));
      await _shoot(binding, tester, '03_job_detail');

      await tester.tap(find.byTooltip('Back'));
      await _waitFor(tester, jobsPage);
      await _pump(tester, const Duration(seconds: 1));
    } else if (find.byType(JobCard).evaluate().isNotEmpty) {
      await tester.tap(find.byType(JobCard).first);
      await _waitFor(tester, find.text('DETAIL'));
      await _pump(tester, const Duration(seconds: 4));
      await _shoot(binding, tester, '03_job_detail');

      await tester.tap(find.text('PUNCH CARD'));
      await _pump(tester, const Duration(seconds: 2));
      await _waitUntilGone(tester, find.text('CHECKING...'));
      await _pump(tester, const Duration(seconds: 2));
      await _shoot(binding, tester, '04_punch_card');

      await tester.tap(find.byTooltip('Back'));
      await _waitFor(tester, jobsPage);
      await _pump(tester, const Duration(seconds: 1));
    }

    if (filterJobs.evaluate().isNotEmpty) {
      await tester.tap(filterJobs);
      await _waitFor(tester, find.text('Filter & Sort'));
      await _pump(tester, const Duration(milliseconds: 800));
      await _shoot(binding, tester, '05_filter');
      await tester.tap(find.text('Apply Filters'));
      await _pump(tester, const Duration(seconds: 1));
    }

    await tester.tap(find.text('Activity'));
    await _waitFor(tester, find.text('TIMESHEETS'));
    await _pump(tester, const Duration(seconds: 5));
    await _shoot(binding, tester, '06_activity');

    await tester.tap(find.text('Profile'));
    await _waitFor(tester, find.text('PERSONAL'));
    await _pump(tester, const Duration(seconds: 5));
    await _shoot(binding, tester, '07_profile');
  });
}

Future<void> _shoot(
  IntegrationTestWidgetsFlutterBinding binding,
  WidgetTester tester,
  String name,
) async {
  await _pump(tester, const Duration(milliseconds: 500));
  debugPrint('[store_screenshots] capturing $name');
  await binding.takeScreenshot(name);
  debugPrint('[store_screenshots] captured $name');
}

Future<void> _pump(WidgetTester tester, Duration duration) async {
  final end = DateTime.now().add(duration);
  while (DateTime.now().isBefore(end)) {
    await tester.pump(const Duration(milliseconds: 100));
  }
}

Future<void> _waitFor(
  WidgetTester tester,
  Finder finder, {
  Duration timeout = const Duration(seconds: 30),
}) async {
  debugPrint(
    '[store_screenshots] waiting for ${finder.describeMatch(Plurality.one)}',
  );
  final end = DateTime.now().add(timeout);
  while (DateTime.now().isBefore(end)) {
    await tester.pump(const Duration(milliseconds: 200));
    if (finder.evaluate().isNotEmpty) return;
  }
  debugPrint(
    '[store_screenshots] timed out waiting for ${finder.describeMatch(Plurality.one)}',
  );
  throw TestFailure(
    'Timed out after $timeout waiting for ${finder.describeMatch(Plurality.one)}',
  );
}

Future<void> _waitUntilGone(
  WidgetTester tester,
  Finder finder, {
  Duration timeout = const Duration(seconds: 30),
}) async {
  debugPrint(
    '[store_screenshots] waiting until gone ${finder.describeMatch(Plurality.one)}',
  );
  final end = DateTime.now().add(timeout);
  while (DateTime.now().isBefore(end)) {
    await tester.pump(const Duration(milliseconds: 200));
    if (finder.evaluate().isEmpty) return;
  }
  debugPrint(
    '[store_screenshots] still present after $timeout: ${finder.describeMatch(Plurality.one)}',
  );
}

Finder _any(List<Finder> finders) => find.byWidgetPredicate(
  (widget) => finders.any(
    (finder) => finder.evaluate().any((element) => element.widget == widget),
  ),
  description: finders
      .map((finder) => finder.describeMatch(Plurality.one))
      .join(' or '),
);

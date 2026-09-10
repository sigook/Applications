import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/core/routing/app_router.dart';

void main() {
  group('AppRouter.requiresSession', () {
    test('protects the authenticated areas', () {
      for (final location in [
        AppRoutes.jobs,
        AppRoutes.jobDetails,
        AppRoutes.profile,
        AppRoutes.history,
      ]) {
        expect(AppRouter.requiresSession(location), true, reason: location);
      }
    });

    test('leaves the public flows open', () {
      for (final location in [
        AppRoutes.splash,
        AppRoutes.welcome,
        AppRoutes.signIn,
        AppRoutes.forgotPassword,
        AppRoutes.registration,
        AppRoutes.registrationConfirmation,
        AppRoutes.about,
        AppRoutes.privacyPolicy,
        AppRoutes.terms,
      ]) {
        expect(AppRouter.requiresSession(location), false, reason: location);
      }
    });
  });

  group('RouterRefreshNotifier', () {
    test('refresh notifies listeners', () {
      final notifier = RouterRefreshNotifier();
      var notified = 0;
      notifier.addListener(() => notified++);

      notifier.refresh();
      notifier.refresh();

      expect(notified, 2);
      notifier.dispose();
    });
  });
}

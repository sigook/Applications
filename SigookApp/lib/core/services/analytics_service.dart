abstract class AnalyticsService {
  Future<void> logEvent({
    required String name,
    Map<String, dynamic>? parameters,
  });

  Future<void> logScreenView({required String screenName, String? screenClass});

  Future<void> setUserId(String? userId);

  Future<void> setUserProperty({required String name, required String value});

  Future<void> logLogin({String? method});

  Future<void> logSignUp({String? method});

  Future<void> logSearch({String? searchTerm});

  Future<void> logShare({required String contentType, required String itemId});

  Future<void> logPurchase({
    required String currency,
    required double value,
    Map<String, dynamic>? parameters,
  });
}

class AnalyticsServiceImpl implements AnalyticsService {
  @override
  Future<void> logEvent({
    required String name,
    Map<String, dynamic>? parameters,
  }) async {
    // TODO: Implement Firebase Analytics
    // await FirebaseAnalytics.instance.logEvent(
    //   name: name,
    //   parameters: parameters,
    // );

    // For now, just log to console in debug mode
    if (const bool.fromEnvironment('dart.vm.product') == false) {
      print('Analytics Event: $name');
      if (parameters != null) {
        print('Parameters: $parameters');
      }
    }
  }

  @override
  Future<void> logScreenView({
    required String screenName,
    String? screenClass,
  }) async {
    await logEvent(
      name: 'screen_view',
      parameters: {
        'screen_name': screenName,
        if (screenClass != null) 'screen_class': screenClass,
      },
    );
  }

  @override
  Future<void> setUserId(String? userId) async {
    // TODO: Implement Firebase Analytics
    // await FirebaseAnalytics.instance.setUserId(id: userId);
    if (const bool.fromEnvironment('dart.vm.product') == false) {
      print('Analytics: Set User ID: $userId');
    }
  }

  @override
  Future<void> setUserProperty({
    required String name,
    required String value,
  }) async {
    // TODO: Implement Firebase Analytics
    // await FirebaseAnalytics.instance.setUserProperty(
    //   name: name,
    //   value: value,
    // );
    if (const bool.fromEnvironment('dart.vm.product') == false) {
      print('Analytics: Set User Property: $name = $value');
    }
  }

  @override
  Future<void> logLogin({String? method}) async {
    await logEvent(
      name: 'login',
      parameters: {if (method != null) 'method': method},
    );
  }

  @override
  Future<void> logSignUp({String? method}) async {
    await logEvent(
      name: 'sign_up',
      parameters: {if (method != null) 'method': method},
    );
  }

  @override
  Future<void> logSearch({String? searchTerm}) async {
    await logEvent(
      name: 'search',
      parameters: {if (searchTerm != null) 'search_term': searchTerm},
    );
  }

  @override
  Future<void> logShare({
    required String contentType,
    required String itemId,
  }) async {
    await logEvent(
      name: 'share',
      parameters: {'content_type': contentType, 'item_id': itemId},
    );
  }

  @override
  Future<void> logPurchase({
    required String currency,
    required double value,
    Map<String, dynamic>? parameters,
  }) async {
    final params = {'currency': currency, 'value': value, ...?parameters};

    await logEvent(name: 'purchase', parameters: params);
  }
}

class JobAnalyticsEvents {
  static Future<void> logJobViewed(
    AnalyticsService analytics, {
    required String jobId,
    required String jobTitle,
  }) async {
    await analytics.logEvent(
      name: 'job_viewed',
      parameters: {'job_id': jobId, 'job_title': jobTitle},
    );
  }

  static Future<void> logJobApplied(
    AnalyticsService analytics, {
    required String jobId,
    required String jobTitle,
  }) async {
    await analytics.logEvent(
      name: 'job_applied',
      parameters: {'job_id': jobId, 'job_title': jobTitle},
    );
  }

  static Future<void> logJobSearch(
    AnalyticsService analytics, {
    required String query,
    int? resultsCount,
  }) async {
    await analytics.logEvent(
      name: 'job_search',
      parameters: {
        'search_query': query,
        if (resultsCount != null) 'results_count': resultsCount,
      },
    );
  }
}

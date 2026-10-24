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
      parameters: {'search_query': query, 'results_count': ?resultsCount},
    );
  }
}

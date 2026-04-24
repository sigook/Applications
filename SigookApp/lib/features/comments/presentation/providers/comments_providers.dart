import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/providers/core_providers.dart';
import '../../../auth/presentation/providers/auth_providers.dart';
import '../../../profile/presentation/providers/cached_worker_profile_provider.dart';
import '../../data/datasources/comments_remote_datasource.dart';
import '../../data/repositories/comments_repository_impl.dart';
import '../../domain/entities/worker_comment.dart';
import '../../domain/repositories/comments_repository.dart';
import '../../domain/usecases/get_worker_comments.dart';

final commentsDatasourceProvider = Provider<CommentsRemoteDataSource>((ref) {
  return CommentsRemoteDataSourceImpl(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final commentsRepositoryProvider = Provider<CommentsRepository>((ref) {
  return CommentsRepositoryImpl(
    datasource: ref.read(commentsDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final getWorkerCommentsUseCaseProvider = Provider<GetWorkerComments>((ref) {
  return GetWorkerComments(ref.read(commentsRepositoryProvider));
});

final commentsListProvider = FutureProvider<List<WorkerComment>>((ref) async {
  final profile = await ref.watch(cachedWorkerProfileProvider.future);
  if (profile == null) return [];

  final useCase = ref.read(getWorkerCommentsUseCaseProvider);
  final result = await useCase(profile.id);
  return result.fold(
    (failure) => throw Exception(failure.message),
    (comments) => comments,
  );
});

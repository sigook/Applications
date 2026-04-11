import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../presentation/providers/cached_worker_profile_provider.dart';
import '../providers/sin_providers.dart';

part 'sin_viewmodel.freezed.dart';
part 'sin_viewmodel.g.dart';

@freezed
abstract class SinState with _$SinState {
  const factory SinState({
    @Default(false) bool isEditing,
    @Default(false) bool isSaving,
    String? saveError,
    @Default(false) bool justSaved,
  }) = _SinState;
}

@riverpod
class SinViewModel extends _$SinViewModel {
  @override
  SinState build() => const SinState();

  void startEditing() => state = state.copyWith(
        isEditing: true,
        saveError: null,
        justSaved: false,
      );

  void cancelEditing() => state = state.copyWith(isEditing: false);

  Future<void> save(Map<String, String> fields, {String? filePath}) async {
    state = state.copyWith(isSaving: true, saveError: null, justSaved: false);

    final result = await ref.read(updateSinUseCaseProvider)(
      fields,
      filePath: filePath,
    );

    result.fold(
      (failure) => state = state.copyWith(
        isSaving: false,
        saveError: 'Failed to save: ${failure.message}',
      ),
      (_) {
        state = state.copyWith(
          isSaving: false,
          isEditing: false,
          justSaved: true,
        );
        ref.invalidate(cachedWorkerProfileProvider);
      },
    );
  }
}

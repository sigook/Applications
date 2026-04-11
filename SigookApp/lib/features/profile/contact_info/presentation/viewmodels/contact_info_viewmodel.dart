import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../presentation/providers/cached_worker_profile_provider.dart';
import '../providers/contact_info_providers.dart';

part 'contact_info_viewmodel.freezed.dart';
part 'contact_info_viewmodel.g.dart';

@freezed
abstract class ContactInfoState with _$ContactInfoState {
  const factory ContactInfoState({
    @Default(false) bool isEditing,
    @Default(false) bool isSaving,
    String? saveError,
    @Default(false) bool justSaved,
  }) = _ContactInfoState;
}

@riverpod
class ContactInfoViewModel extends _$ContactInfoViewModel {
  @override
  ContactInfoState build() => const ContactInfoState();

  void startEditing() => state = state.copyWith(
        isEditing: true,
        saveError: null,
        justSaved: false,
      );

  void cancelEditing() => state = state.copyWith(isEditing: false);

  Future<void> save(Map<String, String> fields) async {
    state = state.copyWith(isSaving: true, saveError: null, justSaved: false);

    final result = await ref.read(updateContactInfoUseCaseProvider)(fields);

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

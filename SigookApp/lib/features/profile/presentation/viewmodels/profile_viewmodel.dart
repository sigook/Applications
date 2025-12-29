import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/entities/worker_profile.dart';
import '../providers/cached_worker_profile_provider.dart';

part 'profile_viewmodel.freezed.dart';
part 'profile_viewmodel.g.dart';

@freezed
abstract class ProfileState with _$ProfileState {
  const factory ProfileState({
    @Default(false) bool isLoading,
    @Default(false) bool isEditing,
    String? error,
    WorkerProfile? profile,
  }) = _ProfileState;
}

@riverpod
class ProfileViewModel extends _$ProfileViewModel {
  @override
  ProfileState build() {
    _loadProfile();
    return const ProfileState();
  }

  Future<void> _loadProfile() async {
    state = state.copyWith(isLoading: true, error: null);

    final profileAsync = await ref.read(cachedWorkerProfileProvider.future);

    state = state.copyWith(isLoading: false, profile: profileAsync);
  }

  void toggleEditing() {
    state = state.copyWith(isEditing: !state.isEditing);
  }

  Future<void> refresh() async {
    await _loadProfile();
  }

  Future<void> updateProfile(WorkerProfile updatedProfile) async {
    state = state.copyWith(isLoading: true, error: null);

    state = state.copyWith(
      isLoading: false,
      profile: updatedProfile,
      isEditing: false,
    );
  }
}

import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../../core/constants/error_messages.dart';
import '../../../../../core/providers/file_opener_provider.dart';
import '../../domain/entities/pay_stub_summary.dart';
import '../providers/pay_stubs_providers.dart';

part 'pay_stubs_viewmodel.freezed.dart';
part 'pay_stubs_viewmodel.g.dart';

@freezed
abstract class PayStubsState with _$PayStubsState {
  const factory PayStubsState({
    @Default([]) List<int> years,
    int? year,
    @Default([]) List<PayStubSummary> items,
    @Default(0) int totalCount,
    @Default(0) double ytdNet,
    @Default(false) bool isLoading,
    @Default(false) bool isLoadingMore,
    @Default(1) int currentPage,
    @Default(false) bool hasMore,
    String? error,
    String? openingId,
    String? pdfError,
  }) = _PayStubsState;
}

@riverpod
class PayStubsViewModel extends _$PayStubsViewModel {
  static const _pageSize = 30;

  @override
  PayStubsState build() => const PayStubsState();

  Future<void> load() async {
    state = state.copyWith(isLoading: true, error: null);
    final years = await ref.read(getPayStubYearsUseCaseProvider)();
    if (!ref.mounted) return;
    await years.fold(
      (failure) async => state = state.copyWith(
        isLoading: false,
        error: ErrorMessages.fromException(Exception(failure.message)),
      ),
      (list) async {
        final year = state.year != null && list.contains(state.year) ? state.year : list.firstOrNull;
        state = state.copyWith(years: list, year: year);
        if (year == null) {
          state = state.copyWith(isLoading: false, items: [], totalCount: 0, ytdNet: 0);
          return;
        }
        await _loadYear(year);
      },
    );
  }

  Future<void> selectYear(int year) async {
    if (year == state.year) return;
    state = state.copyWith(year: year, isLoading: true, error: null);
    await _loadYear(year);
  }

  Future<void> loadMore() async {
    final year = state.year;
    if (year == null || state.isLoading || state.isLoadingMore || !state.hasMore) return;
    state = state.copyWith(isLoadingMore: true);
    final result = await ref.read(getPayStubsUseCaseProvider)(
      year: year,
      pageIndex: state.currentPage + 1,
      pageSize: _pageSize,
    );
    if (!ref.mounted) return;
    result.fold(
      (failure) => state = state.copyWith(isLoadingMore: false, hasMore: false),
      (page) => state = state.copyWith(
        isLoadingMore: false,
        items: [...state.items, ...page.items],
        currentPage: page.pageIndex,
        hasMore: page.hasMore,
      ),
    );
  }

  Future<void> openPdf(PayStubSummary payStub) async {
    if (state.openingId != null) return;
    state = state.copyWith(openingId: payStub.id, pdfError: null);
    final result = await ref.read(downloadPayStubPdfUseCaseProvider)(payStub);
    if (!ref.mounted) return;
    await result.fold(
      (failure) async => state = state.copyWith(
        openingId: null,
        pdfError: ErrorMessages.fromException(Exception(failure.message)),
      ),
      (path) async {
        final opened = await ref.read(fileOpenerServiceProvider).open(path);
        if (!ref.mounted) return;
        state = state.copyWith(
          openingId: null,
          pdfError: opened ? null : 'No app found to open PDF files',
        );
      },
    );
  }

  Future<void> _loadYear(int year) async {
    final pageFuture = ref.read(getPayStubsUseCaseProvider)(year: year, pageSize: _pageSize);
    final ytdFuture = ref.read(getPayStubYtdNetUseCaseProvider)(year);
    final pageResult = await pageFuture;
    final ytdResult = await ytdFuture;
    if (!ref.mounted || state.year != year) return;
    final ytdNet = ytdResult.getOrElse(() => 0);
    pageResult.fold(
      (failure) => state = state.copyWith(
        isLoading: false,
        items: [],
        error: ErrorMessages.fromException(Exception(failure.message)),
      ),
      (page) => state = state.copyWith(
        isLoading: false,
        items: page.items,
        totalCount: page.totalCount,
        currentPage: page.pageIndex,
        hasMore: page.hasMore,
        ytdNet: ytdNet,
      ),
    );
  }
}

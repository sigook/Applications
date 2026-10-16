import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/core/usecases/usecase.dart';
import 'package:sigook_app_flutter/features/auth/domain/usecases/logout.dart';

import '../../../../helpers/mocks.dart';

void main() {
  late MockAuthRepository mockRepo;
  late Logout usecase;

  setUp(() {
    mockRepo = MockAuthRepository();
    usecase = Logout(mockRepo);
  });

  test('returns void when repository succeeds', () async {
    when(() => mockRepo.logout())
        .thenAnswer((_) async => const Right(null));

    final result = await usecase(NoParams());

    expect(result.isRight(), true);
    verify(() => mockRepo.logout()).called(1);
  });

  test('returns ServerFailure when repository fails', () async {
    const failure = ServerFailure(message: 'logout error');
    when(() => mockRepo.logout())
        .thenAnswer((_) async => const Left(failure));

    final result = await usecase(NoParams());

    expect(result, const Left(failure));
  });
}

import 'dart:convert';

import 'package:dartz/dartz.dart';
import 'package:flutter/foundation.dart';
import 'package:json_annotation/json_annotation.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/error/failures.dart';
import '../../domain/entities/registration_form.dart';
import '../../domain/repositories/registration_repository.dart';
import '../datasources/registration_local_datasource.dart';
import '../datasources/registration_remote_datasource.dart';
import '../models/registration_form_model.dart';
import '../models/worker_registration_request.dart';

class RegistrationRepositoryImpl implements RegistrationRepository {
  final RegistrationLocalDataSource localDataSource;
  final RegistrationRemoteDataSource remoteDataSource;

  RegistrationRepositoryImpl({
    required this.localDataSource,
    required this.remoteDataSource,
  });

  @override
  Future<Either<Failure, void>> submitRegistration(
    RegistrationForm form,
  ) async {
    try {
      debugPrint('╔═══ WORKER REGISTRATION REQUEST ═══');
      debugPrint('║ Worker Profile Data (JSON):');
      const encoder = JsonEncoder.withIndent('  ');
      final prettyJson = encoder.convert(form.toJson());
      debugPrint(prettyJson);
      debugPrint('╚═══════════════════════════════════');
      final request = WorkerRegistrationRequest.fromEntity(form);

      await remoteDataSource.registerWorker(request);

      await localDataSource.clearDraft();

      return const Right(null);
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } catch (e) {
      return Left(
        ServerFailure(
          message: 'Failed to submit registration: ${e.toString()}',
        ),
      );
    }
  }

  @override
  Future<Either<Failure, void>> saveDraft(RegistrationForm form) async {
    try {
      final model = RegistrationFormModel.fromEntity(form);
      await localDataSource.saveDraft(model);
      return const Right(null);
    } catch (e) {
      return Left(CacheFailure(message: e.toString()));
    }
  }

  @override
  Future<Either<Failure, RegistrationForm>> getDraft() async {
    try {
      final model = await localDataSource.getDraft();
      if (model == null) {
        return Right(RegistrationForm.empty());
      }
      return Right(model.toEntity());
    } catch (e) {
      return Left(CacheFailure(message: e.toString()));
    }
  }

  @override
  Future<Either<Failure, void>> clearDraft() async {
    try {
      await localDataSource.clearDraft();
      return const Right(null);
    } catch (e) {
      return Left(CacheFailure(message: e.toString()));
    }
  }
}

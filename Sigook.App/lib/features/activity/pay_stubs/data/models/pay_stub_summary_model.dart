import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/pay_stub_summary.dart';

part 'pay_stub_summary_model.freezed.dart';
part 'pay_stub_summary_model.g.dart';

@freezed
abstract class PayStubSummaryModel with _$PayStubSummaryModel {
  const PayStubSummaryModel._();

  const factory PayStubSummaryModel({
    required String id,
    String? payStubNumber,
    required DateTime weekEnding,
    required DateTime paymentDate,
    @Default(0) double hours,
    @Default(0) double totalEarnings,
    @Default(0) double totalDeductions,
    @Default(0) double totalPaid,
    @Default([]) List<String> companies,
  }) = _PayStubSummaryModel;

  factory PayStubSummaryModel.fromJson(Map<String, dynamic> json) =>
      _$PayStubSummaryModelFromJson(json);

  PayStubSummary toEntity() => PayStubSummary(
        id: id,
        number: payStubNumber ?? '',
        weekEnding: weekEnding,
        paymentDate: paymentDate,
        hours: hours,
        gross: totalEarnings,
        deductions: totalDeductions,
        net: totalPaid,
        companies: companies,
      );
}

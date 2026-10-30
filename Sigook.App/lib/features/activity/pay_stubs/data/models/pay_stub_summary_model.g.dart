// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'pay_stub_summary_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_PayStubSummaryModel _$PayStubSummaryModelFromJson(Map<String, dynamic> json) =>
    _PayStubSummaryModel(
      id: json['id'] as String,
      payStubNumber: json['payStubNumber'] as String?,
      weekEnding: DateTime.parse(json['weekEnding'] as String),
      paymentDate: DateTime.parse(json['paymentDate'] as String),
      hours: (json['hours'] as num?)?.toDouble() ?? 0,
      totalEarnings: (json['totalEarnings'] as num?)?.toDouble() ?? 0,
      totalDeductions: (json['totalDeductions'] as num?)?.toDouble() ?? 0,
      totalPaid: (json['totalPaid'] as num?)?.toDouble() ?? 0,
      companies:
          (json['companies'] as List<dynamic>?)
              ?.map((e) => e as String)
              .toList() ??
          const [],
    );

Map<String, dynamic> _$PayStubSummaryModelToJson(
  _PayStubSummaryModel instance,
) => <String, dynamic>{
  'id': instance.id,
  'payStubNumber': instance.payStubNumber,
  'weekEnding': instance.weekEnding.toIso8601String(),
  'paymentDate': instance.paymentDate.toIso8601String(),
  'hours': instance.hours,
  'totalEarnings': instance.totalEarnings,
  'totalDeductions': instance.totalDeductions,
  'totalPaid': instance.totalPaid,
  'companies': instance.companies,
};

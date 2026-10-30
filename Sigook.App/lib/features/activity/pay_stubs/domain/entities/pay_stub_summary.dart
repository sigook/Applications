import 'package:equatable/equatable.dart';

class PayStubSummary extends Equatable {
  final String id;
  final String number;
  final DateTime weekEnding;
  final DateTime paymentDate;
  final double hours;
  final double gross;
  final double deductions;
  final double net;
  final List<String> companies;

  const PayStubSummary({
    required this.id,
    required this.number,
    required this.weekEnding,
    required this.paymentDate,
    required this.hours,
    required this.gross,
    required this.deductions,
    required this.net,
    required this.companies,
  });

  @override
  List<Object?> get props =>
      [id, number, weekEnding, paymentDate, hours, gross, deductions, net, companies];
}

class PayStubPage extends Equatable {
  final List<PayStubSummary> items;
  final int pageIndex;
  final int totalCount;
  final bool hasMore;

  const PayStubPage({
    required this.items,
    required this.pageIndex,
    required this.totalCount,
    required this.hasMore,
  });

  @override
  List<Object?> get props => [items, pageIndex, totalCount, hasMore];
}

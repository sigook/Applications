import 'package:intl/intl.dart';

extension DateTimeExtensions on DateTime {
  String formatDate() {
    return DateFormat('MM/dd/yyyy').format(this);
  }

  String formatTime() {
    final hour = this.hour == 0
        ? 12
        : (this.hour > 12 ? this.hour - 12 : this.hour);
    final period = this.hour >= 12 ? 'PM' : 'AM';
    final minute = this.minute.toString().padLeft(2, '0');
    return '$hour:$minute $period';
  }

  String formatDateTime() {
    return '${formatDate()} ${formatTime()}';
  }

  String toFormattedDate() {
    return DateFormat('MMM dd, yyyy').format(this);
  }

  String toShortDate() {
    return DateFormat('dd/MM/yyyy').format(this);
  }

  String toFormattedTime() {
    return DateFormat('hh:mm a').format(this);
  }

  String toFormattedDateTime() {
    return DateFormat('MMM dd, yyyy hh:mm a').format(this);
  }

  String toWeekdayDate() {
    return DateFormat('EEEE, MMM dd').format(this);
  }

  String toRelativeString() {
    final now = DateTime.now();
    final difference = now.difference(this);

    if (difference.inSeconds < 60) {
      return 'Just now';
    } else if (difference.inMinutes < 60) {
      return '${difference.inMinutes} minutes ago';
    } else if (difference.inHours < 24) {
      return '${difference.inHours} hours ago';
    } else if (isYesterday) {
      return 'Yesterday';
    } else if (difference.inDays < 30) {
      return '${difference.inDays} days ago';
    } else {
      return toFormattedDate();
    }
  }

  bool get isToday {
    final now = DateTime.now();
    return year == now.year && month == now.month && day == now.day;
  }

  bool get isYesterday {
    final yesterday = DateTime.now().subtract(const Duration(days: 1));
    return year == yesterday.year &&
        month == yesterday.month &&
        day == yesterday.day;
  }

  bool isSameDay(DateTime other) {
    return year == other.year && month == other.month && day == other.day;
  }

  int daysBetween(DateTime other) {
    final from = DateTime(year, month, day);
    final to = DateTime(other.year, other.month, other.day);
    return to.difference(from).inDays;
  }

  DateTime addDays(int days) {
    return add(Duration(days: days));
  }

  bool get isPast {
    return isBefore(DateTime.now());
  }

  bool get isFuture {
    return isAfter(DateTime.now());
  }

  DateTime get startOfDay {
    return DateTime(year, month, day);
  }

  DateTime get endOfDay {
    return DateTime(year, month, day, 23, 59, 59, 999);
  }
}

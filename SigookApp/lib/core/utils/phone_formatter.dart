String formatPhone(String? phone) {
  if (phone == null || phone.isEmpty) return 'N/A';
  final digits = phone.replaceAll(RegExp(r'[^0-9]'), '');
  if (digits.isEmpty) return 'N/A';
  final buffer = StringBuffer();
  for (var i = 0; i < digits.length && i < 10; i++) {
    if (i == 3) buffer.write(' ');
    if (i == 6) buffer.write('-');
    buffer.write(digits[i]);
  }
  return buffer.toString();
}

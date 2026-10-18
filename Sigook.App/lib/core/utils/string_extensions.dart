extension StringExtensions on String {
  bool get isValidEmail {
    final emailRegex = RegExp(
      r'^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$',
    );
    return emailRegex.hasMatch(this);
  }

  bool get isValidPhone {
    final phoneRegex = RegExp(r'^\+?[\d\s-()]+$');
    return phoneRegex.hasMatch(this) && length >= 10;
  }

  bool get isBlank {
    return trim().isEmpty;
  }

  bool get isNotBlank {
    return trim().isNotEmpty;
  }

  String capitalize() {
    if (isEmpty) return this;
    return '${this[0].toUpperCase()}${substring(1)}';
  }

  String toTitleCase() {
    if (isEmpty) return this;
    return split(' ')
        .map((word) {
          if (word.isEmpty) return word;
          return '${word[0].toUpperCase()}${word.substring(1).toLowerCase()}';
        })
        .join(' ');
  }

  String get capitalizeWords {
    if (isEmpty) return this;
    return split(' ').map((word) => word.capitalize()).join(' ');
  }

  String removeWhitespace() {
    return replaceAll(RegExp(r'\s+'), '');
  }

  String truncate(int maxLength, {String ellipsis = '...'}) {
    if (length <= maxLength) return this;
    return '${substring(0, maxLength - ellipsis.length)}$ellipsis';
  }

  bool get isNumeric {
    return RegExp(r'^[0-9]+$').hasMatch(this);
  }

  bool get isAlpha {
    return RegExp(r'^[a-zA-Z]+$').hasMatch(this);
  }

  bool get isAlphanumeric {
    return RegExp(r'^[a-zA-Z0-9]+$').hasMatch(this);
  }

  bool hasMatch(String pattern) {
    return RegExp(pattern).hasMatch(this);
  }

  String splitPascalCase() {
    return replaceAllMapped(
      RegExp(r'(?<=[a-z0-9])(?=[A-Z])'),
      (_) => ' ',
    );
  }

  String htmlToPlainText() {
    var text = replaceAll(RegExp(r'<br\s*/?>', caseSensitive: false), '\n');
    text = text.replaceAll(RegExp(r'<li[^>]*>', caseSensitive: false), '• ');
    text = text.replaceAll(
      RegExp(r'</(p|div|li|h[1-6]|ul|ol|blockquote|tr)>', caseSensitive: false),
      '\n',
    );
    text = text.replaceAll(RegExp(r'<[^>]*>'), '');
    text = text
        .replaceAll('&nbsp;', ' ')
        .replaceAll('&amp;', '&')
        .replaceAll('&lt;', '<')
        .replaceAll('&gt;', '>')
        .replaceAll('&quot;', '"')
        .replaceAll('&#39;', "'");
    text = text.replaceAll(RegExp(r'[ \t]+\n'), '\n');
    text = text.replaceAll(RegExp(r'\n{3,}'), '\n\n');
    return text.trim();
  }

  int? toInt() {
    return int.tryParse(this);
  }

  double? toDouble() {
    return double.tryParse(this);
  }
}

extension NullableStringExtensions on String? {
  bool get isNullOrEmpty {
    return this == null || this!.isEmpty;
  }

  bool get isNullOrBlank {
    return this == null || this!.trim().isEmpty;
  }

  String orDefault(String defaultValue) {
    return this ?? defaultValue;
  }
}

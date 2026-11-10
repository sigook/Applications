import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/core/utils/string_extensions.dart';

void main() {
  group('StringExtensions', () {
    group('splitPascalCase', () {
      test('inserts spaces between words', () {
        expect('ShortTerm'.splitPascalCase(), 'Short Term');
        expect('LongTerm'.splitPascalCase(), 'Long Term');
        expect('Open'.splitPascalCase(), 'Open');
      });
    });

    group('htmlToPlainText', () {
      test('keeps paragraph breaks from editor html', () {
        const html =
            '<p>Intro located in<strong> Rockville, VA</strong></p>'
            '<p><br></p>'
            '<p>Starting day: Monday</p>'
            '<p>Pay Rate: \$24/h</p>';
        expect(
          html.htmlToPlainText(),
          'Intro located in Rockville, VA\n\n'
          'Starting day: Monday\n'
          'Pay Rate: \$24/h',
        );
      });

      test('renders list items with bullets and decodes entities', () {
        const html = '<ul><li>Safety &amp; boots</li><li>OSHA&nbsp;10</li></ul>';
        expect(html.htmlToPlainText(), '• Safety & boots\n• OSHA 10');
      });

      test('returns plain text untouched', () {
        expect('Plain text'.htmlToPlainText(), 'Plain text');
      });
    });
  });
}

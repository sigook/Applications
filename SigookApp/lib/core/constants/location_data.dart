/// Static location data for provinces/states and cities
/// TODO: Replace with API endpoints when available
library;

class LocationData {
  /// Canadian provinces and territories
  static const List<String> canadianProvinces = [
    'Alberta',
    'British Columbia',
    'Manitoba',
    'New Brunswick',
    'Newfoundland and Labrador',
    'Northwest Territories',
    'Nova Scotia',
    'Nunavut',
    'Ontario',
    'Prince Edward Island',
    'Quebec',
    'Saskatchewan',
    'Yukon',
  ];

  /// US states
  static const List<String> usStates = [
    'Alabama',
    'Alaska',
    'Arizona',
    'Arkansas',
    'California',
    'Colorado',
    'Connecticut',
    'Delaware',
    'Florida',
    'Georgia',
    'Hawaii',
    'Idaho',
    'Illinois',
    'Indiana',
    'Iowa',
    'Kansas',
    'Kentucky',
    'Louisiana',
    'Maine',
    'Maryland',
    'Massachusetts',
    'Michigan',
    'Minnesota',
    'Mississippi',
    'Missouri',
    'Montana',
    'Nebraska',
    'Nevada',
    'New Hampshire',
    'New Jersey',
    'New Mexico',
    'New York',
    'North Carolina',
    'North Dakota',
    'Ohio',
    'Oklahoma',
    'Oregon',
    'Pennsylvania',
    'Rhode Island',
    'South Carolina',
    'South Dakota',
    'Tennessee',
    'Texas',
    'Utah',
    'Vermont',
    'Virginia',
    'Washington',
    'West Virginia',
    'Wisconsin',
    'Wyoming',
  ];

  /// Major cities in Canada
  static const List<String> canadianCities = [
    'Toronto',
    'Montreal',
    'Vancouver',
    'Calgary',
    'Edmonton',
    'Ottawa',
    'Winnipeg',
    'Quebec City',
    'Hamilton',
    'Kitchener',
    'London',
    'Victoria',
    'Halifax',
    'Oshawa',
    'Windsor',
    'Saskatoon',
    'Regina',
    'St. Catharines',
    'Barrie',
    'Kelowna',
    'Sherbrooke',
    'Guelph',
    'Abbotsford',
    'Kingston',
    'Trois-Rivières',
    'Moncton',
    'Saint John',
    'Thunder Bay',
    'Peterborough',
    'Brantford',
  ];

  /// Major cities in USA
  static const List<String> usCities = [
    'New York',
    'Los Angeles',
    'Chicago',
    'Houston',
    'Phoenix',
    'Philadelphia',
    'San Antonio',
    'San Diego',
    'Dallas',
    'San Jose',
    'Austin',
    'Jacksonville',
    'Fort Worth',
    'Columbus',
    'Charlotte',
    'San Francisco',
    'Indianapolis',
    'Seattle',
    'Denver',
    'Boston',
    'Washington',
    'Nashville',
    'Detroit',
    'Portland',
    'Las Vegas',
    'Memphis',
    'Louisville',
    'Baltimore',
    'Milwaukee',
    'Albuquerque',
  ];

  /// Get provinces/states based on country
  static List<String> getProvincesForCountry(String country) {
    if (country.toLowerCase().contains('canada')) {
      return canadianProvinces;
    } else if (country.toLowerCase().contains('united states') ||
        country.toLowerCase().contains('usa')) {
      return usStates;
    }
    // Default to generic provinces/states list
    return [...canadianProvinces, ...usStates]..sort();
  }

  /// Get cities based on country
  static List<String> getCitiesForCountry(String country) {
    if (country.toLowerCase().contains('canada')) {
      return canadianCities;
    } else if (country.toLowerCase().contains('united states') ||
        country.toLowerCase().contains('usa')) {
      return usCities;
    }
    // Default to all cities
    return [...canadianCities, ...usCities]..sort();
  }

  /// Combined list for generic use
  static List<String> get allProvincesStates =>
      [...canadianProvinces, ...usStates]..sort();

  static List<String> get allCities =>
      [...canadianCities, ...usCities]..sort();
}

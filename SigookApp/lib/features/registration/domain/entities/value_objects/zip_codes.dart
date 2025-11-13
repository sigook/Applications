const Map<String, Set<String>> caProvinceToPrefixes = {
  'NL': {'A'}, // Newfoundland and Labrador
  'NS': {'B'}, // Nova Scotia
  'PE': {'C'}, // Prince Edward Island
  'NB': {'E'}, // New Brunswick
  'QC': {'G', 'H', 'J'}, // Quebec (multiple regions)
  'ON': {'K', 'L', 'M', 'N', 'P'}, // Ontario (multiple regions)
  'MB': {'R'}, // Manitoba
  'SK': {'S'}, // Saskatchewan
  'AB': {'T'}, // Alberta
  'BC': {'V'}, // British Columbia
  'NT': {'X'}, // Northwest Territories
  'NU': {'X'}, // Nunavut
  'YT': {'Y'}, // Yukon
};

const Map<String, List<(int, int)>> _usStateToZipRanges = {
  'AK': [(99501, 99950)], // Alaska
  'AL': [(35004, 36925)], // Alabama
  'AR': [(71601, 72959), (75502, 75502)], // Arkansas
  'AZ': [(85001, 86556)], // Arizona
  'CA': [(90001, 96162)], // California
  'CO': [(80001, 81658)], // Colorado
  'CT': [(6001, 6389), (6401, 6928)], // Connecticut
  'DC': [
    (20001, 20039),
    (20042, 20599),
    (20799, 20799),
  ], // District of Columbia
  'DE': [(19701, 19980)], // Delaware
  'FL': [(32004, 34997)], // Florida
  'GA': [(30001, 31999), (39901, 39901)], // Georgia
  'HI': [(96701, 96898)], // Hawaii
  'IA': [(50001, 52809), (68119, 68120)], // Iowa
  'ID': [(83201, 83876)], // Idaho
  'IL': [(60001, 62999)], // Illinois
  'IN': [(46001, 47997)], // Indiana
  'KS': [(66002, 67954)], // Kansas
  'KY': [(40003, 42788)], // Kentucky
  'LA': [(70001, 71232), (71234, 71497)], // Louisiana
  'MA': [(1001, 2791), (5501, 5544)], // Massachusetts
  'MD': [(20331, 20331), (20335, 20797), (20812, 21930)], // Maryland
  'ME': [(3901, 4992)], // Maine
  'MI': [(48001, 49971)], // Michigan
  'MN': [(55001, 56763)], // Minnesota
  'MS': [(38601, 39776), (71233, 71233)], // Mississippi
  'MO': [(63001, 65899)], // Missouri
  'MT': [(59001, 59937)], // Montana
  'NC': [(27006, 28909)], // North Carolina
  'ND': [(58001, 58856)], // North Dakota
  'NE': [(68001, 68118), (68122, 69367)], // Nebraska
  'NH': [(3031, 3897)], // New Hampshire
  'NJ': [(7001, 8989)], // New Jersey
  'NM': [(87001, 88441)], // New Mexico
  'NV': [(88901, 89883)], // Nevada
  'NY': [(6390, 6390), (10001, 14975)], // New York
  'OH': [(43001, 45999)], // Ohio
  'OK': [(73001, 73199), (73401, 74966)], // Oklahoma
  'OR': [(97001, 97920)], // Oregon
  'PA': [(15001, 19640)], // Pennsylvania
  'RI': [(2801, 2940)], // Rhode Island
  'SC': [(29001, 29948)], // South Carolina
  'SD': [(57001, 57799)], // South Dakota
  'TN': [(37010, 38589)], // Tennessee
  'TX': [
    (73301, 73301),
    (75001, 75501),
    (75503, 79999),
    (88510, 88589),
  ], // Texas
  'UT': [(84001, 84784)], // Utah
  'VA': [
    (20040, 20041),
    (20040, 20167),
    (20042, 20042),
    (22001, 24658),
  ], // Virginia
  'VT': [(5001, 5495), (5601, 5907)], // Vermont
  'WA': [(98001, 99403)], // Washington
  'WI': [(53001, 54990)], // Wisconsin
  'WV': [(24701, 26886)], // West Virginia
  'WY': [(82001, 83128)], // Wyoming
  // Add territories if needed, e.g., 'PR': [(600, 999)], but omitted for brevity
};

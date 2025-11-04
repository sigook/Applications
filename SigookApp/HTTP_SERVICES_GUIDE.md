# HTTP Services Implementation Guide

## Overview
Complete HTTP services layer implementation using **Dio** HTTP client following Clean Architecture principles.

---

## Architecture Layers

### 📁 Directory Structure
```
lib/
├── core/
│   ├── network/
│   │   ├── api_client.dart           # Dio HTTP client
│   │   └── network_info.dart         # Connectivity checker
│   ├── error/
│   │   ├── exceptions.dart           # Exception classes
│   │   └── failures.dart             # Failure classes
│   └── usecases/
│       └── usecase.dart               # Base use case
│
└── features/
    └── catalog/
        ├── domain/
        │   ├── entities/
        │   │   └── catalog_item.dart           # Catalog entity
        │   ├── repositories/
        │   │   └── catalog_repository.dart     # Repository interface
        │   └── usecases/
        │       └── get_catalog_data.dart       # Use cases
        ├── data/
        │   ├── models/
        │   │   ├── catalog_item_model.dart     # Data model
        │   │   └── catalog_item_model.g.dart   # Generated JSON code
        │   ├── datasources/
        │   │   └── catalog_remote_datasource.dart  # API calls
        │   └── repositories/
        │       └── catalog_repository_impl.dart    # Repository impl
        └── presentation/
            └── providers/
                └── catalog_providers.dart       # Riverpod providers
```

---

## Dependencies Added

```yaml
# HTTP Client
dio: ^5.7.0                    # HTTP client with interceptors
pretty_dio_logger: ^1.4.0      # Request/response logging

# Network
connectivity_plus: ^6.0.5       # Network status detection
```

---

## API Endpoints

### Base URL
```
https://staging.api.sigook.ca/api
```

### Catalog Endpoints
| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/Catalog/availability` | GET | Get availability options |
| `/Catalog/availabilityTime` | GET | Get availability time slots |
| `/Catalog/country` | GET | Get countries |
| `/Catalog/gender` | GET | Get gender options |
| `/Catalog/identificationType` | GET | Get ID types |
| `/Catalog/language` | GET | Get languages |
| `/Catalog/skills` | GET | Get skills |

---

## Core Components

### 1. API Client (`api_client.dart`)

**Features:**
- ✅ Dio HTTP client configuration
- ✅ Request/response interceptors
- ✅ Pretty logging (debug mode)
- ✅ Error handling
- ✅ 30-second timeout
- ✅ JSON content-type headers

**Usage:**
```dart
final apiClient = ApiClient();

// GET request
final response = await apiClient.get('/Catalog/country');

// POST request
final response = await apiClient.post('/endpoint', data: {});
```

**Configuration:**
- Base URL: `https://staging.api.sigook.ca/api`
- Connect Timeout: 30 seconds
- Receive Timeout: 30 seconds
- Headers: JSON content-type

---

### 2. Network Info (`network_info.dart`)

**Purpose:** Check internet connectivity before making API calls

**Usage:**
```dart
final networkInfo = NetworkInfoImpl(Connectivity());

if (await networkInfo.isConnected) {
  // Make API call
} else {
  // Show no internet error
}
```

**Detects:**
- ✅ WiFi connection
- ✅ Mobile data
- ✅ Ethernet connection

---

### 3. Exceptions (`exceptions.dart`)

**Exception Types:**
- `ServerException` - 4xx/5xx HTTP errors
- `NetworkException` - Connection/timeout errors
- `CacheException` - Local storage errors
- `ParseException` - JSON parsing errors

**Usage:**
```dart
try {
  final data = await apiClient.get('/endpoint');
} on DioException catch (e) {
  if (e.type == DioExceptionType.connectionTimeout) {
    throw NetworkException('Request timeout');
  }
  throw ServerException(message: 'Server error');
}
```

---

### 4. Failures (`failures.dart`)

**Failure Types:**
- `ServerFailure` - Server-side errors
- `NetworkFailure` - Connection issues
- `CacheFailure` - Storage errors
- `ParseFailure` - Data parsing errors
- `ValidationFailure` - Input validation errors

**Usage with Either:**
```dart
Future<Either<Failure, List<CatalogItem>>> getData() async {
  if (!await networkInfo.isConnected) {
    return Left(NetworkFailure());
  }
  
  try {
    final data = await remoteDataSource.getData();
    return Right(data);
  } on ServerException catch (e) {
    return Left(ServerFailure(message: e.message));
  }
}
```

---

## Domain Layer

### Catalog Item Entity
```dart
class CatalogItem extends Equatable {
  final int id;
  final String name;
  final String? description;
  final bool isActive;
  
  // Immutable, testable entity
}
```

### Repository Interface
```dart
abstract class CatalogRepository {
  Future<Either<Failure, List<CatalogItem>>> getAvailability();
  Future<Either<Failure, List<CatalogItem>>> getCountries();
  Future<Either<Failure, List<CatalogItem>>> getGenders();
  // ... other methods
}
```

### Use Cases
Each catalog type has its own use case:
- `GetAvailability`
- `GetAvailabilityTime`
- `GetCountries`
- `GetGenders`
- `GetIdentificationTypes`
- `GetLanguages`
- `GetSkills`

---

## Data Layer

### 1. Data Model (`catalog_item_model.dart`)

**Features:**
- ✅ JSON serialization with `json_annotation`
- ✅ Extends domain entity
- ✅ Auto-generated `fromJson`/`toJson`

**Example:**
```dart
@JsonSerializable()
class CatalogItemModel extends CatalogItem {
  factory CatalogItemModel.fromJson(Map<String, dynamic> json) =>
      _$CatalogItemModelFromJson(json);
      
  Map<String, dynamic> toJson() => 
      _$CatalogItemModelToJson(this);
}
```

### 2. Remote Data Source

**Handles:**
- ✅ HTTP requests to API
- ✅ Response parsing
- ✅ Exception throwing

**Example:**
```dart
class ApiClient {
  Future<Response> get(String path) async {
    return await _dio.get('https://staging.api.sigook.ca/api' + path);
  }
}

Future<List<CatalogItemModel>> _getCatalogItems(String endpoint) async {
  try {
    final response = await apiClient.get(endpoint);
    final List<dynamic> jsonList = response.data;
    return jsonList.map((json) => 
        CatalogItemModel.fromJson(json)).toList();
  } on DioException catch (e) {
    // Handle errors
  }
}
```

### 3. Repository Implementation

**Handles:**
- ✅ Network connectivity checks
- ✅ Exception to Failure conversion
- ✅ Either monad for error handling

**Flow:**
```
Check Network → Call DataSource → Handle Exceptions → Return Either
```

---

## Presentation Layer

### Riverpod Providers

**Provider Hierarchy:**
```
apiClientProvider
       ↓
catalogRemoteDataSourceProvider
       ↓
catalogRepositoryProvider
       ↓
getCountriesProvider (use case)
       ↓
countriesListProvider (FutureProvider)
```

**Usage in UI:**
```dart
class CountryDropdown extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final countriesAsync = ref.watch(countriesListProvider);
    
    return countriesAsync.when(
      data: (countries) => Dropdown(items: countries),
      loading: () => CircularProgressIndicator(),
      error: (error, stack) => Text('Error: $error'),
    );
  }
}
```

---

## Available Providers

### Use Case Providers
```dart
getAvailabilityProvider
getAvailabilityTimeProvider
getCountriesProvider
getGendersProvider
getIdentificationTypesProvider
getLanguagesProvider
getSkillsProvider
```

### Data Providers (FutureProvider)
```dart
availabilityListProvider
availabilityTimeListProvider
countriesListProvider
gendersListProvider
identificationTypesListProvider
languagesListProvider
skillsListProvider
```

---

## Error Handling

### 1. Network Layer
```dart
// API Client handles Dio exceptions
try {
  final response = await apiClient.get('/endpoint');
} on DioException catch (e) {
  if (e.type == DioExceptionType.connectionTimeout) {
    throw NetworkException('Timeout');
  }
  throw ServerException(message: e.message);
}
```

### 2. Repository Layer
```dart
// Converts exceptions to failures
try {
  final data = await remoteDataSource.getData();
  return Right(data);
} on ServerException catch (e) {
  return Left(ServerFailure(message: e.message));
} on NetworkException catch (e) {
  return Left(NetworkFailure(message: e.message));
}
```

### 3. Presentation Layer
```dart
// Handles failures in UI
final result = ref.watch(countriesListProvider);

result.when(
  data: (countries) => ShowData(countries),
  loading: () => LoadingIndicator(),
  error: (error, stack) => ErrorMessage(error.toString()),
);
```

---

## Testing Strategy

### 1. Unit Tests
```dart
test('should return countries when API call succeeds', () async {
  // Arrange
  when(mockApiClient.get('/Catalog/country'))
      .thenAnswer((_) async => Response(data: mockData));
      
  // Act
  final result = await dataSource.getCountries();
  
  // Assert
  expect(result, isA<List<CatalogItemModel>>());
});
```

### 2. Integration Tests
```dart
testWidgets('should display countries in dropdown', (tester) async {
  await tester.pumpWidget(MyApp());
  await tester.pumpAndSettle();
  
  expect(find.byType(DropdownButton), findsOneWidget);
  expect(find.text('United States'), findsOneWidget);
});
```

---

## Usage Examples

### Example 1: Display Countries in Dropdown

```dart
class CountrySelector extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final countriesAsync = ref.watch(countriesListProvider);
    
    return countriesAsync.when(
      data: (countries) {
        return DropdownButtonFormField<int>(
          items: countries.map((country) {
            return DropdownMenuItem(
              value: country.id,
              child: Text(country.name),
            );
          }).toList(),
          onChanged: (value) {
            // Handle selection
          },
        );
      },
      loading: () => CircularProgressIndicator(),
      error: (error, _) => Text('Error loading countries'),
    );
  }
}
```

### Example 2: Multiple Selects (Skills)

```dart
class SkillsSelector extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final skillsAsync = ref.watch(skillsListProvider);
    
    return skillsAsync.when(
      data: (skills) {
        return MultiSelectChips<CatalogItem>(
          label: 'Skills',
          options: skills,
          selectedOptions: selectedSkills,
          getLabel: (skill) => skill.name,
          onChanged: (selected) {
            // Update selected skills
          },
        );
      },
      loading: () => CircularProgressIndicator(),
      error: (error, _) => Text('Error loading skills'),
    );
  }
}
```

---

## Code Generation

### Run build_runner
```bash
# Generate JSON serialization code
flutter pub run build_runner build --delete-conflicting-outputs

# Watch mode (auto-generate on file changes)
flutter pub run build_runner watch --delete-conflicting-outputs
```

---

## Best Practices

### ✅ DO
- Use `Either<Failure, T>` for error handling
- Check network connectivity before API calls
- Log HTTP requests in debug mode only
- Handle all exception types
- Use immutable entities
- Separate concerns (domain/data/presentation)
- Use proper timeout values

### ❌ DON'T
- Throw exceptions from repositories (use Either)
- Make API calls directly from UI
- Hardcode API URLs in widgets
- Ignore network connectivity
- Parse JSON in presentation layer
- Mix business logic with UI code

---

## Benefits of This Approach

### 1. **Clean Architecture**
- Clear separation of concerns
- Testable business logic
- Independent of frameworks

### 2. **Type Safety**
- Compile-time error checking
- Auto-completion in IDE
- Reduced runtime errors

### 3. **Error Handling**
- Explicit error states
- No try-catch in UI
- User-friendly error messages

### 4. **Maintainability**
- Easy to add new endpoints
- Consistent code structure
- Clear dependencies

### 5. **Testability**
- Mock-friendly interfaces
- Unit testable use cases
- Integration testable UI

---

## Next Steps

1. **Update Form Pages** - Replace enums with API data
2. **Add Caching** - Store catalog data locally
3. **Add Retry Logic** - Auto-retry failed requests
4. **Add Refresh** - Pull-to-refresh for stale data
5. **Add Pagination** - For large datasets
6. **Add Search** - Filter catalog items

---

**Your HTTP services are production-ready and follow industry best practices!** 🚀

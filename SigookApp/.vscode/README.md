# VS Code Debug Configurations

This project includes VS Code launch configurations for easy debugging of different environments.

## Available Debug Configurations

### **Environment-Based Launches**

1. **Staging** - Default development mode, loads staging environment
2. **Production** - Production build (use carefully)
3. **Local (localhost services)** - Points to local backend services

### **Advanced**

- **Attach to Flutter Process**: For attaching debugger to running Flutter app

## How to Use

1. **Open VS Code** in the project directory
2. **Go to Run & Debug** (Ctrl+Shift+D / Cmd+Shift+D)
3. **Select a configuration** from the dropdown
4. **Click the green play button** or press F5

## Environment Details

### **Staging**

- Target: `lib/main_staging.dart`
- Environment variables: Loaded from `.env.staging` via `--dart-define-from-file`
- App Name: "Sigook (Staging)"

### **Production**

- Target: `lib/main_production.dart`
- Environment variables: Loaded from `.env.production` via `--dart-define-from-file`
- App Name: "Sigook"

### **Local**

- Target: `lib/main_local.dart`
- Environment variables: Loaded from `.env.local` via `--dart-define-from-file`

## Tips

- **Hot Reload**: Works with all configurations
- **Environment switching**: No runtime switching - each build is locked to its environment via `--dart-define-from-file`

## Troubleshooting

### Device not found

- For Android: Start emulator first or connect device
- For iOS: Make sure Simulator is running

### Environment file not found

- Ensure `.env.staging`, `.env.production`, and `.env.local` exist in project root
- Copy from `.env.example` if missing

---

**Note**: Production environment should only be used for testing production builds. Never debug production in development.

# VS Code Debug Configurations

This project includes VS Code launch configurations for easy debugging of different environments.

## 🚀 Available Debug Configurations

### **Environment-Based Launches**

1. **Development (Staging)** - Default development mode, loads staging environment
2. **Staging Environment** - Explicit staging build
3. **Production Environment** - Explicit production build (⚠️ Use carefully)

### **Platform-Specific Launches**

Each environment has iOS Simulator and Android Emulator variants:

- **iOS Simulator**: Targets iOS Simulator
- **Android Emulator**: Targets Android emulator (use `emulator` device ID)

### **Advanced**

- **Attach to Flutter Process**: For attaching debugger to running Flutter app

## 🎯 How to Use

1. **Open VS Code** in the project directory
2. **Go to Run & Debug** (Ctrl+Shift+D / Cmd+Shift+D)
3. **Select a configuration** from the dropdown
4. **Click the green play button** or press F5

## 📱 Environment Details

### **Development (Staging)**
- Target: `lib/main.dart`
- Flavor: `staging`
- Environment: Loads `.env.staging`
- App Name: "Sigook (Staging)"

### **Staging Environment**
- Target: `lib/main_staging.dart`
- Flavor: `staging`
- Environment: Loads `.env.staging`
- App Name: "Sigook (Staging)"

### **Production Environment**
- Target: `lib/main_production.dart`
- Flavor: `production`
- Environment: Loads `.env.production`
- App Name: "Sigook"

## 🔧 Configuration Details

- **Pre-launch task**: Automatically runs `flutter pub get`
- **Console output**: Debug console for logs
- **Device targeting**: Automatic for platform variants

## 💡 Tips

- **Hot Reload**: Works with all configurations
- **Environment switching**: No runtime switching - each build is locked to its environment
- **Multiple instances**: Can run staging and production simultaneously on the same device
- **iOS schemes**: Make sure Xcode schemes are configured (see `IOS_SCHEMES_SETUP.md`)

## 🐛 Troubleshooting

**"Device not found"**
- For Android: Start emulator first or connect device
- For iOS: Make sure Simulator is running

**"Flavor not found"**
- Run `flutter clean` then try again
- Check that Android flavors are configured in `android/app/build.gradle.kts`

**"Environment file not found"**
- Ensure `.env.staging` and `.env.production` exist in project root
- Check that they're listed in `pubspec.yaml` assets

---

**Note**: Production environment should only be used for testing production builds. Never debug production in development.

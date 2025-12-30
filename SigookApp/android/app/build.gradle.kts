plugins {
    id("com.android.application")
    id("kotlin-android")
    id("dev.flutter.flutter-gradle-plugin")
}

android {
    namespace = "com.example.sigook_app_flutter"

    compileSdk = 36
    ndkVersion = flutter.ndkVersion

    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
        isCoreLibraryDesugaringEnabled = true
    }

    kotlinOptions {
        jvmTarget = "17"   // ← string, not JavaVersion.VERSION_17
    }

    defaultConfig {
        applicationId = "com.example.sigook_app_flutter"
        minSdk = flutter.minSdkVersion
        targetSdk = 36
        versionCode = flutter.versionCode
        versionName = flutter.versionName
        
        // Required for flutter_appauth
        manifestPlaceholders["appAuthRedirectScheme"] = "sigookcallback"
        
        // Add multiDex support
        multiDexEnabled = true
    }

    flavorDimensions.add("environment")

    productFlavors {
        create("staging") {
            dimension = "environment"
            applicationIdSuffix = ".staging"
            versionNameSuffix = "-staging"
            resValue("string", "app_name", "Sigook (Staging)")
        }

        create("production") {
            dimension = "environment"
            resValue("string", "app_name", "Sigook")
        }
    }

    buildTypes {
        release {
            signingConfig = signingConfigs.getByName("debug")
        }
    }
}

flutter {
    source = "../.."
}

dependencies {
    coreLibraryDesugaring("com.android.tools:desugar_jdk_libs:2.1.2")
    implementation("androidx.multidex:multidex:2.0.1")
}

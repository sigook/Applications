// ---------------------------------------------------------
// Root Gradle (android/build.gradle.kts)
// ---------------------------------------------------------

allprojects {
    repositories {
        google()
        mavenCentral()
    }
}

// ---- Custom build directory logic (kept exactly as you had it) ----
val newBuildDir: Directory =
    rootProject.layout.buildDirectory
        .dir("../../build")
        .get()

rootProject.layout.buildDirectory.value(newBuildDir)

subprojects {
    val newSubprojectBuildDir: Directory = newBuildDir.dir(project.name)
    project.layout.buildDirectory.value(newSubprojectBuildDir)
}

// ---- IMPORTANT: compileOptions block removed ---
// AGP 8+ does NOT allow setting compileOptions from the root.
// ---------------------------------------------------------

tasks.register<Delete>("clean") {
    delete(rootProject.layout.buildDirectory)
}

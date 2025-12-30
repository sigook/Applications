# Azure DevOps Pipelines - Guía de Configuración

Este directorio contiene los pipelines de CI/CD para las aplicaciones del monorepo Covenant/Sigook.

## 📁 Estructura de Pipelines

```
.azure-pipelines/
├── sigookapp-pipeline.yml                 # Pipeline para Flutter mobile app (placeholder)
├── sigook-web-pipeline.yml                # Pipeline para Sigook.Web Vue.js app (completo)
├── covenantweb-pipeline.yml               # Pipeline para CovenantWeb marketing (completo)
├── covenant-api-pipeline.yml              # Pipeline para .NET API (completo)
├── covenant-common-nuget-pipeline.yml     # Pipeline para NuGet package (completo)
├── templates/                             # Templates reutilizables
│   ├── dotnet-setup.yml                   # Template: Instalar .NET SDK
│   └── dotnet-build-test.yml              # Template: Build y Tests
└── README.md                              # Esta guía
```

## 🎯 Características Clave

### Triggers Inteligentes Basados en Paths

Cada pipeline **solo se ejecuta cuando hay cambios en su aplicación específica**:

- **sigookapp-pipeline.yml**: Se activa solo con cambios en `SigookApp/**`
- **sigook-web-pipeline.yml**: Se activa solo con cambios en `Sigook.Web/**`
- **covenantweb-pipeline.yml**: Se activa solo con cambios en `covenantWeb/**`
- **covenant-api-pipeline.yml**: Se activa solo con cambios en `Covenant.Api/**` (excepto Covenant.Common)
- **covenant-common-nuget-pipeline.yml**: Se activa solo con cambios en `Covenant.Api/Covenant.Common/**` (solo rama dev)

**Beneficios:**
- ✅ Ahorro de tiempo de build (no ejecuta pipelines innecesarios)
- ✅ Ahorro de minutos de Azure DevOps
- ✅ Feedback más rápido en PRs

### Ambientes Condicionales

Los pipelines detectan automáticamente el ambiente basado en la rama:

| Rama | Ambiente | Build Command |
|------|----------|---------------|
| `main` | Production | `build:production` |
| `dev` | Staging | `build:staging` |
| `feature/*` | Staging | `build:staging` |

**No hay stages duplicados** - un solo pipeline maneja ambos ambientes con variables condicionales.

### Estrategia de Validación de PRs

Los pipelines implementan una **estrategia optimizada** para evitar duplicación de tests:

**✅ Pull Requests hacia `dev`:**
- Pipeline **SÍ se ejecuta** con validación completa (build, tests, linting)
- Garantiza que nada roto llegue a dev
- Quality gate principal del proyecto

**❌ Pull Requests hacia `main`:**
- Pipeline **NO se ejecuta**
- Se confía en que dev ya validó el código
- Evita duplicación innecesaria de tests
- Ahorra tiempo y recursos

**🔒 Push directo a `dev` o `main`:**
- Pipeline **SÍ se ejecuta** con todo el flujo (build, test, deploy)
- `dev` → Deploy a Staging
- `main` → Deploy a Production

**Ventajas:**
- ✅ Evita correr tests 2-3 veces para el mismo código
- ✅ Feedback más rápido en PRs hacia main
- ✅ Ahorra minutos de Azure DevOps
- ✅ Dev actúa como el quality gate principal

**Requisito:** Branch protection configurado en `main` para requerir PRs y approvals (ver sección de configuración).

### Templates Reutilizables

Los pipelines de .NET usan **templates reutilizables** para evitar duplicación de código y mantener consistencia:

#### 📄 `templates/dotnet-setup.yml`

Template para instalar .NET SDK:

```yaml
# Uso:
- template: templates/dotnet-setup.yml
  parameters:
    sdkVersion: '6.0.400'
```

**Parámetros:**
- `sdkVersion` (string): Versión del SDK a instalar (default: '6.0.400')

#### 📄 `templates/dotnet-build-test.yml`

Template para build y ejecución de tests:

```yaml
# Uso:
- template: templates/dotnet-build-test.yml
  parameters:
    buildProjects: '**/*.sln'
    buildConfiguration: 'Release'
    runUnitTests: true
    unitTestProjects: '**/Covenant.Tests/*.csproj'
    runIntegrationTests: false
```

**Parámetros:**
- `buildProjects` (string, requerido): Pattern de proyectos/soluciones a compilar
- `buildConfiguration` (string): Configuración de build (default: 'Release')
- `runUnitTests` (bool): Ejecutar unit tests (default: true)
- `unitTestProjects` (string): Pattern de proyectos de unit tests
- `runIntegrationTests` (bool): Ejecutar integration tests (default: false)
- `integrationTestProjects` (string): Pattern de proyectos de integration tests

**Beneficios:**
- ✅ Código DRY (Don't Repeat Yourself)
- ✅ Fácil mantenimiento (cambios en un solo lugar)
- ✅ Consistencia entre pipelines
- ✅ Configuración flexible mediante parámetros

## 🚀 Configuración Inicial en Azure DevOps

### Paso 1: Crear los Pipelines

1. Ve a **Azure DevOps** → Tu proyecto → **Pipelines**
2. Click en **New Pipeline**

#### Pipeline 1: CovenantWeb
```
1. Where is your code? → Azure Repos Git (o tu proveedor)
2. Select a repository → Tu repositorio
3. Configure your pipeline → Existing Azure Pipelines YAML file
4. Path: /.azure-pipelines/covenantweb-pipeline.yml
5. Review and create → Save (no ejecutar todavía)
6. Rename pipeline a: "CovenantWeb-CI/CD"
```

#### Pipeline 2: SigookApp
```
1. Repetir proceso anterior
2. Path: /.azure-pipelines/sigookapp-pipeline.yml
3. Rename pipeline a: "SigookApp-CI/CD"
```

#### Pipeline 3: Sigook.Web
```
1. Repetir proceso anterior
2. Path: /.azure-pipelines/sigook-web-pipeline.yml
3. Rename pipeline a: "Sigook.Web-CI/CD"
```

#### Pipeline 4: Covenant.Api
```
1. Repetir proceso anterior
2. Path: /.azure-pipelines/covenant-api-pipeline.yml
3. Rename pipeline a: "Covenant.Api-CI/CD"
```

#### Pipeline 5: Covenant.Common (NuGet)
```
1. Repetir proceso anterior
2. Path: /.azure-pipelines/covenant-common-nuget-pipeline.yml
3. Rename pipeline a: "Covenant.Common-NuGet"
4. ⚠️ Este pipeline solo se ejecuta en rama dev cuando hay cambios en Covenant.Common
```

### Paso 2: Crear Environments

Los environments en Azure DevOps permiten:
- Tracking de deployments
- Approvals (requerido para production)
- Deployment history

**Crear environments:**

1. Ve a **Pipelines** → **Environments**
2. Click **New environment**

#### Environment: staging
```
Name: staging
Description: Staging environment for testing
Resource: None
```

#### Environment: production
```
Name: production
Description: Production environment
Resource: None

⚠️ Importante: Configura APPROVALS después de crearlo:
1. Click en el environment "production"
2. Menu (···) → Approvals and checks
3. Add check → Approvals
4. Add approvers (tu equipo)
5. Save
```

### Paso 3: Configurar Variables (si es necesario)

Si necesitas variables secretas o específicas por ambiente:

1. Ve a **Pipelines** → **Library**
2. Click **+ Variable group**

#### Variable Group: CovenantWeb-Staging (ejemplo)
```
Name: CovenantWeb-Staging
Variables:
  - AZURE_STATIC_WEB_APPS_TOKEN: [tu token]
  - API_ENDPOINT: https://api-staging.covenant.com
  (etc.)

⚠️ Para secrets: Click en el candado 🔒 junto a la variable
```

Luego en tu pipeline, referencia el group:
```yaml
variables:
  - group: CovenantWeb-Staging  # Agregar esta línea
```

## 📋 Pipelines Detallados

### CovenantWeb Pipeline

**Archivo:** `covenantweb-pipeline.yml`

**Stages:**
1. **CI** - Build & Validate
   - Instala Node.js 20.x
   - Usa caché para node_modules
   - Type checking con vue-tsc
   - Linting con ESLint
   - Build (staging o production según rama)
   - Publica artifacts

2. **CD** - Deploy
   - Descarga artifacts del stage CI
   - Despliega al ambiente correcto
   - Requiere aprobación para production

**Artifacts Generados:**
- `covenantweb-staging` (rama dev)
- `covenantweb-production` (rama main)

**Triggers:**
- Push a `main` o `dev` con cambios en `covenantWeb/**`
- Pull Requests a `main` o `dev`
- Excluye: READMEs y archivos markdown

### SigookApp Pipeline (Placeholder)

**Archivo:** `sigookapp-pipeline.yml`

**Estado:** Placeholder con validación básica

**Stages Actuales:**
1. **Validate** - Validación básica del proyecto
   - Verifica estructura Flutter
   - Lista archivos principales
   - Valida pubspec.yaml y directorios

2. **BuildStaging** - Placeholder con TODOs
3. **BuildProduction** - Placeholder con TODOs

**Para Expandir:**
Cuando estés listo para implementar el build completo de Flutter, el pipeline debería incluir:
- Instalación de Flutter SDK
- `flutter pub get`
- `flutter pub run build_runner build`
- `flutter analyze`
- `flutter test`
- `flutter build apk/aab` con flavors
- Firma de APK con keystore
- Publicación a Firebase App Distribution o Play Store

### Sigook.Web Pipeline

**Archivo:** `sigook-web-pipeline.yml`

**Propósito:** Aplicación web principal de Sigook (Vue.js 2) desplegada como contenedor Docker.

**Stages:**
1. **Build and Validate** - Validación y Linting
   - Instala Node.js 16.x
   - Usa caché para node_modules
   - Linting con ESLint
   - Validación de build

2. **Build Docker and Deploy** - Dockerización y Deployment
   - Replace tokens (versión en index.html y version.json)
   - Build de imagen Docker multi-stage (Node.js → Nginx)
   - Push a Azure Container Registry (ACR)
   - Deploy a Azure App Service Container
   - Tags: `latest_staging` o `latest_production`

**Tecnología:**
- Vue.js 2 con vue-cli-service
- Node.js 16 para build
- Nginx stable-alpine para serving
- Docker multi-stage build

**Triggers:**
- Push a `main`, `master`, o `dev` con cambios en `Sigook.Web/**`
- Pull Requests a `dev` (NO a main)
- Excluye: archivos markdown

**Deployment Targets:**
- Staging: `sigook-web-staging.azurewebsites.net`
- Production: `sigook.azurewebsites.net`

**Build Arguments:**
- `ENV=staging` o `ENV=production` (usado en Dockerfile para ejecutar `npm run staging` o `npm run production`)

### Covenant.Api Pipeline

**Archivo:** `covenant-api-pipeline.yml`

**Stages:**
1. **Build and Test** - Compilación y Quality Gate
   - Instala .NET SDK 6.0.400 (usando template)
   - Build de la solución completa
   - Corre Unit Tests
   - Corre Integration Tests
   - Usa templates reutilizables

2. **Build Docker and Deploy** - Dockerización y Deployment
   - Build de imagen Docker
   - Push a Azure Container Registry (ACR)
   - Deploy a Azure App Service (staging o production)
   - Tags: `latest_staging` o `latest_production`

**Triggers:**
- Push a `main`, `master`, o `dev` con cambios en `Covenant.Api/**`
- Pull Requests a `main`, `master`, o `dev`
- Excluye: archivos markdown

**Deployment Targets:**
- Staging: `sigook-api-staging.azurewebsites.net`
- Production: `sigook-api.azurewebsites.net`

### Covenant.Common NuGet Pipeline

**Archivo:** `covenant-common-nuget-pipeline.yml`

**Propósito:** Publicar el paquete NuGet `Covenant.Common` cuando hay cambios en la librería compartida.

**Stages:**
1. **Build, Test, and Publish NuGet**
   - **Quality Gate Job**:
     - Instala .NET SDK (usando template)
     - Build de la solución completa
     - Corre Unit Tests (garantiza calidad antes de publicar)
     - Usa templates reutilizables

   - **Pack and Publish Job**:
     - Pack del proyecto Covenant.Common
     - Autentica con Azure Artifacts
     - Publica a feed `sigook/Covenant.Common`
     - Versión automática basada en build number

**Triggers:**
- ⚠️ **Solo rama `dev`**
- Solo cuando hay cambios en `Covenant.Api/Covenant.Common/**`
- Excluye: archivos markdown
- **No se ejecuta en PRs** (solo pushes directos)

**Características Especiales:**
- ✅ **Quality Gate obligatorio**: Los tests deben pasar antes de publicar
- ✅ **Path-based trigger**: Solo se ejecuta cuando Covenant.Common cambia
- ✅ **Versión automática**: Usa el build number como versión del paquete
- ✅ **Templates compartidos**: Reutiliza templates de .NET

**Consumir el paquete:**
```bash
dotnet add package Covenant.Common --version <Build.BuildNumber>
```

## 🔧 Deployment Configuration

### CovenantWeb - Azure App Service (Linux)

El pipeline está configurado para desplegar a **Azure App Service en Linux** usando Node.js.

#### Configuración Actual:

```yaml
- task: AzureWebApp@1
  inputs:
    azureSubscription: 'SigookPipelines'
    appType: 'webAppLinux'
    appName: '$(azureAppName)'              # covenantgroup o covenantgroup-staging
    package: '$(Pipeline.Workspace)/covenantweb-drop/*.zip'
    runtimeStack: 'NODE|20-lts'
    startUpCommand: 'npm start'             # Ejecuta: serve -s dist -l 8080
    appSettings: '-CVN_VERSION $(Build.BuildId)'
```

#### App Services Configurados:

| Ambiente | App Service Name | URL |
|----------|------------------|-----|
| Staging | `covenantgroup-staging` | https://covenantgroup-staging.azurewebsites.net |
| Production | `covenantgroup` | https://covenantgroup.azurewebsites.net |

#### Cómo Funciona:

1. **Build Stage**: Compila la aplicación Vue.js (`npm run build:staging` o `build:production`)
2. **Archive**: Empaqueta TODO el proyecto (no solo dist) en un ZIP
3. **Deploy**: Azure descomprime el ZIP y ejecuta `npm start`
4. **Serve**: El paquete `serve` sirve los archivos de `dist/` en el puerto 8080

#### Servidor de Producción:

La aplicación usa el paquete `serve` para servir archivos estáticos:

```json
// package.json
{
  "scripts": {
    "start": "serve -s dist -l 8080"
  },
  "dependencies": {
    "serve": "^14.2.3"
  }
}
```

**Características de `serve`:**
- ✅ Optimizado para SPAs (Single Page Applications)
- ✅ Automáticamente redirige todas las rutas a `index.html` (Vue Router)
- ✅ Sirve archivos estáticos con headers correctos
- ✅ Ligero y rápido

## 🧪 Probar los Pipelines

### Test 1: Cambio solo en CovenantWeb
```bash
# Hacer un cambio en covenantWeb
echo "// test" >> covenantWeb/src/App.vue

git add covenantWeb/src/App.vue
git commit -m "test: pipeline trigger test"
git push origin dev
```

**Resultado esperado:** Solo corre `covenantweb-pipeline.yml` ✅

### Test 2: Cambio solo en SigookApp
```bash
# Hacer un cambio en SigookApp
echo "// test" >> SigookApp/lib/main.dart

git add SigookApp/lib/main.dart
git commit -m "test: pipeline trigger test"
git push origin dev
```

**Resultado esperado:** Solo corre `sigookapp-pipeline.yml` ✅

### Test 3: Cambio en documentación
```bash
# Modificar README
echo "test" >> README.md

git add README.md
git commit -m "docs: update readme"
git push origin dev
```

**Resultado esperado:** NO corre ningún pipeline ✅

## 📊 Monitoring y Troubleshooting

### Ver Ejecuciones de Pipeline
1. Azure DevOps → Pipelines
2. Click en el pipeline específico
3. Ve el historial de runs

### Ver Artifacts Generados
1. Click en un pipeline run
2. Tab "Artifacts"
3. Download para inspeccionar

### Logs Detallados
- Cada step tiene logs expandibles
- Usa `displayName` descriptivos (ya incluidos)
- Los scripts Bash muestran información útil

### Common Issues

#### Pipeline no se ejecuta
- ✅ Verifica que los cambios estén en el path correcto (`covenantWeb/**` o `SigookApp/**`)
- ✅ Verifica que el archivo no esté en la lista de exclusión (`.md` files)
- ✅ Verifica que la rama esté en la lista de triggers (`main` o `dev`)

#### Build falla
- ✅ Verifica que `package.json` tenga los scripts correctos
- ✅ Verifica versión de Node.js (requiere 20.x)
- ✅ Revisa los logs del step específico que falla

#### Deployment no funciona
- ✅ Los deployment steps están comentados por defecto
- ✅ Descomenta y configura el método que uses
- ✅ Configura las variables/secrets necesarias

## 🎯 Next Steps

### Para CovenantWeb:
1. ✅ Pipeline creado y funcionando
2. ⏳ Configurar deployment real (descomentar y configurar en CD stage)
3. ⏳ Agregar variables de ambiente si son necesarias
4. ⏳ Configurar approvals para production

### Para SigookApp:
1. ✅ Placeholder creado con triggers inteligentes
2. ⏳ Expandir con Flutter SDK installation
3. ⏳ Agregar build de APK/AAB
4. ⏳ Configurar keystore para firma
5. ⏳ Agregar deployment a Firebase App Distribution o Play Store

## 📚 Referencias

- [Azure Pipelines YAML Schema](https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema)
- [Triggers Documentation](https://learn.microsoft.com/en-us/azure/devops/pipelines/build/triggers)
- [Environments and Approvals](https://learn.microsoft.com/en-us/azure/devops/pipelines/process/environments)
- [Variable Groups](https://learn.microsoft.com/en-us/azure/devops/pipelines/library/variable-groups)

## 💬 Soporte

Si encuentras problemas con los pipelines:
1. Revisa los logs detallados en Azure DevOps
2. Verifica la configuración de environments y variables
3. Consulta la documentación oficial de Microsoft

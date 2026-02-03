# Covenant/Sigook Platform - Documentación Técnica

Bienvenido a la documentación técnica completa de la plataforma Covenant/Sigook.

## 📚 Índice de Documentación

### 🎯 Negocio y Modelo de Dominio
- **[BUSINESS_MODEL.md](./BUSINESS_MODEL.md)** - Modelo de negocio, actores principales, value proposition, y flujo general del sistema

### 🏗️ Arquitectura y Estructura
- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Stack tecnológico, estructura de capas, módulos principales, y organización de proyectos

### 📊 Modelo de Datos
- **[ENTITIES_RELATIONSHIPS.md](./ENTITIES_RELATIONSHIPS.md)** - Entidades principales, relaciones, y diagramas del modelo de datos

### 🔌 APIs y Endpoints
- **[API_ENDPOINTS.md](./API_ENDPOINTS.md)** - Documentación completa de endpoints por módulo (Agency, Company, Worker, Accounting)

### 💰 Reglas de Payroll
- **[PAYROLL_RULES.md](./PAYROLL_RULES.md)** - Cálculos de nómina, deducciones (CPP, EI), impuestos federales y provinciales (Canadá)

### 📄 Reglas de Facturación
- **[BILLING_RULES.md](./BILLING_RULES.md)** - Generación de invoices, HST/GST, rates, markup calculations

### ⏱️ Reglas de Timesheets
- **[TIMESHEET_RULES.md](./TIMESHEET_RULES.md)** - Cálculos de horas (regular, overtime, night shift, holiday), validaciones

### 🔄 Workflows Principales
- **[WORKFLOWS.md](./WORKFLOWS.md)** - Flujos detallados paso a paso (Worker Registration, Job Matching, Payroll Processing, etc.)

### 🔧 DevOps e Infraestructura
- **[AZURE_DEVOPS_SELF_HOSTED_AGENT.md](./AZURE_DEVOPS_SELF_HOSTED_AGENT.md)** - Configuración y mantenimiento del self-hosted agent de Azure DevOps, VM setup, troubleshooting

---

## 🚀 Inicio Rápido

### Para entender el negocio:
1. Lee [BUSINESS_MODEL.md](./BUSINESS_MODEL.md) - Comprende qué problema resuelve la plataforma
2. Lee [WORKFLOWS.md](./WORKFLOWS.md) - Entiende los flujos principales

### Para desarrollo backend:
1. Lee [ARCHITECTURE.md](./ARCHITECTURE.md) - Estructura de capas y módulos
2. Lee [ENTITIES_RELATIONSHIPS.md](./ENTITIES_RELATIONSHIPS.md) - Modelo de datos
3. Lee [API_ENDPOINTS.md](./API_ENDPOINTS.md) - Endpoints disponibles

### Para modificar payroll:
1. Lee [PAYROLL_RULES.md](./PAYROLL_RULES.md) - Reglas de cálculo de nómina
2. Referencia: `Covenant.Api/Covenant.PayStubs/` y `Covenant.Api/Covenant.Deductions/`

### Para modificar facturación:
1. Lee [BILLING_RULES.md](./BILLING_RULES.md) - Reglas de facturación
2. Referencia: `Covenant.Api/Covenant.Billing/`

### Para modificar timesheets:
1. Lee [TIMESHEET_RULES.md](./TIMESHEET_RULES.md) - Reglas de cálculo de horas
2. Referencia: `Covenant.Api/Covenant.Core.BL/Services/TimeSheetService.cs`

---

## 📝 Cómo usar esta documentación

### Como desarrollador nuevo:
Lee los documentos en orden: BUSINESS_MODEL → ARCHITECTURE → ENTITIES_RELATIONSHIPS → el resto según tu área

### Como Claude Code (IA):
Cuando recibas un requerimiento:
1. Identifica el área (payroll, billing, timesheets, API, etc)
2. Lee el documento relevante de `.docs/`
3. Referencia el código correspondiente
4. Implementa siguiendo los patrones existentes

### Como arquitecto/lead:
Estos documentos son la fuente de verdad. Actualízalos cuando cambies reglas de negocio o arquitectura.

---

## 🔄 Mantenimiento

**Importante:** Mantén esta documentación actualizada cuando:
- Cambies reglas de negocio (payroll, billing, validation rules)
- Agregues nuevos módulos o servicios
- Modifiques la arquitectura
- Agregues nuevos endpoints
- Cambies el modelo de datos

**Formato:** Todos los archivos usan Markdown para fácil lectura en GitHub/Azure DevOps

---

## 📞 Contacto y Recursos

- **Repositorio principal:** Ver README.md en la raíz
- **CI/CD:** Ver `.azure-pipelines/README.md`
- **CLAUDE.md:** Instrucciones para Claude Code

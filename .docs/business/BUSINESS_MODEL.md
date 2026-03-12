# Modelo de Negocio - Covenant/Sigook Platform

## 🎯 Value Proposition

Covenant/Sigook es una **plataforma integral de staffing y reclutamiento** que conecta agencias de personal temporal con empresas que necesitan trabajadores, manejando todo el ciclo de vida desde el reclutamiento hasta el pago.

### Problema que Resuelve

**Gestionar el flujo completo de staffing temporal/permanente incluyendo:**
- ✅ Registro y certificación de trabajadores
- ✅ Matching entre trabajadores y órdenes de trabajo
- ✅ Gestión de horarios y timesheets con punch card
- ✅ Procesamiento de nómina con impuestos canadienses complejos (CPP, EI, Federal, Provincial)
- ✅ Facturación automatizada a empresas
- ✅ Cumplimiento regulatorio (documentos, certificaciones, seguros)
- ✅ Generación de documentos legales (pay stubs, invoices)

---

## 👥 Actores Principales

### 1. AGENCY (Agencia de Personal)

**Rol:** Intermediario que conecta Companies con Workers y gestiona todo el proceso.

**Tipos de Agency:**
- **Master** - Agencia principal con sub-agencias
- **Regular** - Agencia estándar independiente
- **BusinessPartner** - Socio comercial con acceso limitado

**Responsabilidades:**
- Reclutar y aprobar Workers
- Gestionar Companies (clientes)
- Crear y asignar órdenes de trabajo (Requests)
- Aprobar timesheets
- Procesar payroll para workers
- Facturar a companies
- Mantener cumplimiento regulatorio

**Estructura:**
- Tiene ubicaciones físicas (AgencyLocation) con dirección de facturación
- Tiene personal interno (AgencyPersonnel):
  - Recruiters (reclutadores)
  - Sales Representatives (ventas)
  - Account Managers
- Tiene BusinessNumber, HstNumber (tax registration)

---

### 2. COMPANY (Empresa Cliente)

**Rol:** Cliente de la agencia que necesita personal temporal o permanente.

**Pipeline de Estados:**
```
Lead → Potential → Prospect → Quoted → Client → Blocked/Inactive
```

**Responsabilidades:**
- Definir job positions con rates (tarifas)
- Crear órdenes de trabajo (Requests)
- Revisar y aprobar candidatos
- Revisar timesheets (opcional)
- Recibir facturación por servicios

**Estructura:**
- Tiene perfil (CompanyProfile) gestionado por una Agency
- Múltiples ubicaciones (CompanyProfileLocation)
- Job positions con rates definidos (CompanyProfileJobPositionRate):
  - **WorkerRate** - Lo que se paga al worker
  - **AgencyRate** - Lo que cobra la agencia (incluye markup)
- Contactos (CompanyProfileContactPerson)
- Usuarios internos (CompanyUser) para gestionar órdenes

**Datos clave:**
- BusinessName, DbaName
- BusinessNumber, HstNumber
- Billing address y shipping addresses
- RequiresPermissionToSeeOrders (control de acceso)

---

### 3. WORKER (Trabajador)

**Rol:** Profesional que busca empleo temporal o permanente a través de la plataforma.

**Estados y Flags:**
- `ApprovedToWork` - Aprobado por la agencia para trabajar (requiere documentos completos)
- `Dnu` (Do Not Use) - Marcado como no disponible
- `IsSubcontractor` - Trabaja como subcontratista (diferentes tax rules)
- `IsContractor` - Trabaja como contratista independiente

**Responsabilidades:**
- Completar registro con información completa
- Mantener documentos vigentes (SIN, IDs, licenses, certificates)
- Aplicar a órdenes de trabajo
- Completar timesheets (clock in/out)
- Recibir pay stubs

**Estructura del Perfil (WorkerProfile):**

**Información Personal:**
- FirstName, LastName, BirthDay, Gender
- SocialInsurance (SIN) con archivo y fecha de vencimiento
- IdentificationNumber1/2 con archivos (Passport, Driver License, etc)
- ProfileImage

**Información de Contacto:**
- MobileNumber, Phone, Email
- Location (Address, City, Province, PostalCode)
- HasVehicle

**Información Profesional:**
- Skills (múltiples habilidades)
- Languages (idiomas con nivel de competencia)
- Licenses (licencias profesionales con vencimiento)
- Certificates (certificaciones con vencimiento)
- JobExperience (experiencia laboral)

**Disponibilidad:**
- AvailabilityType (FullTime, PartTime, Flexible)
- AvailabilityTime (días y horarios disponibles)
- LocationPreferences (ciudades preferidas)

**Información Fiscal:**
- TaxCategory (FederalCategory, ProvincialCategory) - Claim codes para cálculo de impuestos
- Province - Determina qué tabla de impuestos provinciales usar

---

### 4. CANDIDATE (Candidato)

**Rol:** Prospecto gestionado por la agencia que AÚN NO tiene cuenta de usuario en el sistema.

**Diferencia con Worker:**
- **Candidate** - Solo existe en el sistema de la agencia, sin User asociado
- **Worker** - Tiene User asociado (email, autenticación), puede usar la app

**Transición:**
```
Candidate (gestionado por agencia) → Worker (se registra en Flutter app)
```

**Uso:**
- Agency registra Candidates manualmente
- Agency hace seguimiento y reclutamiento
- Cuando el Candidate se registra en el sistema, se convierte en Worker

---

## 🔄 Flujo de Negocio Completo

### FASE 1: PREPARACIÓN

```
┌─────────────────────────────────────────────────────────┐
│ 1. AGENCY registra COMPANY                              │
│    - CompanyProfile con BusinessName, locations         │
│    - Job Positions con rates (worker rate, agency rate) │
│    - Contact persons y users                            │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 2. WORKER se registra vía Flutter app                   │
│    - Personal info, contact, address                    │
│    - Documents (SIN, IDs, certificates)                 │
│    - Skills, languages, experience                      │
│    - Availability (days, hours, locations)              │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 3. AGENCY aprueba WORKER                                │
│    - Revisa documentos y perfil                         │
│    - Set ApprovedToWork = true                          │
│    - Worker puede ver y aplicar a jobs                  │
└─────────────────────────────────────────────────────────┘
```

### FASE 2: CREACIÓN DE ORDEN

```
┌─────────────────────────────────────────────────────────┐
│ COMPANY o AGENCY crea REQUEST                           │
│                                                          │
│ Información clave:                                       │
│  - JobTitle, Description, Requirements                   │
│  - WorkersQuantity (cuántos necesita)                   │
│  - JobLocation (dónde se trabaja)                       │
│  - JobPositionRate (define tarifas)                     │
│  - Shift (horario: 7:00 AM - 3:00 PM)                  │
│  - DurationTerm (LongTerm/ShortTerm)                    │
│  - EmploymentType (FullTime/PartTime/Contractor)        │
│  - StartAt, FinishAt (fechas)                           │
│  - Incentive (bonus opcional)                           │
│                                                          │
│ Estados:                                                 │
│  - Requested: Recién creada                             │
│  - InProcess: En proceso de llenado                     │
│  - Cancelled: Cancelada                                 │
│  - IsOpen: Todavía acepta workers                       │
└─────────────────────────────────────────────────────────┘
```

### FASE 3: MATCHING Y ASIGNACIÓN

```
┌─────────────────────────────────────────────────────────┐
│ 1. WORKERS ven Requests en Flutter app                  │
│    GET /api/WorkerRequest/Available                     │
│    - Filtran por ciudad, job type, rate                 │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 2. WORKER aplica o AGENCY asigna                        │
│    POST /api/WorkerRequest/Apply                        │
│    POST /api/AgencyRequest/{id}/AssignWorker            │
│                                                          │
│    Crea WORKERREQUEST:                                  │
│     - Status: Booked (asignado)                         │
│     - StartWorking: Fecha de inicio                     │
│     - WeekStartWorking: Semana de inicio                │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 3. REQUEST se llena                                     │
│    WorkersQuantityWorking >= WorkersQuantity            │
│    → Request puede cerrar o seguir abierto              │
└─────────────────────────────────────────────────────────┘
```

### FASE 4: TRABAJO Y TIME TRACKING

```
┌─────────────────────────────────────────────────────────┐
│ 1. WORKER hace Clock In/Out diario                      │
│    POST /api/WorkerRequestTimeSheet/ClockIn             │
│    - ClockIn: 2026-02-01T07:05:23Z (real time)         │
│    - ClockInRounded: 2026-02-01T07:00:00Z (rounded)    │
│                                                          │
│    POST /api/WorkerRequestTimeSheet/ClockOut            │
│    - ClockOut: 2026-02-01T15:08:12Z                    │
│    - ClockOutRounded: 2026-02-01T15:00:00Z             │
│                                                          │
│    Crea TIMESHEET por cada día trabajado                │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 2. AGENCY aprueba TIMESHEET                             │
│    PUT /api/AgencyRequestTimeSheet/{id}/Approve         │
│    - TimeInApproved: 2026-02-01T07:00:00Z              │
│    - TimeOutApproved: 2026-02-01T15:00:00Z             │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 3. SISTEMA calcula TIMESHEETTOTAL                       │
│    - TotalHours = TimeOutApproved - TimeInApproved      │
│    - RegularHours (primeras 44 hrs/semana)             │
│    - OvertimeHours (después de 44 hrs)                 │
│    - NightShiftHours (11 PM - 7 AM)                    │
│    - HolidayHours (si IsHoliday = true)                │
│    - AccumulateWeekHours (suma semanal)                │
└─────────────────────────────────────────────────────────┘
```

### FASE 5: PAYROLL (NÓMINA)

```
┌─────────────────────────────────────────────────────────┐
│ 1. AGENCY selecciona workers para payroll               │
│    POST /api/v4/Accounting/PayStub                      │
│    - Worker, PaymentDate, WeekEnding                    │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 2. SISTEMA calcula EARNINGS                             │
│    - RegularHours × WorkerRate = RegularWage            │
│    - OvertimeHours × (Rate × 1.5) = OvertimeWage       │
│    - NightShiftHours × NightShiftRate                   │
│    - HolidayHours × HolidayRate                         │
│    - GrossPayment = suma de todos los wages             │
│    - Vacations = GrossPayment × 4% (mandatorio Canadá)  │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 3. SISTEMA calcula DEDUCTIONS (ver PAYROLL_RULES.md)   │
│    - CPP (Canada Pension Plan): 5.95%                   │
│    - EI (Employment Insurance): 1.66%                   │
│    - FederalTax (lookup tables)                         │
│    - ProvincialTax (por provincia)                      │
│    - TotalDeductions = suma                             │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 4. GENERA PAYSTUB                                       │
│    - PayStubNumber: PS-0001-26                          │
│    - TotalEarnings = GrossPayment + Vacations           │
│    - TotalPaid = TotalEarnings - TotalDeductions        │
│    - Genera PDF y envía al Worker                       │
└─────────────────────────────────────────────────────────┘
```

### FASE 6: BILLING (FACTURACIÓN)

```
┌─────────────────────────────────────────────────────────┐
│ 1. AGENCY genera INVOICE para COMPANY                   │
│    POST /api/v4/Accounting/Invoice                      │
│    - CompanyProfile, WeekEnding, WorkerRequests         │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 2. SISTEMA calcula INVOICE TOTALS (ver BILLING_RULES.md)│
│    Por cada Worker:                                      │
│     - RegularHours × AgencyRate                         │
│     - OvertimeHours × (AgencyRate × 1.5)               │
│     - NightShiftHours × NightShiftRate                  │
│     - HolidayHours × HolidayRate                        │
│    SubTotal = suma de todos los workers                 │
│    Vacations = SubTotal × 4%                            │
│    HST/GST = (SubTotal + Vacations) × TaxRate          │
│    TotalNet = SubTotal + Vacations + HST                │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 3. GENERA INVOICE                                       │
│    - InvoiceNumber: AI-0001-26                          │
│    - InvoiceTotals (breakdown por worker)               │
│    - Genera PDF y envía a Company recipients            │
└─────────────────────────────────────────────────────────┘
```

---

## 💰 Modelo de Ingresos (Agency)

### Revenue = Markup on Worker Rates

```
AgencyRate - WorkerRate = Agency Profit (Markup)

Ejemplo:
- AgencyRate: $25/hr (lo que cobra al Company)
- WorkerRate: $18/hr (lo que paga al Worker)
- Markup: $7/hr (28% profit margin)

Para 40 horas/semana:
- Agency cobra al Company: $1,000
- Agency paga al Worker: $720
- Agency profit: $280/semana por worker
```

### Costos de la Agency:
- Payroll processing (CPP, EI employer contributions)
- Insurance y liability
- Overhead (personal, office, software)
- Marketing y recruitment

---

## 🎯 Diferenciadores Competitivos

### 1. Automatización Completa
- Desde job posting hasta invoice generation
- Cálculos automáticos de payroll (complejos impuestos canadienses)
- Document generation (PDF pay stubs, invoices)

### 2. Multi-jurisdicción
- Canadá (CPP, EI, Federal/Provincial taxes)
- USA (Federal, State, FICA) - preparado para expansión
- Tax tables actualizadas

### 3. Mobile-First para Workers
- Flutter app nativa para iOS/Android
- Clock in/out con GPS
- Real-time job search
- Document upload

### 4. Compliance y Tracking
- Document expiry tracking
- License/certificate validation
- Audit trail completo
- Legal document generation

### 5. Cloud-Native
- Azure Storage para documentos
- Azure Service Bus para async processing
- Escalable y resiliente

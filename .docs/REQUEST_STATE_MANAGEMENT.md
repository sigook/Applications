# Request State Management System

## Documento de Referencia
**Última actualización:** 2026-01-22
**Autor:** Documentación generada durante refactorización de estados

---

## Índice
1. [Sistema Actual (Pre-Refactorización)](#sistema-actual-pre-refactorización)
2. [Nuevo Sistema (Post-Refactorización)](#nuevo-sistema-post-refactorización)
3. [Archivos Afectados - Backend](#archivos-afectados---backend)
4. [Archivos Afectados - Frontend](#archivos-afectados---frontend)
5. [Lógica de Transiciones de Estado](#lógica-de-transiciones-de-estado)
6. [Reglas de Negocio](#reglas-de-negocio)
7. [Migración de Datos](#migración-de-datos)

---

## Sistema Actual (Pre-Refactorización)

### Mecanismo Dual de Estado

El sistema usaba **DOS mecanismos** para gestionar el estado de las órdenes (Request):

1. **`RequestStatus` enum** - Estado del proceso
2. **`IsOpen` boolean** - Flag calculada automáticamente

### RequestStatus Enum (Antes)

```csharp
public enum RequestStatus
{
    Requested = 0,    // Recién creada
    InProcess = 1,    // En proceso
    Cancelled = 2,    // Cancelada
    Open = 3,         // SOLO FILTRO (no estado real)
    NoOpen = 4        // SOLO FILTRO (no estado real)
}
```

**Nota:** `Open` y `NoOpen` NO eran estados reales de la entidad, solo valores para filtros en queries.

### Propiedad IsOpen

**Ubicación:** `Covenant.Api/Covenant.Common/Entities/Request/Request.cs:46`

```csharp
public bool IsOpen { get; set; } = true;  // Inicializado en true por defecto
```

**Cálculo Automático (líneas 121-136):**

```csharp
private void UpdateIsOpen()
{
    switch (Status)
    {
        case RequestStatus.Requested:
        case RequestStatus.InProcess:
            // Abierta si aún hay capacidad disponible
            IsOpen = WorkersQuantityWorking < WorkersQuantity;
            break;
        case RequestStatus.Cancelled:
            IsOpen = false;
            break;
        default:
            IsOpen = false;
            break;
    }
}
```

**Lógica:**
- Si status = `Requested` o `InProcess` → IsOpen depende de si hay capacidad
- Si status = `Cancelled` → IsOpen = false
- `UpdateIsOpen()` se ejecutaba automáticamente al cambiar `WorkersQuantityWorking`

### Problema del Sistema Dual

- **Complejidad:** Dos fuentes de verdad (status + IsOpen)
- **Lógica distribuida:** La lógica de "disponibilidad" estaba en `UpdateIsOpen()` + validaciones dispersas
- **Frontend confuso:** Necesitaba combinar `isOpen` + `requestStatus` para mostrar estado correcto
- **Reportes complejos:** Código de 15 líneas para determinar qué mostrar (ver GenerateAgencyRequestsReport.cs:42-57)

---

## Nuevo Sistema (Post-Refactorización)

### RequestStatus Enum (Después)

```csharp
public enum RequestStatus
{
    Open = 1,        // Nueva, sin workers asignados, aceptando aplicaciones
    InProgress = 2,  // Workers asignados, aún con capacidad disponible
    Filled = 3,      // Todas las posiciones llenas
    Cancelled = 4    // Cancelada (solo desde estado Open)
}
```

### Eliminación de IsOpen

- ❌ Propiedad `IsOpen` eliminada
- ❌ Método `UpdateIsOpen()` eliminado
- ✅ Un solo estado explícito: `RequestStatus`

### Ventajas del Nuevo Sistema

- **Claridad:** Un solo campo de estado, cuatro valores explícitos
- **Simplicidad:** Lógica de transiciones centralizada en métodos de negocio
- **Mantenibilidad:** Más fácil de entender y modificar
- **UI clara:** Frontend muestra directamente el estado sin cálculos

---

## Archivos Afectados - Backend

### 1. Covenant.Common/Enums/RequestStatus.cs
**Cambios:**
- Renombrar: `Requested` → `Open`
- Renombrar: `InProcess` → `InProgress`
- Agregar: `Filled = 3`
- Actualizar: `Cancelled = 4`
- Eliminar: Valores filtro `Open` y `NoOpen` (líneas 8-9)

### 2. Covenant.Common/Entities/Request/Request.cs
**Líneas modificadas:**

| Línea | Elemento | Acción |
|-------|----------|--------|
| 46 | `IsOpen` property | ❌ Eliminar |
| 89 | `IsAvailableToApply` | ✏️ Cambiar a: `Status == Open \|\| Status == InProgress` |
| 109-117 | `WorkersQuantityWorking` setter | ✏️ Remover llamada a `UpdateIsOpen()` |
| 121-136 | `UpdateIsOpen()` method | ❌ Eliminar |
| 138-162 | `AddWorker()` | ✏️ Agregar transiciones Open→InProgress, →Filled |
| 164-173 | `RejectWorker()` | ✏️ Agregar transiciones Filled→InProgress, →Open |
| 175-181 | `PutInProcess()` | ✏️ Actualizar a usar InProgress |
| 183-195 | `Cancel()` | ✏️ Agregar validación: solo desde Open |
| 197-215 | `Open()` (reabrir) | ✏️ Transición Cancelled→Open/InProgress |

### 3. Covenant.Infrastructure/Repositories/Request/RequestRepository.cs
**Líneas modificadas:**

| Línea | Elemento | Acción |
|-------|----------|--------|
| 87 | Select `IsOpen` | ❌ Eliminar |
| 149-151 | Filtro por `IsOpen` | ✏️ Reemplazar por filtro de estados |
| 204 | `.Where(r => r.IsOpen)` | ✏️ Cambiar a `.Where(r => r.Status == Open \|\| r.Status == InProgress)` |
| 738 | `.Where(r => r.IsOpen && ...)` | ✏️ Cambiar a filtro por estados |

### 4. Covenant.Common/Models/Request/AgencyRequestListModel.cs
**Líneas modificadas:**

| Línea | Elemento | Acción |
|-------|----------|--------|
| 29 | `IsOpen` property | ❌ Eliminar |

### 5. Covenant.Documents/Services/GenerateAgencyRequestsReport.cs
**Líneas modificadas:**

| Línea | Elemento | Acción |
|-------|----------|--------|
| 42-57 | Lógica compleja IsOpen + Status | ✏️ Reemplazar con switch simple de RequestStatus |

**Antes (15 líneas):**
```csharp
if (data.IsOpen)
{
    if (data.RequestStatus == RequestStatus.Requested || data.RequestStatus == RequestStatus.InProcess)
        sheet.Cell($"J{row}").SetValue($"Open");
    else
        sheet.Cell($"J{row}").SetValue($"Not Filled");
}
else
{
    if (data.RequestStatus == RequestStatus.Cancelled)
        sheet.Cell($"J{row}").SetValue($"Cancelled");
    else if (data.WorkersQuantityWorking < data.WorkersQuantity)
        sheet.Cell($"J{row}").SetValue($"Not Filled");
    else
        sheet.Cell($"J{row}").SetValue($"Filled");
}
```

**Después (8 líneas):**
```csharp
var statusText = data.RequestStatus switch
{
    RequestStatus.Open => "Open",
    RequestStatus.InProgress => "In Progress",
    RequestStatus.Filled => "Filled",
    RequestStatus.Cancelled => "Cancelled",
    _ => "Unknown"
};
sheet.Cell($"J{row}").SetValue(statusText);
```

### 6. Covenant.Tests/Request/RequestTest.cs
**Tests modificados:**

| Línea | Test | Acción |
|-------|------|--------|
| 74-91 | `WorkersQuantityWorking()` | ✏️ Reemplazar asserts de `IsOpen` por `Status` |
| 93-112 | `IsOpen()` | ✏️ Renombrar a `StatusTransitions()` y validar estados |
| Nuevo | `CanOnlyCancelFromOpen()` | ➕ Agregar test para validar restricción de cancelación |

---

## Archivos Afectados - Frontend

### 1. Sigook.Web/src/varaibles.js (líneas 4-19)
**Antes:**
```javascript
Vue.prototype.$statusRequested = "Requested";
Vue.prototype.$statusFinalized = "Finalized";
Vue.prototype.$statusCancelled = "Cancelled";
Vue.prototype.$statusInProcess = "InProcess";
Vue.prototype.$statusOpen = "Open";
Vue.prototype.$statusFilled = "Filled";
```

**Después:**
```javascript
// Request Status (debe coincidir con backend enum)
Vue.prototype.$statusOpen = "Open";
Vue.prototype.$statusInProgress = "InProgress";
Vue.prototype.$statusFilled = "Filled";
Vue.prototype.$statusCancelled = "Cancelled";

// Display labels
Vue.prototype.$statusDisplayOpen = "Open";
Vue.prototype.$statusDisplayInProgress = "In Progress";
Vue.prototype.$statusDisplayFilled = "Filled";
Vue.prototype.$statusDisplayCancelled = "Cancelled";
```

### 2. Sigook.Web/src/components/agency_request/TableRequests.vue

**Líneas 172-179:** Eliminar lógica de `isOpen`
```vue
<!-- ANTES: Complejo -->
<i v-if="props.row.isOpen" class="fz-2 block">
  <span v-if="canEdit(props.row.status)" class="tag-yellow">
    {{ $statusOpen }}
  </span>
  <span v-else>{{ $statusNotFilled }}</span>
</i>
<!-- ... más lógica ... -->

<!-- DESPUÉS: Simple y directo -->
<span v-if="props.row.requestStatus === 'Open'" class="tag-yellow">
  {{ $statusDisplayOpen }}
</span>
<span v-else-if="props.row.requestStatus === 'InProgress'">
  {{ $statusDisplayInProgress }}
</span>
<span v-else-if="props.row.requestStatus === 'Filled'" class="tag-green">
  {{ $statusDisplayFilled }}
</span>
<span v-else-if="props.row.requestStatus === 'Cancelled'" class="tag-red">
  {{ $statusDisplayCancelled }}
</span>
```

**Líneas 260-266:** Actualizar filtro de estados
```javascript
// ANTES: 5 opciones (incluía Open/NoOpen como filtros)
statuses: [
  { id: 0, value: this.$statusDisplayRequested },
  { id: 1, value: this.$statusDisplayInProcess },
  { id: 2, value: this.$statusDisplayCancelled },
  { id: 3, value: this.$statusDisplayOpen },
  { id: 4, value: this.$statusDisplayNoOpen }
]

// DESPUÉS: 4 estados reales
statuses: [
  { id: 1, value: this.$statusDisplayOpen },
  { id: 2, value: this.$statusDisplayInProgress },
  { id: 3, value: this.$statusDisplayFilled },
  { id: 4, value: this.$statusDisplayCancelled }
]
```

### 3. Sigook.Web/src/pages/worker/Request.vue (líneas 141-151)

**Método `canApply()`:**
```javascript
// ANTES: Switch complejo con None, Requested, InProcess, Finalized
canApply() {
  let available = false;
  switch (this.request.requestStatus) {
    case this.$statusNone:
    case this.$statusRequested:
    case this.$statusInProcess:
      available = true;
      break;
    case this.$statusFinalized:
    case this.$statusCancelled:
      available = false;
      break;
  }
  return available;
}

// DESPUÉS: Lógica clara con nuevos estados
canApply() {
  return this.request.requestStatus === this.$statusOpen ||
         this.request.requestStatus === this.$statusInProgress;
}
```

### 4. Sigook.Web/src/pages/agency/Request.vue (líneas 137-147)

**⚠️ RESTRICCIÓN NUEVA - Solo Open puede cancelarse:**
```javascript
// ANTES: Requested o InProcess podían cancelarse
canEditRequest(request) {
  return (
    request.status === this.$statusRequested ||
    request.status === this.$statusInProcess
  );
}

// DESPUÉS: Solo Open puede cancelarse
canEditRequest(request) {
  return request.status === this.$statusOpen;
}
```

### 5. Sigook.Web/src/pages/company/Request.vue (líneas 188-193)

**Mismo cambio:**
```javascript
// ANTES: !Finalized && !Cancelled podían cancelarse
canEdit() {
  if (this.request.status === this.$statusFinalized) {
    return false;
  }
  return this.request.status !== this.$statusCancelled;
}

// DESPUÉS: Solo Open puede cancelarse
canEdit() {
  return this.request.status === this.$statusOpen;
}
```

### 6. Sigook.Web/src/pages/agency/WeeklyBoard.vue (líneas 95-96)

**Verificar CSS dinámico:**
```vue
<!-- CSS class: status-{estado en minúsculas} -->
<div class="dot-status" :class="'status-' + item.requestStatus.toLowerCase()"></div>
```

Debe funcionar con: `status-open`, `status-inprogress`, `status-filled`, `status-cancelled`

### 7. Sigook.Web/src/assets/scss/base.scss (líneas 1221-1276)

**Actualizar clases CSS:**
```scss
// ELIMINAR:
.status-requested { background-color: $accent; }
.status-inprocess { background-color: $primary; }
.status-finalized { background-color: $green; }
.Requested { color: $accent; }
.InProcess { color: $primary; }
.Finalized { color: $green; }

// AGREGAR/MANTENER:
.status-open { background-color: $accent; }
.status-inprogress { background-color: $primary; }
.status-filled { background-color: $green; }
.status-cancelled { background-color: $red; }

.Open { color: $accent; }
.InProgress { color: $primary; }
.Filled { color: $green; }
.Cancelled { color: $red; }
```

### 8. Sigook.Web/src/lang/*.json

**Archivos:** `en.json`, `es.json`, `fr.json`

**Agregar traducciones:**
```json
{
  "Open": "Open",
  "InProgress": "In Progress",
  "Filled": "Filled",
  "Cancelled": "Cancelled"
}
```

```json
{
  "Open": "Abierta",
  "InProgress": "En Progreso",
  "Filled": "Llena",
  "Cancelled": "Cancelada"
}
```

```json
{
  "Open": "Ouverte",
  "InProgress": "En Cours",
  "Filled": "Remplie",
  "Cancelled": "Annulée"
}
```

---

## Lógica de Transiciones de Estado

### Diagrama de Estados

```
┌────────┐   Primer worker asignado   ┌────────────┐   Capacidad llena   ┌────────┐
│  Open  │ ───────────────────────▶  │ InProgress │ ──────────────────▶ │ Filled │
└────────┘                            └────────────┘                      └────────┘
    ▲                                        ▲                                 │
    │                                        │                                 │
    │ Todos los workers                     │ Worker rechazado/                │
    │ removidos                              │ removido (aún hay               │
    │                                        │ workers)                        │
    │                                        │                                 │
    └────────────────────────────────────────┴─────────────────────────────────┘

    │
    │ Cancelar (solo desde Open)
    ▼
┌───────────┐
│ Cancelled │  ◀─── Solo se puede cancelar desde Open
└───────────┘
    │
    │ Reabrir
    ▼
Vuelve a Open (si sin workers) o InProgress (si tiene workers)
```

### Transiciones Automáticas

#### 1. Creación de Orden
```csharp
// Al crear Request
Status = RequestStatus.Open;  // Estado inicial
```

#### 2. Asignación de Worker (`AddWorker()`)
```csharp
// Si es el primer worker
if (WorkersQuantityWorking == 0 && Status == RequestStatus.Open)
    Status = RequestStatus.InProgress;

// Si se llena la capacidad
if (WorkersQuantityWorking >= WorkersQuantity)
    Status = RequestStatus.Filled;
```

#### 3. Rechazo de Worker (`RejectWorker()`)
```csharp
// Si estaba lleno y se libera espacio
if (Status == RequestStatus.Filled && WorkersQuantityWorking < WorkersQuantity)
    Status = RequestStatus.InProgress;

// Si se remueven todos los workers
if (WorkersQuantityWorking == 0 && Status == RequestStatus.InProgress)
    Status = RequestStatus.Open;
```

#### 4. Incremento de Capacidad (`IncreaseWorkersQuantityByOne()`)
```csharp
// Si estaba llena, vuelve a estar en progreso
if (Status == RequestStatus.Filled)
    Status = RequestStatus.InProgress;
```

#### 5. Cancelación (`Cancel()`)
```csharp
// ⚠️ RESTRICCIÓN: Solo desde Open
if (Status != RequestStatus.Open)
    return Result.Fail("Solo se pueden cancelar órdenes en estado Open");

Status = RequestStatus.Cancelled;
```

#### 6. Reapertura (`Open()`)
```csharp
// Solo si está cancelada
if (Status != RequestStatus.Cancelled)
    return Result.Fail("Solo se pueden reabrir órdenes canceladas");

// Decide estado según workers asignados
Status = WorkersQuantityWorking > 0
    ? RequestStatus.InProgress
    : RequestStatus.Open;
```

---

## Reglas de Negocio

### 1. Cancelación Restringida
**Regla:** Solo las órdenes en estado `Open` pueden ser canceladas.

**Justificación:**
- Si ya hay workers asignados (InProgress), cancelar afectaría a trabajadores ya comprometidos
- Si está llena (Filled), cancelar sería más complejo (liquidaciones, contratos, etc.)
- Solo órdenes sin workers pueden cancelarse limpiamente

**Impacto:**
- Backend: Validación en `Cancel()` method
- Frontend: Botón "Cancel" solo visible si `status === 'Open'`

### 2. Disponibilidad para Aplicar
**Regla:** Workers solo pueden aplicar a órdenes `Open` o `InProgress`.

**Lógica:**
```csharp
public bool IsAvailableToApply =>
    Status == RequestStatus.Open || Status == RequestStatus.InProgress;
```

### 3. Transiciones Automáticas
**Regla:** Los cambios de estado se ejecutan automáticamente al modificar workers.

**No se requiere acción manual** para:
- Open → InProgress (al asignar primer worker)
- InProgress → Filled (al llenar capacidad)
- Filled → InProgress (al rechazar worker)
- InProgress → Open (al remover todos los workers)

### 4. Reapertura Inteligente
**Regla:** Al reabrir una orden cancelada, el estado depende de si tiene workers asignados.

**Lógica:**
- Si tiene workers → `InProgress`
- Si no tiene workers → `Open`

---

## Migración de Datos

### Script SQL

```sql
-- ============================================
-- Migración de estados: IsOpen → RequestStatus
-- Fecha: 2026-01-22
-- ============================================

-- PASO 1: Agregar columna temporal para debugging (opcional)
ALTER TABLE "Request" ADD COLUMN "OldStatus" INT;
UPDATE "Request" SET "OldStatus" = "Status";

-- PASO 2: Migrar datos a nuevos estados
UPDATE "Request"
SET "Status" =
  CASE
    -- Requested sin workers → Open (1)
    WHEN "Status" = 0 AND "WorkersQuantityWorking" = 0 THEN 1

    -- InProcess sin workers → Open (1)
    WHEN "Status" = 1 AND "WorkersQuantityWorking" = 0 THEN 1

    -- InProcess con workers pero no lleno → InProgress (2)
    WHEN "Status" = 1
      AND "WorkersQuantityWorking" > 0
      AND "WorkersQuantityWorking" < "WorkersQuantity" THEN 2

    -- InProcess lleno → Filled (3)
    WHEN "Status" = 1
      AND "WorkersQuantityWorking" >= "WorkersQuantity" THEN 3

    -- Cancelled → Cancelled (4)
    WHEN "Status" = 2 THEN 4

    -- Cualquier otro caso (no debería existir)
    ELSE "Status"
  END;

-- PASO 3: Verificar migración (ejecutar ANTES de eliminar columnas)
SELECT
    "OldStatus",
    "Status" as "NewStatus",
    "IsOpen",
    "WorkersQuantityWorking",
    "WorkersQuantity",
    COUNT(*) as "Count"
FROM "Request"
GROUP BY "OldStatus", "Status", "IsOpen", "WorkersQuantityWorking", "WorkersQuantity"
ORDER BY "OldStatus", "Status";

-- PASO 4: Después de validar en staging, eliminar columnas obsoletas
-- ⚠️ EJECUTAR SOLO DESPUÉS DE VALIDAR QUE TODO FUNCIONA
ALTER TABLE "Request" DROP COLUMN "IsOpen";
ALTER TABLE "Request" DROP COLUMN "OldStatus";

-- PASO 5: Verificación final
SELECT
    "Status",
    COUNT(*) as "Total",
    AVG("WorkersQuantityWorking") as "AvgWorkers"
FROM "Request"
GROUP BY "Status"
ORDER BY "Status";
```

### Mapeo de Migración

| Estado Original | WorkersQuantityWorking | IsOpen | → | Nuevo Estado | ID |
|----------------|------------------------|--------|---|--------------|-----|
| Requested (0) | 0 | true | → | Open | 1 |
| InProcess (1) | 0 | true | → | Open | 1 |
| InProcess (1) | > 0, < Quantity | true | → | InProgress | 2 |
| InProcess (1) | >= Quantity | false | → | Filled | 3 |
| Cancelled (2) | any | false | → | Cancelled | 4 |

### Validación Pre-Migración

**Query para revisar distribución actual:**
```sql
SELECT
    CASE "Status"
        WHEN 0 THEN 'Requested'
        WHEN 1 THEN 'InProcess'
        WHEN 2 THEN 'Cancelled'
        ELSE 'Unknown'
    END as "StatusName",
    "IsOpen",
    CASE
        WHEN "WorkersQuantityWorking" = 0 THEN 'No workers'
        WHEN "WorkersQuantityWorking" < "WorkersQuantity" THEN 'Partial'
        WHEN "WorkersQuantityWorking" >= "WorkersQuantity" THEN 'Full'
    END as "Capacity",
    COUNT(*) as "Count"
FROM "Request"
GROUP BY "Status", "IsOpen", "Capacity"
ORDER BY "Status", "IsOpen", "Capacity";
```

### Checklist de Migración

- [ ] Backup de base de datos
- [ ] Ejecutar query de validación pre-migración
- [ ] Ejecutar PASO 1 y 2 del script (agregar columna temporal y migrar)
- [ ] Ejecutar PASO 3 (verificación)
- [ ] Deploy código backend actualizado
- [ ] Deploy código frontend actualizado
- [ ] Testing en staging (crear orden, asignar workers, cancelar, etc.)
- [ ] Validar reportes y filtros
- [ ] Ejecutar PASO 4 (eliminar columnas IsOpen y OldStatus)
- [ ] Ejecutar PASO 5 (verificación final)
- [ ] Deploy a producción
- [ ] Monitoreo post-deploy (logs, errores, comportamiento)

---

## Notas Adicionales

### Test Coverage

Los siguientes tests validan el comportamiento de estados:

**Archivo:** `Covenant.Tests/Request/RequestTest.cs`
- `WorkersQuantityWorking()` - Valida transiciones automáticas al agregar/remover workers
- `StatusTransitions()` (antes: `IsOpen()`) - Valida todos los estados y transiciones
- `CanOnlyCancelFromOpen()` (nuevo) - Valida restricción de cancelación

### Endpoints Afectados

**Covenant.Api:**
- `GET /api/AgencyRequest` - Lista de órdenes (ya no devuelve IsOpen)
- `GET /api/AgencyRequest/{id}` - Detalle de orden
- `POST /api/AgencyRequest/{requestId}/Worker/{workerId}/Book` - Asignar worker (transición automática)
- `PUT /api/AgencyRequest/{id}/Cancel` - Cancelar (solo si Open)
- `PUT /api/AgencyRequest/{id}/Open` - Reabrir
- `GET /api/CompanyRequest` - Similar para companies

### Consideraciones de Rollback

Si es necesario hacer rollback:
1. Revertir código backend y frontend a versiones anteriores
2. Restaurar columna `IsOpen` desde backup
3. No es necesario revertir datos de Status (son compatibles hacia atrás)

### Performance

No hay impacto significativo de performance:
- Se elimina una columna de la tabla Request (reduce tamaño)
- Queries filtran por Status en lugar de IsOpen (mismo índice)
- Se reduce complejidad de lógica (menos operaciones)

---

## Referencias

- **Código fuente:** `Covenant.Api/Covenant.Common/Entities/Request/Request.cs`
- **Tests:** `Covenant.Tests/Request/RequestTest.cs`
- **Documentación de negocio:** `.docs/BUSINESS_MODEL.md` (líneas 179-228)
- **Workflows:** `.docs/WORKFLOWS.md` (líneas 394-593)

---

**Fin del Documento**

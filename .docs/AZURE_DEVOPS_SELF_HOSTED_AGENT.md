# Azure DevOps Self-Hosted Agent

Esta documentación describe la configuración y uso del self-hosted agent de Azure DevOps para los pipelines de Covenant/Sigook.

## Índice

1. [Resumen](#resumen)
2. [Especificaciones de la VM](#especificaciones-de-la-vm)
3. [Configuración Inicial](#configuración-inicial)
4. [Instalación del Agent](#instalación-del-agent)
5. [Troubleshooting](#troubleshooting)
6. [Mantenimiento](#mantenimiento)
7. [Próximos Pasos](#próximos-pasos)

---

## Resumen

**Objetivo:** Migrar los 7 pipelines de Azure DevOps desde hosted agents de Microsoft a un self-hosted agent corriendo en una VM de Azure.

**Beneficios:**
- Control total sobre el entorno de build
- Cache persistente entre builds (node_modules, Gradle, Docker layers)
- Builds subsecuentes ~30-50% más rápidos
- Escalabilidad cuando sea necesario

**Configuración Actual:**
- **VM**: Standard B2s (2 vCPUs, 4GB RAM)
- **OS**: Ubuntu Server 24.04 LTS - x64 Gen2
- **Pool**: covenant-build-pool
- **Agent Name**: covenant-agent-1
- **Organization**: sigook
- **Costo estimado**: ~$137/mes

**Ubicaciones:**
- **VM Name**: covenant-build-vm (o sigook-build-vm según configuración real)
- **Resource Group**: covenant-build-infrastructure
- **Work Directory**: /mnt/builds/agent-1/_work
- **Agent Directory**: ~/agents/agent-1/

---

## Especificaciones de la VM

### Configuración de Azure

**Basics:**
- Subscription: (tu subscripción)
- Resource Group: `covenant-build-infrastructure`
- VM name: `covenant-build-vm`
- Region: `East US` (o tu región)
- Image: `Ubuntu Server 24.04 LTS - x64 Gen2`
- Size: **Standard_B2s** (2 vCPUs, 4GB RAM)
- Authentication: SSH public key
  - Username: `azureuser`
  - Key pair: descargada como .pem file

**Disks:**
- OS disk type: `Premium SSD (locally-redundant storage)`
- OS disk size: `256 GiB` (P15)
- Data disk: `512 GiB` (P20) - para workspaces de builds

**Networking:**
- Virtual network: `covenant-build-vnet`
- Public IP: Standard, Static
- Inbound ports: SSH (22)

**Management:**
- System assigned managed identity: Enabled
- Boot diagnostics: Enabled

### Costos Mensuales Estimados

| Recurso | Costo Mensual |
|---------|---------------|
| VM Standard_B2s | ~$41.20 |
| Disco OS P15 (256GB) | ~$19.71 |
| Disco Data P20 (512GB) | ~$72.97 |
| Public IP | ~$3.50 |
| **TOTAL** | **~$137.38** |

**⚠️ Nota sobre Serie B (Burstable):**
- La VM B2s usa "créditos de CPU" que se regeneran con el tiempo
- Builds intensivos consumen créditos más rápido de lo que se regeneran
- Si se agotan, el performance se reduce significativamente
- **Solución:** Escalar a D2s v5 (~$176/mes) para performance garantizada sin créditos

---

## Configuración Inicial

### 1. Conectar a la VM vía SSH

**Desde Windows (PowerShell):**

La clave SSH descargada de Azure (.pem) requiere permisos específicos en Windows:

```powershell
# Navegar a la carpeta donde está la clave
cd C:\Src\Covenant

# Ajustar permisos (IMPORTANTE en Windows)
$keyFile = ".\sigook-build-vm_key.pem"
icacls $keyFile /inheritance:r
icacls $keyFile /grant:r "$($env:USERNAME):(R)"

# Conectar
ssh -i sigook-build-vm_key.pem azureuser@<PUBLIC_IP>
```

**Verificar IP pública:**
- Azure Portal → Tu VM → Overview → Public IP address

### 2. Solucionar Problema de DNS

**Problema:** La VM con Ubuntu 24.04 LTS puede tener problemas para resolver nombres de dominio externos (como `vstsagentpackage.azureedge.net`).

**Síntoma:**
```bash
wget https://vstsagentpackage.azureedge.net/...
# Error: Resolving vstsagentpackage.azureedge.net ... failed: Name or service not known
```

**Solución:** Configurar DNS servers de Google en systemd-resolved:

```bash
# Crear archivo de configuración DNS
sudo mkdir -p /etc/systemd/resolved.conf.d/
sudo bash -c 'cat > /etc/systemd/resolved.conf.d/dns.conf << EOF
[Resolve]
DNS=8.8.8.8 8.8.4.4 1.1.1.1
FallbackDNS=168.63.129.16
EOF'

# Reiniciar servicio DNS
sudo systemctl restart systemd-resolved

# Verificar que funcione
resolvectl status
ping google.com
```

**Explicación:**
- `8.8.8.8`, `8.8.4.4`: Google DNS
- `1.1.1.1`: Cloudflare DNS
- `168.63.129.16`: Azure internal DNS (fallback)

### 3. Configurar Disco de Datos

**⚠️ IMPORTANTE:** Este paso es **OBLIGATORIO** antes de configurar el agente. El disco de datos de 512GB debe montarse en `/mnt/builds` para almacenar workspaces de builds.

```bash
# 1. Verificar disco de datos
lsblk
# Deberías ver /dev/sdc con 512GB (o /dev/sdd si hay múltiples discos)

# 2. Particionar y formatear el disco
# IMPORTANTE: Verifica que /dev/sdc sea el disco correcto antes de ejecutar
sudo parted /dev/sdc --script mklabel gpt
sudo parted /dev/sdc --script mkpart primary ext4 0% 100%
sudo mkfs.ext4 /dev/sdc1

# 3. Obtener UUID del disco
sudo blkid /dev/sdc1
# Ejemplo de output: /dev/sdc1: UUID="a1b2c3d4-..." TYPE="ext4"
# Copia el UUID completo

# 4. Crear punto de montaje
sudo mkdir -p /mnt/builds

# 5. Configurar montaje automático en /etc/fstab
# REEMPLAZA <tu-uuid> con el UUID obtenido en el paso 3
echo "UUID=<tu-uuid>  /mnt/builds  ext4  defaults,nofail  0  2" | sudo tee -a /etc/fstab

# 6. Montar el disco
sudo mount -a

# 7. Verificar que se montó correctamente
df -h /mnt/builds
# Deberías ver ~512GB disponible

# 8. Crear estructura de directorios para el agente
sudo mkdir -p /mnt/builds/agent-1/_work

# 9. ⚠️ CRÍTICO: Asignar permisos al usuario azureuser
sudo chown -R azureuser:azureuser /mnt/builds

# 10. Verificar permisos
ls -la /mnt/builds
# Debe mostrar: drwxr-xr-x ... azureuser azureuser ... agent-1
```

**Verificación final:**
```bash
# El usuario azureuser debe poder escribir en el directorio
touch /mnt/builds/test.txt && rm /mnt/builds/test.txt
# Si no hay error, los permisos están correctos
```

---

## Instalación del Agent

### 1. Crear Agent Pool en Azure DevOps

1. **Azure DevOps** → Organization Settings (esquina inferior izquierda)
2. **Pipelines** → Agent pools
3. **Click** "Add pool"
4. **Configurar:**
   - Pool type: **Self-hosted**
   - Name: **covenant-build-pool**
   - Grant access permission to all pipelines: ☑️
   - Auto-provision this agent pool in all projects: ☑️
5. **Click** "Create"

### 2. Crear Personal Access Token (PAT)

1. **Azure DevOps** → Click en tu avatar → Personal Access Tokens
2. **Click** "New Token"
3. **Configurar:**
   - Name: `covenant-build-agents`
   - Expiration: **1 year** (365 days)
   - Scopes: **Custom defined**
     - Agent Pools: ☑️ Read & manage
     - Build: ☑️ Read & execute
     - Code: ☑️ Read
4. **Click** "Create"
5. **IMPORTANTE**: Copia el token inmediatamente (no se podrá ver de nuevo)

### 3. Descargar e Instalar el Agent

**Obtener URL de descarga desde Azure DevOps:**

1. **Azure DevOps** → Organization Settings → Agent pools
2. Click en **covenant-build-pool**
3. Pestaña **Agents**
4. Click en **New agent**
5. Seleccionar **Linux** → **x64**
6. Azure DevOps mostrará la URL de descarga actualizada y las instrucciones

**Instalar en la VM:**

```bash
# Crear directorio
mkdir -p ~/agents/agent-1
cd ~/agents/agent-1

# Descargar agent (usa la URL proporcionada por Azure DevOps)
# Ejemplo:
wget https://vstsagentpackage.azureedge.net/agent/<version>/vsts-agent-linux-x64-<version>.tar.gz

# Extraer
tar zxvf vsts-agent-linux-x64-*.tar.gz

# Configurar
./config.sh
```

**Respuestas durante la configuración:**

```
Server URL: https://dev.azure.com/sigook
Authentication type: [Enter] (PAT)
Personal access token: [Pega tu PAT aquí]
Agent pool: covenant-build-pool
Agent name: covenant-agent-1
Work folder: /mnt/builds/agent-1/_work
Run agent as service? (Y/N): Y
User account: [Enter] (azureuser)
```

### 4. Gestionar el Servicio

**Nombre del servicio:**
```
vsts.agent.sigook.covenant-build-pool.covenant-agent-1.service
```

**Comandos útiles:**

```bash
# Ver estado
sudo systemctl status vsts.agent.sigook.covenant-build-pool.covenant-agent-1.service

# Iniciar
sudo ./svc.sh start

# Detener
sudo ./svc.sh stop

# Habilitar inicio automático
sudo systemctl enable vsts.agent.sigook.covenant-build-pool.covenant-agent-1.service

# Ver logs en tiempo real
sudo journalctl -u vsts.agent.sigook.covenant-build-pool.covenant-agent-1.service -f
```

**Verificación:**

El servicio debe mostrar:
```
Active: active (running)
...
Listening for Jobs
```

**En Azure DevOps:**
1. Organization Settings → Agent pools
2. Click en "covenant-build-pool"
3. Pestaña "Agents"
4. Deberías ver `covenant-agent-1` con status **Online** (círculo verde)

---

## Troubleshooting

### Problema: SSH "Bad permissions" en Windows

**Error:**
```
Permissions for 'C:\Src\Covenant\sigook-build-vm_key.pem' are too open
```

**Solución:**
```powershell
$keyFile = "C:\Src\Covenant\sigook-build-vm_key.pem"
icacls $keyFile /inheritance:r
icacls $keyFile /grant:r "$($env:USERNAME):(R)"
```

### Problema: DNS no resuelve dominios externos

**Error:**
```
Resolving vstsagentpackage.azureedge.net ... failed: Name or service not known
```

**Solución:** Ver sección [Solucionar Problema de DNS](#2-solucionar-problema-de-dns)

### Problema: Agent aparece Offline en Azure DevOps

**Diagnóstico:**

```bash
# 1. Verificar servicio
sudo systemctl status vsts.agent.sigook.covenant-build-pool.covenant-agent-1.service

# 2. Ver logs
sudo journalctl -u vsts.agent.sigook.covenant-build-pool.covenant-agent-1.service -n 50

# 3. Verificar conectividad
ping dev.azure.com
curl https://dev.azure.com/sigook/_apis/projects?api-version=6.0
```

**Soluciones:**

```bash
# Reiniciar servicio
cd ~/agents/agent-1
sudo ./svc.sh stop
sudo ./svc.sh start

# O con systemctl
sudo systemctl restart vsts.agent.sigook.covenant-build-pool.covenant-agent-1.service
```

### Problema: Servicio no existe después de ./config.sh

**Causa:** El servicio no se instaló correctamente.

**Solución:**

```bash
cd ~/agents/agent-1

# Desinstalar (si existe)
sudo ./svc.sh uninstall

# Instalar
sudo ./svc.sh install

# Habilitar inicio automático
sudo systemctl enable vsts.agent.sigook.covenant-build-pool.covenant-agent-1.service

# Iniciar
sudo ./svc.sh start

# Verificar
sudo systemctl status vsts.agent.sigook.covenant-build-pool.covenant-agent-1.service
```

### Problema: "Agent pool not found"

**Error:**
```
Failed to connect. VSS.Server.WebRequestException: Agent pool not found: 'sigook-build-pool'
```

**Causa:** Pool name incorrecto.

**Solución:**
1. Verificar nombre correcto del pool en Azure DevOps
2. Desconfigurar agent: `./config.sh remove`
3. Reconfigurar con nombre correcto: `./config.sh`

### Problema: "Access to the path '/mnt/builds/agent-1/_work/_tool' is denied"

**Error completo:**
```
##[error]Error reported in diagnostic logs. Please examine the log for more details.
System.UnauthorizedAccessException: Access to the path '/mnt/builds/agent-1/_work/_tool' is denied.
 ---> System.IO.IOException: Permission denied
```

**Causa:** El usuario `azureuser` no tiene permisos de escritura en el directorio `/mnt/builds/`.

**Diagnóstico:**
```bash
# Verificar propietario actual del directorio
ls -la /mnt/builds

# Verificar si el disco está montado
df -h /mnt/builds

# Intentar crear un archivo de prueba
touch /mnt/builds/test.txt
# Si falla con "Permission denied", el problema es de permisos
```

**Solución:**
```bash
# 1. Asignar permisos al usuario azureuser
sudo chown -R azureuser:azureuser /mnt/builds

# 2. Verificar permisos
ls -la /mnt/builds
# Debe mostrar: drwxr-xr-x ... azureuser azureuser

# 3. Reiniciar el servicio del agente
cd ~/agents/agent-1
sudo ./svc.sh restart

# 4. Verificar estado
sudo systemctl status vsts.agent.sigook.covenant-build-pool.covenant-agent-1.service
```

**Nota:** Si el directorio `/mnt/builds` no existe o el disco no está montado, sigue los pasos de [Configurar Disco de Datos](#3-configurar-disco-de-datos) primero.

---

## Mantenimiento

### Verificación de Estado

**Script de monitoreo rápido:**

```bash
# Crear ~/check-agent.sh
cat > ~/check-agent.sh << 'EOF'
#!/bin/bash
echo "=========================================="
echo "Estado del Build Agent - $(date)"
echo "=========================================="

# Estado del servicio
echo ""
echo ">>> Estado del servicio:"
sudo systemctl status vsts.agent.sigook.covenant-build-pool.covenant-agent-1.service | grep "Active:"

# Recursos
echo ""
echo ">>> Recursos del sistema:"
echo -n "CPU: "
top -bn1 | grep "Cpu(s)" | sed "s/.*, *\([0-9.]*\)%* id.*/\1/" | awk '{print 100 - $1"%"}'

echo -n "RAM: "
free -h | awk '/^Mem:/ {print $3 " / " $2 " (" int($3/$2 * 100) "%)"}'

echo ""
echo "Disco:"
df -h / /mnt/builds | grep -v "Filesystem"

echo ""
echo "=========================================="
EOF

chmod +x ~/check-agent.sh
```

**Ejecutar:**
```bash
./check-agent.sh
```

### Actualizar Agent

Cuando Azure DevOps notifique una nueva versión:

```bash
cd ~/agents/agent-1

# Detener servicio
sudo ./svc.sh stop

# Obtener URL de la nueva versión desde Azure DevOps
# (Organization Settings → Agent pools → covenant-build-pool → New agent)

# Descargar nueva versión
wget <URL-desde-Azure-DevOps>

# Extraer (sobrescribe archivos)
tar zxvf vsts-agent-linux-x64-*.tar.gz

# Reiniciar servicio
sudo ./svc.sh start
```

### Limpieza de Espacio (Futuro)

**Pendiente:** Script de limpieza automática con cron para prevenir que el disco se llene:
- Limpiar Docker (imágenes, contenedores)
- Limpiar Gradle cache (>30 días)
- Limpiar Flutter cache
- Limpiar node_modules antiguos
- Limpiar directorios de trabajo antiguos

---

## Próximos Pasos

### 1. Instalar Dependencias de Software

**Pendiente:** Ejecutar script de instalación para:
- Docker
- Node.js 16 y 20 (via NVM)
- .NET SDK 6.0.400, 6.0.420, 8.0.415
- Flutter SDK
- Android SDK/NDK
- Gradle
- Azure CLI

Ver plan completo en: `C:\Users\SEBASTIAN\.claude\plans\silly-puzzling-bachman.md`

### 2. Migrar Pipelines (Gradual)

**Orden de migración:**

**Semana 1:**
- ✅ covenant-web-pipeline.yml (más simple, validación de infraestructura)

**Semana 2:**
- ✅ covenant-common-nuget-pipeline.yml
- ✅ sigookfunctions-pipeline.yml

**Semana 3:**
- ✅ covenant-api-pipeline.yml
- ✅ covenant-identityserver-pipeline.yml

**Semana 4:**
- ✅ sigook-web-pipeline.yml
- ✅ sigookapp-pipeline.yml (Android stages solamente, iOS mantiene hosted macOS)

**Cambios necesarios en cada pipeline:**

```yaml
# ANTES:
pool:
  vmImage: 'ubuntu-22.04'

# DESPUÉS:
pool:
  name: 'covenant-build-pool'
```

**Para pipelines con Node.js (covenant-web, sigook-web):**
- Eliminar tarea `NodeTool@0`
- Agregar script de NVM para seleccionar versión correcta

### 3. Configurar Limpieza Automática

Script de cron para ejecutar semanalmente y limpiar cachés/builds antiguos.

### 4. Monitorear y Optimizar

- Monitorear tiempo de builds
- Monitorear CPU credits (Serie B)
- Evaluar si necesita escalar a D2s v5 o D4s v5 (más agents)

---

## Referencias

- **Plan Completo de Migración:** `C:\Users\SEBASTIAN\.claude\plans\silly-puzzling-bachman.md`
- **Documentación de Azure DevOps Agents:** https://docs.microsoft.com/azure/devops/pipelines/agents/agents
- **Pipelines del Repositorio:** `.azure-pipelines/*.yml`

---

**Última actualización:** 2026-02-03
**Estado:** Agent instalado, disco de datos configurado, y funcionando correctamente. Pendiente: instalación de dependencias y migración de pipelines.

# 🏢 Modernización de Sistema Legacy .NET - Sistema de Gestión de Órdenes de Venta

## 📋 Descripción General

Este proyecto demuestra una **modernización completa** de un sistema legacy basado en .NET Framework hacia una arquitectura moderna con Clean Architecture y CQRS usando .NET 8.0.

### Contexto del Proyecto

**Problema Original:**
- Aplicación legacy basada en ASP.NET MVC 5 (.NET Framework 4.7)
- Servicios WCF acoplados para comunicación entre módulos
- Acceso a datos mediante ADO.NET y procedimientos almacenados
- Windows Services para tareas en segundo plano sin monitoreo
- Falta de escalabilidad, mantenibilidad y testabilidad

**Solución Propuesta:**
- Arquitectura moderna **Clean Architecture** con separación de responsabilidades
- Patrón **CQRS** para separación de lectura y escritura
- **Entity Framework Core 8.0** como ORM moderna
- **JWT** para autenticación segura con roles
- **REST API** con documentación **Swagger**
- **Simulación de componentes legacy** (WCF, Windows Service) para demostrar conocimiento de migración

---

## 🏗️ Arquitectura

### Capas del Proyecto

```
src/
├── Domain/                    # Capa de dominio (modelos, interfaces)
│   ├── Entities/              # Orden, DetalleOrden
│   ├── Exceptions/            # Excepciones de dominio
│   └── Repositories/          # Interfaces de repositorios
│
├── Application/               # Capa de aplicación (casos de uso)
│   ├── DTOs/                  # Mapeos de datos
│   ├── CQRS/                  # Commands y Queries (MediatR)
│   ├── Validators/            # Validaciones (FluentValidation)
│   └── Mapping/               # AutoMapper profiles
│
├── Infrastructure/            # Capa de infraestructura (acceso a datos)
│   ├── Data/                  # DbContext (EF Core)
│   └── Repositories/          # Implementación de repositorios
│
├── WebAPI/                    # Capa de API REST
│   ├── Controllers/           # Controladores REST con Swagger
│   ├── appsettings.json       # Configuración
│   └── Program.cs             # Configuración de DI
│
├── Frontend/                  # Capa de presentación (ASP.NET MVC Core)
│   ├── Controllers
│   ├── Models
│   ├── Services
│   ├── Views
│   ├── appsettings.json  
│   └── Program.cs 
│
└── LegacySimulation/          # Simulación de componentes legacy
    ├── WcfService/            # Simulación de servicio WCF
    └── WindowsServiceSim/     # Simulación de Windows Service
```

### Diagrama de Arquitectura

```
┌─────────────────────────────────────────────────────┐
│            CLIENTE (ASP .NET MVC)                   │
│  Aplicacion web con ASP.NET MVC Core                │
└────────────┬────────────────────────────────────────┘
             │ HTTP/HTTPS
             ▼
┌─────────────────────────────────────────────────────┐
│            REST API (WebAPI)                        │
│  - Controladores REST                               │
│  - Autenticación JWT                                │
│  - Swagger Documentation                            │
└────────────┬────────────────────────────────────────┘
             │ MediatR (CQRS)
     ┌───────┴───────┐
     ▼               ▼
┌──────────────┐  ┌──────────────┐
│ Commands     │  │ Queries      │
│ (Escritura)  │  │ (Lectura)    │
└────────┬─────┘  └────────┬─────┘
         │                 │
         └────────┬────────┘
                  ▼
         ┌─────────────────┐
         │ Application     │
         │ Layer           │
         │ - Validadores   │
         │ - DTOs          │
         │ - Mappers       │
         └────────┬────────┘
                  │
         ┌─────────────────┐
         │ Domain Layer    │
         │ - Entidades     │
         │ - Lógica Negocio│
         └────────┬────────┘
                  │
         ┌─────────────────┐
         │ Infrastructure  │
         │ - DbContext (EF)│
         │ - Repositorios  │
         └────────┬────────┘
                  │
                  ▼
         ┌─────────────────┐
         │ Persistencia    │
         │ SQL Server/DB   │
         └─────────────────┘

LEGACY SIMULATION (Demostración de Migración):
┌─────────────────────────────────────────────┐
│ WCF Service Simulation                      │
│ - Contrato de servicio                      │
│ - Almacenamiento en memoria                 │
└─────────────────────────────────────────────┘

┌─────────────────────────────────────────────┐
│ Windows Service Simulation                  │
│ - Tareas en segundo plano cada 30 seg       │
│ - Procesamiento de órdenes                  │
└─────────────────────────────────────────────┘
```

---

## 🚀 Tecnologías

| Capa | Tecnología | Versión | Propósito |
|------|-----------|---------|----------|
| **Runtime** | .NET SDK | 8.0.416 | Plataforma de ejecución moderna |
| **ORM** | Entity Framework Core | 8.0.1 | Acceso a datos y migraciones |
| **CQRS** | MediatR | 12.3.0 | Patrón de separación de lectura/escritura |
| **Validación** | FluentValidation | 11.10.0 | Validaciones complejas |
| **Mapeo** | AutoMapper | 12.0.1 | Transformación entre modelos |
| **Autenticación** | JWT Bearer | 8.0.1 | Seguridad basada en tokens |
| **API Docs** | Swagger/Swashbuckle | 6.6.2 | Documentación automática |
| **Base Datos** | SQL Server | Persistencia de datos |

---

## ⚙️ Configuración Inicial

### 1. Clonar el Repositorio

```bash
git clone https://github.com/cemado/SalesOrderManagement
cd SalesOrderManagement
```

### 2. Restaurar Dependencias

```bash
dotnet restore
```

### 3. Configurar la Base de Datos

#### Usar SQL Server (Recomendado para desarrollo)

```bash
# La conexión por defecto en appsettings.json es:
# "DefaultConnection": "Server=.\\SQLEXPRESS01;Database=SalesOrderManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

Editar `src/WebAPI/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS01;Database=SalesOrderManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 4. Crear Migraciones (Generar Schema)

```bash
cd src/Infrastructure
dotnet ef migrations add InitialCreate
dotnet ef database update
```

O desde la carpeta raíz:

```bash
dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/WebAPI
dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI
```

### 5. Compilar la Solución

```bash
dotnet build
```

---

## 🚀 Ejecución

### Opción 1: Ejecutar WebAPI + Frontend

```bash
# En una terminal para la WebAPI
cd src/WebAPI
dotnet build
dotnet run --urls "https://localhost:5001"

# En otra terminal para el Frontend
cd src/Frontend
dotnet run --urls "https://localhost:5002"
```

La WebAPI estará disponible en: `https://localhost:5001`  
Swagger UI disponible en: `https://localhost:5001/index.html`
El frontend estará disponible en: `https://localhost:5002`  


### Opción 2: Ejecutar Windows Service Simulation

```bash
cd src/LegacySimulation/WindowsServiceSim
dotnet run
```

Verá la simulación procesando órdenes cada 30 segundos.

### Opción 3: Compilar en Release

```bash
dotnet build -c Release
dotnet run --configuration Release
```

---

## 🔐 Autenticación JWT

### Usuarios de Prueba

| Usuario | Contraseña | Rol | Permisos |
|---------|-----------|-----|----------|
| `admin@test.com` | `admin123` | Admin | CRUD completo |
| `vendedor@test.com` | `vendedor123` | Vendedor | Lectura y Creación |

### Obtener Token JWT

```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin@test.com","password":"admin123"}'
```

Respuesta:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Usar Token en Peticiones

```bash
curl -X GET "https://localhost:5001/api/ordenes" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

---

## 📡 Endpoints REST API

### Autenticación

| Método | Endpoint | Descripción | Acceso |
|--------|----------|-------------|--------|
| POST | `/api/auth/login` | Generar token JWT | Público |

### Órdenes

| Método | Endpoint | Descripción | Acceso | Rol Requerido |
|--------|----------|-------------|--------|---------------|
| GET | `/api/ordenes` | Listar órdenes (paginado) | Protegido | Cualquiera |
| GET | `/api/ordenes/{id}` | Obtener orden por ID | Protegido | Cualquiera |
| POST | `/api/ordenes` | Crear nueva orden | Protegido | Admin, Vendedor |
| PUT | `/api/ordenes/{id}` | Actualizar orden | Protegido | Admin |
| DELETE | `/api/ordenes/{id}` | Eliminar orden | Protegido | Admin |

### Ejemplos de Uso

#### Listar Órdenes

```bash
curl -X GET "https://localhost:5001/api/ordenes?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

#### Crear Orden

```bash
curl -X POST "https://localhost:5001/api/ordenes" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "fecha": "2025-01-15",
    "cliente": "Empresa XYZ",
    "detalles": [
      {
        "producto": "Producto A",
        "cantidad": 10,
        "precioUnitario": 100.50
      },
      {
        "producto": "Producto B",
        "cantidad": 5,
        "precioUnitario": 200.00
      }
    ]
  }'
```

#### Actualizar Orden

```bash
curl -X PUT "https://localhost:5001/api/ordenes/1" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "cliente": "Empresa XYZ Actualizado",
    "estado": "En Proceso"
  }'
```

#### Eliminar Orden

```bash
curl -X DELETE "https://localhost:5001/api/ordenes/1" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## 📊 Modelo de Datos

### Tabla: Ordenes

```sql
CREATE TABLE Ordenes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Fecha DATETIME NOT NULL,
    Cliente NVARCHAR(100) NOT NULL,
    Total DECIMAL(18,2) NOT NULL,
    Estado NVARCHAR(50) NOT NULL DEFAULT 'Pendiente'
);
```

### Tabla: DetalleOrdenes

```sql
CREATE TABLE DetalleOrdenes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrdenId INT NOT NULL,
    Producto NVARCHAR(100) NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (OrdenId) REFERENCES Ordenes(Id) ON DELETE CASCADE
);
```

---

## ✅ Reglas de Negocio

1. **Una orden debe tener al menos un detalle** ✓
2. **Cantidad y precio unitario no pueden ser negativos** ✓
3. **El total se calcula como suma de subtotales** ✓
4. **No se permite registrar dos órdenes del mismo cliente en la misma fecha** ✓
5. **Solo Admin puede actualizar y eliminar** ✓
6. **Admin y Vendedor pueden crear** ✓

---

## 🔍 Componentes Legacy (Simulación)

### 1. WCF Service Simulation

Ubicación: `src/LegacySimulation/WcfService/`

**Propósito:** Demostrar conocimiento de patrones legacy y cómo se migran.

```csharp
// Interface (simulando [ServiceContract])
public interface IOrdenService
{
    int RegistrarOrden(OrdenWcfDto orden);
    OrdenWcfDto ObtenerOrden(int id);
    List<OrdenWcfDto> ListarOrdenes();
    bool ActualizarOrden(OrdenWcfDto orden);
    bool EliminarOrden(int id);
}

// Implementation (simulando [ServiceBehavior])
public class OrdenService : IOrdenService
{
    private static readonly Dictionary<int, OrdenWcfDto> _ordenes = new();
    // ... implementación con sincronización
}
```

**Características legacy observadas:**
- Comunicación SOAP/XML (simulada)
- Almacenamiento en memoria estático
- Falta de inversión de control (IoC)
- Sincronización manual con `lock()`
- Acoplamiento fuerte

**Cómo se migra a REST:**
1. Reemplazar `[ServiceContract]` → Controlador REST
2. Reemplazar `IOrdenService` → Interface de Repositorio
3. Reemplazar almacenamiento estático → Entity Framework Core
4. Agregar inyección de dependencias
5. Usar Commands/Queries (CQRS) en lugar de métodos directos

### 2. Windows Service Simulation

Ubicación: `src/LegacySimulation/WindowsServiceSim/Program.cs`

**Propósito:** Simular tareas en segundo plano sin requerir instalación como servicio real.

```csharp
// Legacy approach (Windows Service inheriting from ServiceBase)
public class OrderProcessor : ServiceBase
{
    // OnStart(), OnStop(), etc.
}

// Modern approach (usando PeriodicTimer)
using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
while (await timer.WaitForNextTickAsync())
{
    await ProcesarOrdenesAsync();
}
```

**Características legacy:**
- Ejecución de fondo sin logging adecuado
- Sin escalabilidad
- Difícil de testear
- Requiere instalación como servicio

**Cómo se migra:**
1. Usar **PeriodicTimer** o **BackgroundService** de ASP.NET Core
2. Agregar logging con **Serilog** o **ILogger**
3. Implementar **Health Checks**
4. Desplegar como **Azure Functions** o **Kubernetes CronJob**
5. Usar **Message Queue** (RabbitMQ, Azure Service Bus) para tareas distribuidas

---

## 🧪 Pruebas

### Prueba Manual con Swagger

1. Ir a: `https://localhost:5001/index.html`
2. Hacer clic en "Authorize"
3. Ingresar token JWT obtenido del endpoint `/api/auth/login`
4. Ejecutar endpoints desde UI interactiva

### Prueba con cURL

```bash
# 1. Login
TOKEN=$(curl -s -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin@test.com","password":"admin123"}' \
  | jq -r '.token')

# 2. Crear orden
curl -X POST "https://localhost:5001/api/ordenes" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "fecha": "2025-01-15",
    "cliente": "Test Cliente",
    "detalles": [{"producto": "Test Producto", "cantidad": 1, "precioUnitario": 100}]
  }'

# 3. Listar órdenes
curl -X GET "https://localhost:5001/api/ordenes" \
  -H "Authorization: Bearer $TOKEN"
```

---

## 📚 Patrón de Diseño: CQRS

Este proyecto implementa **Command Query Responsibility Segregation (CQRS)**:

### Commands (Escritura/Cambios)
```csharp
// Crear orden
var command = new CrearOrdenCommand { Orden = request };
var resultado = await _mediator.Send(command);
```

### Queries (Lectura)
```csharp
// Consultar órdenes
var query = new GetOrdenesQuery { PageNumber = 1, PageSize = 10 };
var resultado = await _mediator.Send(query);
```

**Beneficios:**
- ✅ Escalabilidad independiente de lectura/escritura
- ✅ Mejor rendimiento con cachés en lecturas
- ✅ Validaciones específicas por operación
- ✅ Auditoría más fácil
- ✅ Testabilidad mejorada

---

## 🎓 Conceptos SOLID Aplicados

### 1. Single Responsibility (SRP)
- `OrdenRepository`: Solo responsable de acceso a datos
- `CrearOrdenCommandHandler`: Solo maneja creación de órdenes
- `AuthController`: Solo maneja autenticación

### 2. Open/Closed (OCP)
- `IOrdenRepository`: Extensible sin modificar código existente
- `AbstractValidator`: Fácil agregar nuevos validadores

### 3. Liskov Substitution (LSP)
- `OrdenRepository` reemplaza a `IOrdenRepository` sin afectar comportamiento
- Handlers de MediatR son intercambiables

### 4. Interface Segregation (ISP)
- `IOrdenRepository` con métodos bien definidos
- Interfaces pequeñas y específicas

### 5. Dependency Inversion (DI)
- Inyección de `IOrdenRepository` en handlers
- Configuración centralizada en `Program.cs`

---

## 📖 Referencias

### Clean Architecture
- https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html
- Robert C. Martin - "Clean Code"

### CQRS Pattern
- https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs
- Greg Young - "CQRS Documents"

### Entity Framework Core
- https://docs.microsoft.com/en-us/ef/core/

### JWT & Security
- https://tools.ietf.org/html/rfc7519
- https://cheatsheetseries.owasp.org/

### MediatR
- https://github.com/jbogard/MediatR
- https://youtu.be/a3x-HA8zGaE (Example walkthrough)

---

## 👨‍💼 Notas de desarrollo

Este proyecto demuestra la modernizacion de un sistema con servicios legacy (.Net Framework) hacia uno moderno (.Net 8.0), esta modernización abarca todas estas caracteristicas:

✅ **Capacidad de Modernización**
- Migración de legacy a arquitectura moderna
- Decisiones tecnológicas justificadas
- Balance entre deuda técnica y nuevas features

✅ **Conocimiento de Patrones**
- Clean Architecture
- CQRS
- Repository Pattern
- Dependency Injection
- Principios SOLID

✅ **Seguridad**
- Autenticación JWT
- Control de acceso por roles
- Validación de entrada

✅ **Escalabilidad**
- Arquitectura preparada para microservicios
- Separación de responsabilidades
- Fácil de probar y mantener

✅ **Documentación**
- README completo
- Swagger/OpenAPI
- Comentarios en código
- Explicación de lógica de negocio

✅ **Competencias DevOps**
- Migraciones EF Core automatizadas
- Configuración por entorno
- Preparada para CI/CD

---

## 📄 Declaración de responsabilidad

Este proyecto es parte de una prueba técnica y se proporciona tal como está.

---

## 📞 Contacto

Para preguntas o sugerencias sobre este proyecto, contacte al desarrollador.

---

**Último actualizado:** 15 de Enero, 2026  
**Versión:** 1.0.0  
**Estado:** ✅ Desarrollo completado

---

## 🔗 Forma de Entrega (Repositorio Git)

Cumple con lo requerido en el documento (PRUEBA_TECNICA.md), sección “4. FORMA DE ENTREGA”:


- URL del repositorio: [SalesOrderManagement](https://github.com/cemado/SalesOrderManagement)
- Instrucciones rápidas:
  ```bash
  git clone https://github.com/cemado/SalesOrderManagement
  dotnet build
  cd src/WebAPI # Verifica contenido del Rest API
  cd src/Frontend #Verifica contenido del Frontend
  ```

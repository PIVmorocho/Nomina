# 💼 NominaApp – Sistema de Gestión de Nómina

> Sistema web completo para la gestión de nómina empresarial, desarrollado con **ASP.NET Core 8 MVC** y **SQL Server**. Permite administrar empleados, departamentos, salarios históricos, títulos/cargos, gerentes y generar reportes exportables a Excel.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?style=flat-square&logo=microsoftsqlserver)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat-square&logo=bootstrap)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

---

## 📌 Tabla de Contenidos

- [Descripción del Proyecto](#-descripción-del-proyecto)
- [Tecnologías Utilizadas](#-tecnologías-utilizadas)
- [Funcionalidades](#-funcionalidades)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación y Configuración](#️-instalación-y-configuración)
- [Base de Datos](#-base-de-datos)
- [Acceso Inicial](#-acceso-inicial)
- [Roles del Sistema](#-roles-del-sistema)
- [Módulos del Sistema](#-módulos-del-sistema)
- [Solución de Problemas](#-solución-de-problemas)

---

## 📖 Descripción del Proyecto

**NominaApp** es una aplicación web de gestión de nómina diseñada para equipos de Recursos Humanos. Permite llevar el control completo del ciclo de vida laboral de cada empleado: desde su ingreso, la asignación a departamentos, los cambios de cargo (títulos), hasta el historial salarial con auditoría automática de cada modificación.

El sistema maneja **vigencias** en todos los registros históricos (asignaciones, salarios, títulos, gerentes), lo que permite consultar el estado de la organización en cualquier punto del tiempo.

### ¿Qué problema resuelve?

- Centraliza la información del personal en una sola plataforma
- Mantiene el historial completo de salarios con trazabilidad (quién cambió qué y cuándo)
- Genera reportes de nómina vigente, cambios salariales y estructura organizacional
- Controla el acceso por roles, diferenciando entre Administradores y personal de RRHH

---

## 🛠 Tecnologías Utilizadas

### Backend

| Tecnología | Versión | Uso |
|---|---|---|
| ASP.NET Core MVC | .NET 8.0 | Framework principal de la aplicación |
| Entity Framework Core | 8.0.0 | ORM para mapeo objeto-relacional |
| EF Core SQL Server | 8.0.0 | Proveedor de base de datos |
| BCrypt.Net-Next | 4.0.3 | Hash seguro de contraseñas |
| ClosedXML | 0.102.2 | Generación de archivos Excel (.xlsx) |

### Frontend

| Tecnología | Versión | Uso |
|---|---|---|
| Bootstrap | 5.3.2 | Framework CSS responsivo |
| Bootstrap Icons | 1.11.3 | Biblioteca de íconos SVG |
| jQuery | 3.7.1 | Manipulación DOM |
| jQuery Validate | 1.19.5 | Validación de formularios del lado cliente |
| jQuery Validate Unobtrusive | 4.0.0 | Integración con validaciones de ASP.NET |

### Base de Datos

| Tecnología | Versión |
|---|---|
| Microsoft SQL Server | 2019 o superior |
| SQL Server Express | Compatible |

### Herramientas de Desarrollo

| Herramienta | Uso |
|---|---|
| Visual Studio 2022 | IDE principal |
| SQL Server Management Studio (SSMS) | Administración de la base de datos |
| NuGet Package Manager | Gestión de dependencias |

---

## ✨ Funcionalidades

### Gestión de Personal
- ✅ Alta, edición y desactivación de empleados
- ✅ Búsqueda por nombre, CI o correo con paginación
- ✅ Ficha completa del empleado (departamentos, títulos y salarios en una sola vista)

### Estructura Organizacional
- ✅ Gestión de departamentos
- ✅ Asignación de empleados a departamentos con fechas de vigencia
- ✅ Asignación de gerentes por departamento con historial
- ✅ Control de solapamiento de vigencias

### Gestión Salarial
- ✅ Historial de salarios por empleado con fechas desde/hasta
- ✅ Auditoría automática: cada cambio salarial queda registrado con usuario, fecha y detalle
- ✅ Exportación del log de auditoría a Excel con filtros

### Títulos y Cargos
- ✅ Historial de cargos por empleado con vigencias
- ✅ Validación de solapamiento de períodos

### Reportes
- ✅ **Nómina Vigente**: empleados activos con salario, departamento y título actual
- ✅ **Cambios Salariales**: historial de modificaciones en un rango de fechas
- ✅ **Estructura Organizacional**: vista jerárquica departamento → gerente → empleados
- ✅ Exportación a Excel de todos los reportes
- ✅ Impresión / exportación a PDF desde el navegador

### Seguridad
- ✅ Autenticación por cookies con expiración configurable
- ✅ Contraseñas hasheadas con BCrypt
- ✅ Autorización por roles (`Administrador`, `RRHH`)
- ✅ Protección CSRF en todos los formularios
- ✅ Opción "Recordarme" (sesión extendida 7 días)

### Administración
- ✅ Gestión de usuarios del sistema (solo Administrador)
- ✅ Reset de contraseñas desde la interfaz
- ✅ Dashboard con KPIs: empleados activos, nómina total, vigencias por vencer

---

## 📁 Estructura del Proyecto

```
NominaApp/
│
├── Controllers/
│   ├── AccountController.cs        # Login / Logout
│   ├── DashboardController.cs      # Panel principal con KPIs
│   ├── EmpleadosController.cs      # ABM de empleados
│   ├── DepartamentosController.cs  # ABM de departamentos
│   ├── AsignacionesController.cs   # Empleado <-> Departamento
│   ├── GerentesController.cs       # Gerentes por departamento
│   ├── TitulosController.cs        # Cargos / títulos históricos
│   ├── SalariosController.cs       # Historial salarial + auditoría
│   ├── AuditoriaController.cs      # Log de cambios salariales
│   ├── ReportesController.cs       # Generación de reportes + Excel
│   └── UsuariosController.cs       # Gestión de usuarios del sistema
│
├── Data/
│   └── AppDbContext.cs             # Contexto EF Core + configuración de modelos
│
├── Models/
│   ├── Employee.cs                 # Empleado
│   ├── Department.cs               # Departamento
│   ├── DeptEmp.cs                  # Asignación empleado-departamento
│   ├── DeptManager.cs              # Gerente de departamento
│   ├── Title.cs                    # Título / cargo
│   ├── Salary.cs                   # Salario
│   ├── User.cs                     # Usuario del sistema
│   └── LogAuditoriaSalarios.cs     # Log de auditoría
│
├── ViewModels/
│   ├── LoginViewModel.cs
│   ├── DashboardViewModel.cs
│   └── ReporteViewModel.cs         # Modelos para los 3 tipos de reporte
│
├── Services/
│   └── AuditoriaService.cs         # Registro automático de auditoría
│
├── Views/
│   ├── Account/                    # Login
│   ├── Dashboard/                  # Panel principal
│   ├── Empleados/                  # Index, Form, Ver
│   ├── Departamentos/              # Index, Form
│   ├── Asignaciones/               # Index, Form
│   ├── Gerentes/                   # Index, Form
│   ├── Titulos/                    # Index, Form
│   ├── Salarios/                   # Index, Form
│   ├── Auditoria/                  # Index
│   ├── Reportes/                   # Index + 3 vistas de reporte
│   ├── Usuarios/                   # Index, Form
│   └── Shared/
│       ├── _Layout.cshtml          # Layout principal (sidebar)
│       ├── _LoginLayout.cshtml     # Layout de pantalla de login
│       └── _ValidationScriptsPartial.cshtml
│
├── database/
│   ├── NominaDB_backup.bak         # Backup restaurable en SSMS
│   └── NominaDB_script.sql         # Script para crear la BD desde cero
│
├── appsettings.json                # CONFIGURAR AQUÍ la cadena de conexión
└── Program.cs                      # Configuración de servicios y pipeline HTTP
```

---

## 📋 Requisitos Previos

Asegúrate de tener instalado lo siguiente antes de continuar:

- [Visual Studio 2022](https://visualstudio.microsoft.com/) con la carga de trabajo **"Desarrollo de ASP.NET y web"**
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads) o **SQL Server Express** (gratuito)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms)

---

## ⚙️ Instalación y Configuración

### Paso 1 — Clonar el Repositorio

```bash
git clone https://github.com/tu-usuario/NominaApp.git
cd NominaApp
```

O descarga el ZIP desde GitHub y extrae los archivos.

---

### Paso 2 — Restaurar la Base de Datos

Tienes dos opciones:

#### Opción A: Restaurar desde el archivo `.bak` (recomendado)

1. Abre **SQL Server Management Studio (SSMS)**
2. Clic derecho en **Bases de datos → Restaurar base de datos...**
3. Selecciona **Dispositivo** → clic en `...` → **Agregar**
4. Busca y selecciona el archivo `database/NominaDB_backup.bak`
5. Confirma que el nombre de destino sea `NominaDB`
6. Clic en **Aceptar**

#### Opción B: Ejecutar el script SQL desde cero

1. Abre **SSMS** y conéctate a tu instancia de SQL Server
2. Abre una **Nueva Consulta**
3. Copia y ejecuta el contenido del archivo `database/NominaDB_script.sql`

<details>
<summary>📄 Ver script SQL completo (haz clic para expandir)</summary>

```sql
CREATE DATABASE NominaDB;
GO
USE NominaDB;
GO

-- Tabla employees
CREATE TABLE employees (
    emp_no      INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
    ci          VARCHAR(50)   NOT NULL,
    birth_date  DATE          NOT NULL,
    first_name  VARCHAR(50)   NOT NULL,
    last_name   VARCHAR(50)   NOT NULL,
    gender      CHAR(1)       NOT NULL CHECK (gender IN ('M','F')),
    hire_date   DATE          NOT NULL,
    correo      VARCHAR(100)  NULL,
    activo      BIT           NOT NULL DEFAULT 1,
    CONSTRAINT UQ_employees_ci     UNIQUE (ci),
    CONSTRAINT UQ_employees_correo UNIQUE (correo)
);

-- Tabla departments
CREATE TABLE departments (
    dept_no    INT          NOT NULL IDENTITY(1,1) PRIMARY KEY,
    dept_name  VARCHAR(50)  NOT NULL,
    activo     BIT          NOT NULL DEFAULT 1
);

-- Tabla dept_emp
CREATE TABLE dept_emp (
    emp_no     INT   NOT NULL,
    dept_no    INT   NOT NULL,
    from_date  DATE  NOT NULL,
    to_date    DATE  NULL,
    CONSTRAINT PK_dept_emp         PRIMARY KEY (emp_no, dept_no, from_date),
    CONSTRAINT FK_dept_emp_emp     FOREIGN KEY (emp_no)  REFERENCES employees(emp_no),
    CONSTRAINT FK_dept_emp_dept    FOREIGN KEY (dept_no) REFERENCES departments(dept_no)
);

-- Tabla dept_manager
CREATE TABLE dept_manager (
    emp_no     INT   NOT NULL,
    dept_no    INT   NOT NULL,
    from_date  DATE  NOT NULL,
    to_date    DATE  NULL,
    CONSTRAINT PK_dept_manager      PRIMARY KEY (emp_no, dept_no, from_date),
    CONSTRAINT FK_dept_manager_emp  FOREIGN KEY (emp_no)  REFERENCES employees(emp_no),
    CONSTRAINT FK_dept_manager_dept FOREIGN KEY (dept_no) REFERENCES departments(dept_no)
);

-- Tabla titles
CREATE TABLE titles (
    emp_no     INT          NOT NULL,
    title      VARCHAR(50)  NOT NULL,
    from_date  DATE         NOT NULL,
    to_date    DATE         NULL,
    CONSTRAINT PK_titles     PRIMARY KEY (emp_no, title, from_date),
    CONSTRAINT FK_titles_emp FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);

-- Tabla salaries
CREATE TABLE salaries (
    emp_no     INT     NOT NULL,
    salary     BIGINT  NOT NULL,
    from_date  DATE    NOT NULL,
    to_date    DATE    NULL,
    CONSTRAINT PK_salaries     PRIMARY KEY (emp_no, from_date),
    CONSTRAINT FK_salaries_emp FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);

-- Tabla users
CREATE TABLE users (
    usuario  VARCHAR(100)  NOT NULL PRIMARY KEY,
    emp_no   INT           NOT NULL,
    clave    VARCHAR(100)  NOT NULL,
    rol      VARCHAR(50)   NOT NULL DEFAULT 'RRHH',
    activo   BIT           NOT NULL DEFAULT 1,
    CONSTRAINT FK_users_emp FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);

-- Tabla Log_AuditoriaSalarios
CREATE TABLE Log_AuditoriaSalarios (
    id                 INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
    usuario            VARCHAR(50)   NOT NULL,
    fechaActualizacion DATE          NOT NULL,
    DetalleCambio      VARCHAR(250)  NOT NULL,
    salario            BIGINT        NOT NULL,
    emp_no             INT           NOT NULL,
    CONSTRAINT FK_audit_emp FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);

-- Empleado y usuario administrador inicial
-- Contraseña: Admin123
INSERT INTO employees (ci, birth_date, first_name, last_name, gender, hire_date, correo)
VALUES ('0000000001', '1980-01-01', 'Admin', 'Sistema', 'M', GETDATE(), 'admin@nomina.com');

INSERT INTO users (usuario, emp_no, clave, rol)
VALUES ('admin', 1, '$2a$11$K7Ei3B1v9E3pJ4Jgj4NQyuHVfOHR0.O7KD1V3qz3FwwMv5D5cQVCC', 'Administrador');
GO
```

</details>

---

### Paso 3 — Configurar la Cadena de Conexión

Abre el archivo **`appsettings.json`** en la raíz del proyecto:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=NominaDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### ¿Qué poner en `Server=`?

| Tu entorno | Valor de `Server` |
|---|---|
| SQL Server local (instancia por defecto) | `Server=.` |
| SQL Server Express local | `Server=.\SQLEXPRESS` |
| Instancia con nombre (ver en SSMS al conectarte) | `Server=NOMBRE_PC\NOMBRE_INSTANCIA` |
| Servidor remoto por IP | `Server=192.168.1.100` |

**Ejemplo con instancia con nombre** (como en este proyecto):
```json
"DefaultConnection": "Server=DESKTOP-MCDC74U\\MSSQLSERVER01;Database=NominaDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

**Ejemplo con usuario y contraseña SQL** (si no usas Windows Auth):
```json
"DefaultConnection": "Server=.;Database=NominaDB;User Id=sa;Password=tu_contraseña;TrustServerCertificate=True;"
```

> 💡 **¿Cómo sé el nombre de mi instancia?** Abre SSMS. El nombre exacto aparece en el campo **"Nombre del servidor"** al conectarte. Cópialo tal cual.

---

### Paso 4 — Instalar Paquetes NuGet

Abre **Herramientas → Administrador de Paquetes NuGet → Consola del Administrador de Paquetes** y ejecuta:

```powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 8.0.0
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.0
Install-Package BCrypt.Net-Next -Version 4.0.3
Install-Package ClosedXML -Version 0.102.2
```

---

### Paso 5 — Ejecutar la Aplicación

Presiona **F5** en Visual Studio, o desde la terminal:

```bash
dotnet run
```

Abre en tu navegador: `https://localhost:XXXX/Account/Login`

---

## 🗄 Base de Datos

### Diagrama de relaciones

```
employees ──┬──< dept_emp        >──── departments
            ├──< dept_manager    >──── departments
            ├──< titles
            ├──< salaries
            ├──  users
            └──< Log_AuditoriaSalarios
```

### Descripción de tablas

| Tabla | Descripción |
|---|---|
| `employees` | Datos personales del empleado (CI, nombre, género, fechas) |
| `departments` | Catálogo de departamentos |
| `dept_emp` | Asignación histórica empleado ↔ departamento (con vigencia) |
| `dept_manager` | Historial de gerentes por departamento (con vigencia) |
| `titles` | Cargos o títulos históricos por empleado (con vigencia) |
| `salaries` | Historial salarial por empleado (con vigencia) |
| `users` | Usuarios del sistema con rol y hash BCrypt de contraseña |
| `Log_AuditoriaSalarios` | Registro automático de cada cambio salarial |

### Archivos de base de datos incluidos en el repositorio

```
BDA backup/
├── NominaDB_backup.bak    ← Restaurar directamente desde SSMS
BDA  script/
└── NominaDB_script.sql    ← Crear la BD desde cero con este script
```

---

## 🔑 Acceso Inicial

| Campo | Valor |
|---|---|
| URL | `https://localhost:XXXX/Account/Login` |
| Usuario | `admin` |
| Contraseña | `Admin123` |

> ⚠️ **Cambia la contraseña del administrador** inmediatamente después del primer acceso.
> Ve a **Usuarios → ícono de llave (🔑) → Resetear Clave**.

---

## 👥 Roles del Sistema

| Rol | Permisos |
|---|---|
| `Administrador` | Acceso completo a todos los módulos + gestión de usuarios |
| `RRHH` | Empleados, departamentos, asignaciones, salarios, auditoría y reportes |

---

## 📊 Módulos del Sistema

| Módulo | Ruta | Rol mínimo | Descripción |
|---|---|---|---|
| Dashboard | `/Dashboard` | RRHH | KPIs: empleados activos, nómina, vigencias por vencer |
| Empleados | `/Empleados` | RRHH | Alta, edición, desactivación y ficha completa |
| Departamentos | `/Departamentos` | RRHH | Catálogo de departamentos |
| Asignaciones | `/Asignaciones` | RRHH | Empleado ↔ Departamento con vigencias |
| Gerentes | `/Gerentes` | RRHH | Gerente activo por departamento |
| Títulos | `/Titulos` | RRHH | Cargos históricos por empleado |
| Salarios | `/Salarios` | RRHH | Historial salarial con auditoría automática |
| Auditoría | `/Auditoria` | RRHH | Log de cambios salariales + exportar Excel |
| Reportes | `/Reportes` | RRHH | Nómina vigente, cambios salariales, estructura org. |
| Usuarios | `/Usuarios` | **Administrador** | Crear, desactivar y resetear claves |

---

## 🔧 Solución de Problemas

### Error de conexión a la base de datos

1. Verifica que el valor de `Server=` en `appsettings.json` coincide con el nombre que ves en SSMS
2. Confirma que el servicio de SQL Server está activo: `Win + R → services.msc → SQL Server (...)`
3. Verifica que la base de datos `NominaDB` existe en tu instancia

### El usuario `admin` no puede iniciar sesión

El hash incluido en el script puede no ser compatible. Generas uno nuevo así:

```csharp
// Crea una app de consola .NET con este código y ejecútala:
using BCrypt.Net;
Console.WriteLine(BCrypt.HashPassword("Admin123"));
```

Luego actualiza el hash en SSMS:

```sql
USE NominaDB;
UPDATE users SET clave = 'PEGA_EL_NUEVO_HASH_AQUI' WHERE usuario = 'admin';
```

### Error al exportar Excel

Verifica que `ClosedXML` esté instalado. En la consola NuGet:

```powershell
Update-Package ClosedXML
```

### Advertencia de certificado SSL en desarrollo

```bash
dotnet dev-certs https --trust
```

---


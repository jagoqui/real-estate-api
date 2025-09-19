# 🏠 Real Estate API

API REST para gestión de propiedades inmobiliarias construida con .NET 9, MongoDB Atlas y NUnit. Implementa una arquitectura hexagonal que permite un desarrollo modular, escalable y altamente testeable.

## 🌐 Demo en Vivo

🚀 **API desplegada en Render**: [https://real-estate-api-i4og.onrender.com/api](https://real-estate-api-i4og.onrender.com/api)
📡 **Documentación Swagger**: [https://real-estate-api-i4og.onrender.com/swagger/index.html](https://real-estate-api-i4og.onrender.com/swagger/index.html)
🤖 **Documentación automática** [Deepwiki](https://deepwiki.com/jagoqui/real-estate-api)

## �📋 Tabla de Contenidos

- [🌐 Demo en Vivo](#-demo-en-vivo)
- [📖 Descripción](#-descripción)
- [🚀 Inicio Rápido](#-inicio-rápido)
- [🏗️ Arquitectura](#️-arquitectura)
- [📊 Diagrama de Base de Datos](#-diagrama-de-base-de-datos)
- [🛠️ Tecnologías y Herramientas](#️-tecnologías-y-herramientas)
- [📡 Endpoints API](#-endpoints-api)
- [🔧 Configuración del Proyecto](#-configuración-del-proyecto)
- [📏 Estándares de Código](#-estándares-de-código)
- [🔌 Extensiones Recomendadas](#-extensiones-recomendadas)
- [🧪 Testing](#-testing)
- [📝 Contribución](#-contribución)

## 📖 Descripción

Este proyecto es una API REST para la gestión de propiedades inmobiliarias que incluye:

- **Gestión de Propietarios**: CRUD completo de propietarios con información personal
- **Gestión de Propiedades**: CRUD de propiedades con filtros avanzados de búsqueda
- **Gestión de Imágenes**: Manejo de imágenes asociadas a propiedades
- **Trazabilidad**: Registro histórico de transacciones y cambios en propiedades

## 🚀 Inicio Rápido

### Prerrequisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MongoDB Atlas](https://www.mongodb.com/atlas) (base de datos en la nube) o [MongoDB Community](https://www.mongodb.com/try/download/community) (local)
- [Docker](https://www.docker.com/get-started) (Se usa para el despliegue y de forma opcional en local)

### Configuración del Entorno

1. **Clonar el repositorio**

   ```bash
   git clone https://github.com/jagoqui/real-estate-api.git
   cd real-estate-api
   ```

2. **Restaurar dependencias**

   ```bash
   dotnet restore
   ```

### 3. Configurar MongoDB y variables de entorno

- Crear un archivo `.env` en la raíz del proyecto con las siguientes variables (no incluyas valores reales):

  ```env
  ASPNETCORE_URLS=http://+:8080
  DatabaseSettings__ConnectionString=mongodb+srv://<usuario>:<contraseña>@<tu-cluster>.mongodb.net/?retryWrites=true&w=majority&appName=RealEstateCluster
  DatabaseSettings__DatabaseName=<NombreDeTuBaseDeDatos>
  ```

### 4. Ejecutar la aplicación

- #### Levantar en local con dotnet

  ```bash
  dotnet watch run
  ```

- #### Levantar en local con Docker

  ```bash
  docker run -it --rm -p 5247:8080 -v "$(pwd)/environments/.env:/etc/secrets/.env"  realestate-api
  ```

La API estará disponible en `http://localhost:8080` y la documentación Swagger en `http://localhost:8080/swagger`.

### 🌐 **URLs de Producción**

- **API en Producción**: [https://real-estate-api-i4og.onrender.com/api](https://real-estate-api-i4og.onrender.com/api)
- **Swagger en Producción**: [https://real-estate-api-i4og.onrender.com/swagger/index.html](https://real-estate-api-i4og.onrender.com/swagger/index.html)

## 🏗️ Arquitectura

El proyecto implementa una **Arquitectura Hexagonal** (Clean Architecture) organizada en tres capas principales:

### 📁 Estructura de Capas

```bash
├── Domain/                     # 🎯 Capa de Dominio
│   └── Entities/              # Entidades de negocio
├── Application/               # 📋 Capa de Aplicación  
│   └── Contracts/             # Interfaces y contratos
└── Infrastructure/            # 🔧 Capa de Infraestructura
    ├── API/                   # Controladores y middleware
    ├── Config/                # Configuraciones
    ├── DTOs/                  # Objetos de transferencia
    ├── Repositories/          # Acceso a datos
    └── Services/              # Lógica de aplicación
```

### 🎯 **Domain (Dominio)**

- **Responsabilidad**: Contiene las entidades de negocio y reglas de dominio
- **Contenido**: Entidades como `Owner`, `Property`, `PropertyImage`, `PropertyTrace`
- **Dependencias**: Ninguna (capa más interna)

### 📋 **Application (Aplicación)**

- **Responsabilidad**: Define contratos e interfaces para los servicios
- **Contenido**: Interfaces como `IOwnerService`, `IPropertyService`, etc.
- **Dependencias**: Solo puede importar `Domain`

### 🔧 **Infrastructure (Infraestructura)**

- **Responsabilidad**: Implementaciones concretas, acceso a datos, APIs, configuraciones
- **Contenido**: Controladores, repositorios, servicios, DTOs, middleware
- **Dependencias**: Puede importar `Application` y `Domain`

### 🔄 **Flujo de Dependencias**

```bash
Infrastructure → Application → Domain
```

**Regla de Oro**: Solo se pueden importar capas que estén por encima de la capa actual.

### 🔀 **Flujo de Ejecución**

```bash
Controller → Service → Repository → Database
```

## 📊 Diagrama de Base de Datos

```mermaid
erDiagram
    Owner {
        ObjectId IdOwner PK
        string Name
        string Address
        string Photo
        DateTime Birthday
    }
    
    Property {
        ObjectId IdProperty PK
        string Name
        string Address
        decimal Price
        string CodeInternal
        int Year
        ObjectId IdOwner FK
    }
    
    PropertyImage {
        ObjectId IdPropertyImage PK
        ObjectId IdProperty FK
        string File
        bool Enabled
    }
    
    PropertyTrace {
        ObjectId IdPropertyTrace PK
        DateTime DateSale
        string Name
        decimal Value
        decimal Tax
        ObjectId IdProperty FK
    }
    
    Owner ||--o{ Property : "owns"
    Property ||--o{ PropertyImage : "has"
    Property ||--o{ PropertyTrace : "tracks"
```

  > [!NOTE]
  > Puede copiar el código y ver el diagrama en <https://mermaid.live/>

### 🔗 **Relaciones**

- **Owner → Property**: Un propietario puede tener múltiples propiedades (1:N)
- **Property → PropertyImage**: Una propiedad puede tener múltiples imágenes (1:N)
- **Property → PropertyTrace**: Una propiedad puede tener múltiples registros de trazabilidad (1:N)

## 🛠️ Tecnologías y Herramientas

### 🎯 **Framework y Runtime**

- **.NET 9**: Framework principal
- **ASP.NET Core**: Para API REST
- **C#**: Lenguaje de programación

### 🗄️ **Base de Datos**

- **MongoDB Atlas 3.5.0**: Base de datos NoSQL en la nube
- **MongoDB.Driver**: Driver oficial para .NET

### 📚 **Librerías y Paquetes**

- **Swashbuckle.AspNetCore 9.0.4**: Documentación OpenAPI/Swagger automática
- **DotNetEnv 3.1.1**: Manejo de variables de entorno
- **StyleCop.Analyzers 1.1.118**: Análisis estático de código

### 🧪 **Testing**

- **NUnit**: Framework de testing principal

### 🔧 **Herramientas de Desarrollo**

- **Docker**: Containerización
- **Swagger UI**: Interfaz interactiva para documentación API
- **Render**: Plataforma de despliegue en la nube
- **EditorConfig**: Configuración de editor consistente
- **StyleCop**: Análisis y formato de código

## 📡 Endpoints API

### 👤 **Owners (Propietarios)**

| Método | Endpoint | Descripción | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| `GET` | `/api/owner` | Obtener todos los propietarios | - | `IEnumerable<Owner>` |
| `GET` | `/api/owner/{id}` | Obtener propietario por ID | - | `Owner` |
| `POST` | `/api/owner` | Crear nuevo propietario | `OwnerWithoutId` | `Owner` |
| `PUT` | `/api/owner/{id}` | Actualizar propietario | `OwnerWithoutId` | `Owner` |
| `DELETE` | `/api/owner/{id}` | Eliminar propietario | - | `204 No Content` |

### 🏠 **Properties (Propiedades)**

| Método | Endpoint | Descripción | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| `GET` | `/api/property` | Obtener todas las propiedades | - | `IEnumerable<Property>` |
| `GET` | `/api/property/{id}` | Obtener propiedad por ID | - | `Property` |
| `GET` | `/api/property/owner/{ownerId}` | Obtener propiedades por propietario | - | `IEnumerable<Property>` |
| `GET` | `/api/property/filter` | Filtrar propiedades | Query params | `IEnumerable<Property>` |
| `POST` | `/api/property` | Crear nueva propiedad | `PropertyWithoutId` | `Property` |
| `PUT` | `/api/property/{id}` | Actualizar propiedad | `PropertyWithoutId` | `Property` |
| `DELETE` | `/api/property/{id}` | Eliminar propiedad | - | `204 No Content` |

#### 🔍 **Filtros de Propiedades**

```
GET /api/property/filter?name={name}&address={address}&minPrice={min}&maxPrice={max}
```

### 🖼️ **Property Images (Imágenes de Propiedades)**

| Método | Endpoint | Descripción | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| `GET` | `/api/propertyimage` | Obtener todas las imágenes | - | `IEnumerable<PropertyImage>` |
| `GET` | `/api/propertyimage/{id}` | Obtener imagen por ID | - | `PropertyImage` |
| `GET` | `/api/propertyimage/property/{propertyId}` | Obtener imágenes por propiedad | - | `IEnumerable<PropertyImage>` |
| `POST` | `/api/propertyimage` | Crear nueva imagen | `PropertyImageWithoutId` | `PropertyImage` |
| `PUT` | `/api/propertyimage/{id}` | Actualizar imagen | `PropertyImageWithoutId` | `PropertyImage` |
| `PATCH` | `/api/propertyimage/{id}/file` | Actualizar archivo de imagen | `string` (base64) | `PropertyImage` |

### 📊 **Property Traces (Trazabilidad de Propiedades)**

| Método | Endpoint | Descripción | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| `GET` | `/api/propertytrace` | Obtener todos los trazos | - | `IEnumerable<PropertyTrace>` |
| `GET` | `/api/propertytrace/{id}` | Obtener trazo por ID | - | `PropertyTrace` |
| `POST` | `/api/propertytrace` | Crear nuevo trazo | `IPropertyTraceTax` | `PropertyTrace` |

### 📋 **Códigos de Estado**

- **200 OK**: Operación exitosa
- **201 Created**: Recurso creado exitosamente
- **204 No Content**: Operación exitosa sin contenido
- **400 Bad Request**: Datos de entrada inválidos
- **404 Not Found**: Recurso no encontrado

## 🔧 Configuración del Proyecto

### ⚙️ **Variables de Entorno**

```env
# Configuración del servidor
ASPNETCORE_URLS=http://+:8080

# Configuración de MongoDB Atlas
DatabaseSettings__ConnectionString=mongodb+srv://usuario:contraseña@cluster.mongodb.net/?retryWrites=true&w=majority&appName=RealEstateCluster
DatabaseSettings__DatabaseName=RealEstateDB
```

### 🚀 **Despliegue en Render**

La aplicación está desplegada en [Render](https://render.com/) con las siguientes características:

- **Plataforma**: Render Web Service
- **Base de Datos**: MongoDB Atlas (nube)
- **URL de Producción**: [https://real-estate-api-i4og.onrender.com](https://real-estate-api-i4og.onrender.com)
- **Swagger UI**: [https://real-estate-api-i4og.onrender.com/swagger/index.html](https://real-estate-api-i4og.onrender.com/swagger/index.html)
- **Configuración**: Variables de entorno configuradas en Render Dashboard

### 🐳 **Docker**

```dockerfile
# Ejecutar con Docker
docker build -t realestate-api .
docker run -it --rm -p 5247:8080 -v "$(pwd)/environments/.env:/etc/secrets/.env" realestate-api
```

### 🗂️ **Configuración de Base de Datos**

La configuración de MongoDB se encuentra en `Infrastructure/Config/DatabaseSettings.cs` y se inyecta mediante el patrón Options.

## 📏 Estándares de Código

### 🎨 **StyleCop y EditorConfig**

El proyecto utiliza **StyleCop.Analyzers** y **EditorConfig** para mantener consistencia en el código:

- **Indentación**: 4 espacios
- **Longitud máxima de línea**: 120 caracteres
- **Estilo de llaves**: Nueva línea para todas las llaves
- **Ordenamiento**: `using` del sistema primero

### 🔧 **Formateo de Código**

Para formatear el código según los estándares del proyecto:

```bash
# Formatear todo el proyecto
dotnet tool run dotnet-format RealEstateApi.sln

# Verificar formato sin aplicar cambios
dotnet tool run dotnet-format RealEstateApi.sln --verify-no-changes
```

### ✅ **Reglas Principales**

- **SA1000**: Palabras clave deben estar seguidas de espacio
- **SA1200**: `using` statements deben estar dentro del namespace (deshabilitado)
- **SA1300**: Nombres de elementos deben comenzar con mayúscula
- **SA1400**: Modificadores de acceso deben estar declarados
- **SA1500**: Llaves deben estar en nueva línea
- **SA1600**: Elementos deben estar documentados (deshabilitado para este proyecto)

## 🔌 Extensiones Recomendadas

### 🛠️ **VS Code Extensions**

```json
{
  "recommendations": [
    "ms-dotnettools.csdevkit",           // 🔧 Kit de desarrollo C#
    "EditorConfig.EditorConfig",         // ⚙️ Soporte para EditorConfig  
    "streetsidesoftware.code-spell-checker", // 📝 Corrector ortográfico
    "streetsidesoftware.code-spell-checker-spanish", // 🇪🇸 Español
    "hediet.vscode-drawio",              // 📊 Diagramas Draw.io
    "yzhang.markdown-all-in-one",       // 📄 Markdown mejorado
    "christian-kohler.path-intellisense", // 📂 IntelliSense para rutas
    "wayou.vscode-todo-highlight",       // ✅ Destacar TODOs
    "DavidAnson.vscode-markdownlint"    // 📋 Linter para Markdown
  ]
}
```

### ⚙️ **Configuración de VS Code**

```json
{
  "editor.formatOnSave": true,                    // ✨ Formatear al guardar
  "editor.defaultFormatter": "ms-dotnettools.csdevkit", // 🔧 Formateador por defecto
  "csharp.suppressHiddenDiagnostics": false,     // 🔍 Mostrar diagnósticos
  "omnisharp.useModernNet": true,                 // 🆕 Usar .NET moderno
  "csharp.format.enable": true,                   // ✅ Habilitar formato C#
  "csharp.enableEditorConfigSupport": true,      // ⚙️ Soporte EditorConfig
  "csharp.enableRoslynAnalyzers": true,          // 🔍 Analyzers Roslyn
  "csharp.organizeImportsOnFormat": true,        // 📁 Organizar imports
  "[csharp]": {
    "editor.defaultFormatter": "ms-dotnettools.csharp" // 🎯 Formateador C#
  }
}
```

## 🧪 Testing

### 🚀 **Ejecutar Tests**

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar tests con coverage
dotnet test --collect:"XPlat Code Coverage"

# Ejecutar tests en modo watch
dotnet watch test
```

### 📊 **Framework de Testing**

- **NUnit**: Framework principal para unit tests
- **Cobertura**: Análisis de cobertura de código incluido

## 📝 Contribución

### 🔄 **Workflow de Desarrollo**

1. **Fork** del repositorio
2. **Crear** una rama para tu feature: `git checkout -b feature/nueva-funcionalidad`
3. **Commitear** cambios: `git commit -am 'Add nueva funcionalidad'`
4. **Push** a la rama: `git push origin feature/nueva-funcionalidad`
5. **Crear** un Pull Request

### 📋 **Guías de Contribución**

- Seguir los estándares de código establecidos
- Ejecutar `dotnet tool run dotnet-format` antes de hacer commit
- Escribir tests para nuevas funcionalidades
- Actualizar documentación según sea necesario
- Usar commits descriptivos y en inglés

### 🐛 **Reportar Issues**

Para reportar bugs o solicitar features, usar el sistema de Issues de GitHub con las siguientes etiquetas:

- `bug` 🐛: Para errores
- `enhancement` ✨: Para mejoras
- `documentation` 📚: Para documentación
- `help wanted` 🆘: Para ayuda de la comunidad

---

## 📞 Contacto

**Desarrollador**: Jagoqui  
**Repository**: [https://github.com/jagoqui/real-estate-api](https://github.com/jagoqui/real-estate-api)

---

⭐ ¡No olvides dar una estrella al proyecto si te fue útil!

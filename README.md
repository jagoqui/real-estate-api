# Real Estate API

## Descripción del Proyecto

Esta es la API backend de una **prueba técnica fullstack para Sr. Frontend Developer**. El proyecto consiste en desarrollar una solución completa para la gestión de propiedades inmobiliarias, incluyendo un backend robusto y un frontend moderno para la visualización y filtrado de propiedades.

## Tecnologías Utilizadas

### Backend

- **.NET 8/9** (C#)
- **MongoDB** como base de datos NoSQL
- **NUnit** para pruebas unitarias

### Frontend (Recomendado)

- **React** o **Next.js**
- Integración con la API REST

## Características y Funcionalidades

### API Backend

La API debe proporcionar las siguientes funcionalidades:

- ✅ **Gestión de Propiedades**: CRUD completo para propiedades inmobiliarias
- ✅ **Filtrado Avanzado**: Capacidad de filtrar propiedades por:
  - Nombre de la propiedad
  - Dirección
  - Rango de precios (precio mínimo y máximo)
- ✅ **Gestión de Imágenes**: Soporte para almacenar y servir imágenes de propiedades
- ✅ **API RESTful**: Endpoints bien estructurados siguiendo mejores prácticas

### Modelos de Datos (DTOs)

Los DTOs (Data Transfer Objects) incluyen los siguientes campos:

```csharp
public class PropertyDto
{
    public string IdOwner { get; set; }           // ID del propietario
    public string Name { get; set; }             // Nombre de la propiedad
    public string AddressProperty { get; set; }  // Dirección de la propiedad
    public decimal PriceProperty { get; set; }   // Precio de la propiedad
    public string Image { get; set; }            // URL o referencia de la imagen
}
```

## Estructura del Proyecto

```
real-estate-api/
├── src/
│   ├── RealEstate.API/          # Proyecto principal de la API
│   ├── RealEstate.Core/         # Modelos y lógica de negocio
│   ├── RealEstate.Data/         # Acceso a datos y MongoDB
│   └── RealEstate.DTOs/         # Data Transfer Objects
├── tests/
│   └── RealEstate.Tests/        # Pruebas unitarias con NUnit
├── docs/                        # Documentación adicional
└── README.md
```

## Instalación y Configuración

### Prerequisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) o superior
- [MongoDB](https://www.mongodb.com/try/download/community) (local o en la nube)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) o [VS Code](https://code.visualstudio.com/)

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
  docker run -it --rm --env-file .env -p 5247:8080 realestate-api
  ```

## Endpoints de la API

### Propiedades

| Método | Endpoint               | Descripción                    |
| ------ | ---------------------- | ------------------------------ |
| GET    | `/api/properties`      | Obtener todas las propiedades  |
| GET    | `/api/properties/{id}` | Obtener una propiedad por ID   |
| POST   | `/api/properties`      | Crear nueva propiedad          |
| PUT    | `/api/properties/{id}` | Actualizar propiedad existente |
| DELETE | `/api/properties/{id}` | Eliminar propiedad             |

### Filtrado

| Método | Endpoint                                                                             | Descripción                  |
| ------ | ------------------------------------------------------------------------------------ | ---------------------------- |
| GET    | `/api/properties/search?name={name}`                                                 | Filtrar por nombre           |
| GET    | `/api/properties/search?address={address}`                                           | Filtrar por dirección        |
| GET    | `/api/properties/search?minPrice={min}&maxPrice={max}`                               | Filtrar por rango de precios |
| GET    | `/api/properties/search?name={name}&address={address}&minPrice={min}&maxPrice={max}` | Filtrado combinado           |

### Ejemplos de Uso

#### Crear una nueva propiedad

```bash
curl -X POST http://localhost:5000/api/properties \
  -H "Content-Type: application/json" \
  -d '{
    "idOwner": "owner123",
    "name": "Casa en el Centro",
    "addressProperty": "Calle 123 #45-67, Bogotá",
    "priceProperty": 250000000,
    "image": "https://example.com/images/casa1.jpg"
  }'
```

#### Filtrar propiedades por precio

```bash
curl "http://localhost:5000/api/properties/search?minPrice=200000000&maxPrice=300000000"
```

## Pruebas

### Ejecutar Pruebas Unitarias

El proyecto utiliza **NUnit** para las pruebas unitarias:

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar pruebas con reporte de cobertura
dotnet test --collect:"XPlat Code Coverage"

# Ejecutar pruebas específicas
dotnet test --filter "TestCategory=Unit"
```

### Estructura de Pruebas

```
tests/RealEstate.Tests/
├── Controllers/
│   └── PropertiesControllerTests.cs
├── Services/
│   └── PropertyServiceTests.cs
└── Repositories/
    └── PropertyRepositoryTests.cs
```

## Frontend (Recomendaciones)

Para el desarrollo del frontend, se recomienda:

### React

```bash
npx create-react-app real-estate-frontend
cd real-estate-frontend
npm install axios react-router-dom
```

### Next.js

```bash
npx create-next-app@latest real-estate-frontend
cd real-estate-frontend
npm install axios
```

### Funcionalidades Frontend Esperadas

- 📋 **Lista de Propiedades**: Mostrar todas las propiedades disponibles
- 🔍 **Filtros**: Implementar filtros por nombre, dirección y precio
- 📱 **Diseño Responsivo**: Optimizado para móviles y escritorio
- 🖼️ **Galería de Imágenes**: Mostrar imágenes de las propiedades
- 📄 **Paginación**: Para manejar grandes cantidades de datos

## Contribución

1. Fork del proyecto
2. Crear una rama para la feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit de los cambios (`git commit -am 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Crear un Pull Request

## Criterios de Evaluación

Esta prueba técnica será evaluada considerando:

- ✅ **Arquitectura**: Estructura del código y separación de responsabilidades
- ✅ **Calidad del Código**: Legibilidad, mantenibilidad y mejores prácticas
- ✅ **Funcionalidad**: Implementación completa de los requisitos
- ✅ **Pruebas**: Cobertura y calidad de las pruebas unitarias
- ✅ **Documentación**: Claridad y completitud de la documentación
- ✅ **Performance**: Optimización de consultas y respuesta de la API

## Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

## Contacto

**Jaidiver Gómez Quintero**

- GitHub: [@jagoqui](https://github.com/jagoqui)

---

> **Nota**: Esta es una prueba técnica diseñada para evaluar habilidades en desarrollo fullstack con .NET y React/Next.js. El objetivo es demostrar competencias en arquitectura de software, desarrollo de APIs, gestión de bases de datos y integración frontend-backend.

```

```

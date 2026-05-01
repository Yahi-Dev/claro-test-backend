# ClaroTest.Backend

API REST en .NET 9 que actúa como **proxy** entre el frontend y la API pública [FakeRestAPI](https://fakerestapi.azurewebsites.net/index.html). Implementa **Clean Architecture (Onion)** con **CQRS + MediatR**, validación con FluentValidation, AutoMapper, manejo global de errores y resiliencia HTTP.

---

## Stack

| Componente | Versión |
|---|---|
| .NET SDK | **9.0** |
| MediatR | 12.4.1 |
| AutoMapper | 15.1.3 |
| FluentValidation | 11.11.0 |
| Asp.Versioning | 8.1.0 |
| Swashbuckle (Swagger) | 7.2.0 |
| Microsoft.Extensions.Http.Resilience (Polly) | 9.0.0 |
| xUnit + Moq + FluentAssertions | latest |

---

## Arquitectura

Onion / Clean Architecture con cinco proyectos:

```
ClaroTest.Backend.sln
├── src/
│   ├── ClaroTest.Core.Domain                  Entidades puras (Book, Author, BaseEntity). Sin dependencias.
│   ├── ClaroTest.Core.Application             CQRS+MediatR, ViewModels, Wrappers, Validators, AutoMapper, IOC.
│   │                                          Solo depende de Domain.
│   ├── ClaroTest.Infrastructure.ExternalApi   Repositorios sobre HttpClient tipado contra FakeRestAPI.
│   │                                          Resiliencia con Polly (retry exponencial, circuit breaker, timeout).
│   └── ClaroTest.Presentation.WebApi          Controllers, middleware de errores, Swagger, versionado, CORS.
└── tests/
    └── ClaroTest.Tests                        Tests unitarios (handlers, validators, profile, repositorios).
```

### Flujo de una petición

```
HTTP request
   ↓
Controller (BooksController / AuthorsController)
   ↓ Mediator.Send(command|query)
ValidationBehavior  (pipeline FluentValidation)
   ↓
Handler  (Application/Features/...)
   ↓
IBookRepository | IAuthorRepository  (Application/Interfaces)
   ↓
BookRepository | AuthorRepository    (Infrastructure)
   ↓ HttpClient (Polly resilience)
FakeRestAPI
```

### Patrones aplicados

- **CQRS + MediatR** — un Command/Query por caso de uso, handlers desacoplados de los controllers.
- **Repository pattern** sobre `HttpClient` tipado con `IHttpClientFactory`.
- **Pipeline behavior** de validación — todo command pasa por `FluentValidation` antes de tocar el handler.
- **`Response<T>` wrapper** consistente en todas las respuestas (`{ succeeded, message, errors, data }`).
- **`ErrorHandleMiddleware`** global — convierte `ValidationException` → 400, `NotFoundException` → 404, `ApiException` → status code, otras → 500.
- **Resiliencia HTTP** con `AddStandardResilienceHandler` (Polly): retry exponencial, circuit breaker, attempt timeout (15s), total timeout (60s).
- **Extension methods** por capa para registro de servicios (`AddApplicationLayer`, `AddExternalApiInfrastructure`, `AddSwaggerExtension`, `AddApiVersioningExtension`, `AddCorsExtension`).
- **API Versioning** vía URL (`/api/v1/...`) y default version 1.0.
- **CORS** restringido a orígenes configurables (default: `localhost:4200` para Angular dev).

---

## Endpoints

Base URL en desarrollo: `http://localhost:5099`. Todas las respuestas están envueltas en `Response<T>`.

### Books

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Books` | Lista todos los libros |
| GET | `/api/Books/{id}` | Obtiene un libro por id |
| POST | `/api/Books` | Crea un libro |
| PUT | `/api/Books/{id}` | Actualiza un libro |
| DELETE | `/api/Books/{id}` | Elimina un libro |

### Authors

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Authors` | Lista todos los autores con `bookCount` agregado |
| GET | `/api/Authors/{id}` | Obtiene un autor por id |
| GET | `/api/Authors/by-book/{idBook}` | Autores asociados a un libro |
| POST | `/api/Authors` | Crea un autor |
| PUT | `/api/Authors/{id}` | Actualiza un autor |
| DELETE | `/api/Authors/{id}` | Elimina un autor |

### Swagger

`http://localhost:5099/swagger`

---

## Instalación y ejecución

### Requisitos previos

- **.NET 9 SDK** o superior — [descarga oficial](https://dotnet.microsoft.com/download)
- (Opcional) Visual Studio 2022 17.12+ / Rider 2025.x / VS Code con extensión C# Dev Kit

### Pasos

```bash
# 1. Clonar el repositorio
git clone https://github.com/Yahi-Dev/claro-test-backend.git
cd claro-test-backend

# 2. Restaurar dependencias
dotnet restore

# 3. Compilar la solución
dotnet build

# 4. Ejecutar los tests (opcional, 14 tests unitarios)
dotnet test

# 5. Ejecutar la API
cd src/ClaroTest.Presentation.WebApi
dotnet run
```

Una vez arriba, la API queda escuchando en:

| Protocolo | URL | Uso |
|---|---|---|
| HTTP  | `http://localhost:5099`  | Frontend Angular y curl |
| HTTPS | `https://localhost:7277` | Browser y Swagger UI |

Puntos de entrada:

- **Swagger UI** → [http://localhost:5099/swagger](http://localhost:5099/swagger) (o `https://localhost:7277/swagger`)
- **OpenAPI JSON** → `http://localhost:5099/swagger/v1/swagger.json`
- **Archivo `.http`** → `src/ClaroTest.Presentation.WebApi/ClaroTest.WebApi.http` (compatible con la extensión REST Client de VS Code y con Visual Studio).

> Si abres Swagger en HTTPS por primera vez y el navegador advierte sobre el certificado, ejecuta una sola vez `dotnet dev-certs https --trust` para confiar en el certificado de desarrollo de .NET.

### Ejecución desde Visual Studio 2022

1. Abre `ClaroTest.Backend.sln`.
2. Asegúrate de que **`ClaroTest.Presentation.WebApi`** sea el proyecto de inicio (clic derecho → *Set as Startup Project*).
3. Selecciona el perfil **`https`** o **`http`** del dropdown.
4. F5 (debug) o Ctrl+F5 (sin debug). El navegador abre `/swagger` automáticamente.

> Si Visual Studio reporta `MSB3027 / MSB3021` ("file is locked by ClaroTest.Presentation.WebApi"), significa que hay una instancia previa del API corriendo. Ciérrala con `taskkill /F /IM dotnet.exe`, ejecuta **Build → Clean Solution** y luego **Rebuild**.

### Configuración

`src/ClaroTest.Presentation.WebApi/appsettings.json`:

```json
{
  "FakeRestApi": {
    "BaseUrl": "https://fakerestapi.azurewebsites.net/",
    "TimeoutSeconds": 30
  },
  "Cors": {
    "AllowedOrigins": [ "http://localhost:4200" ]
  }
}
```

Para sobreescribir en local sin tocar el archivo versionado, crea `appsettings.Local.json` (ignorado por `.gitignore`) o usa variables de entorno con prefijo `FakeRestApi__BaseUrl=...`.

---

## Tests

```bash
dotnet test
```

Cobertura actual:

- **Profile** — `AssertConfigurationIsValid()` y mapping de commands.
- **Handlers (Books)** — GetAll, GetById (NotFoundException), Create, Delete.
- **Handlers (Authors)** — agregación de `BookCount` por nombre completo case-insensitive.
- **Validators** — reglas de `CreateBookCommand` (título requerido, page count no negativo, payload válido).
- **Repositorios HTTP** — deserialización, propagación de 404 → null o `NotFoundException`, propagación de 5xx → `ApiException` (con `MockHttpMessageHandler` propio que captura los requests).

Total: **14 tests, 0 fallos**.

---

## Decisiones de diseño

- **No se usa base de datos** — la consigna lo prohíbe; los repositorios delegan a FakeRestAPI vía HTTP.
- **No autenticación** — fuera del alcance de la prueba.
- **`Response<T>` siempre** — el frontend tiene un contrato uniforme, sin parsear status codes para casos felices.
- **CQRS aunque el dominio sea simple** — habilita escalabilidad: agregar nuevos casos de uso es crear una nueva carpeta `Features/...`, no tocar handlers existentes.
- **FluentValidation sobre DataAnnotations** — el pipeline behavior aplica validación uniformemente sin "ensuciar" controllers ni ViewModels.
- **Pinning de `Microsoft.OpenApi 1.6.24`** — Swashbuckle 7.2 todavía no soporta la API rota de OpenApi 2.x; cuando Swashbuckle libere una versión compatible se puede subir.

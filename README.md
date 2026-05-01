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
| Swashbuckle (Swagger) | 6.7.3 |
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

| Requisito | Versión | Cómo verificar | Descarga |
|---|---|---|---|
| **.NET SDK** | 9.0 o superior | `dotnet --version` | [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) |
| Git | cualquiera reciente | `git --version` | [git-scm.com](https://git-scm.com/downloads) |

> **Antes de empezar**, ejecuta una sola vez en cualquier terminal:
> ```bash
> dotnet dev-certs https --trust
> ```
> Esto confía en el certificado de desarrollo de .NET y evita la advertencia roja del navegador al abrir `https://localhost:7277/swagger`.

### Clonar el repositorio

```bash
git clone https://github.com/Yahi-Dev/claro-test-backend.git
cd claro-test-backend
```

A partir de aquí elige el camino que prefieras: **Visual Studio 2022**, **Visual Studio Code** o **terminal directa**.

---

### Opción 1 · Visual Studio 2022 (recomendado en Windows)

**Versión mínima:** Visual Studio 2022 17.12 o superior, con la carga de trabajo *ASP.NET y desarrollo web*.

1. **File → Open → Project/Solution** y selecciona `ClaroTest.Backend.sln` en la raíz del repo.
2. Espera a que VS termine de restaurar paquetes NuGet (barra de progreso abajo). Si no lo hace solo, clic derecho sobre la solución → **Restore NuGet Packages**.
3. En el **Solution Explorer**, clic derecho sobre `ClaroTest.Presentation.WebApi` → **Set as Startup Project** (debe quedar en negrita).
4. En la barra de herramientas superior, junto al botón *Run*, aparece un dropdown con perfiles. Selecciona:
   - **`https`** → arranca en `https://localhost:7277` y `http://localhost:5099` *(recomendado)*.
   - **`http`** → solo `http://localhost:5099`.
5. Ejecuta:
   - **F5** → debug con breakpoints habilitados.
   - **Ctrl+F5** → ejecutar sin debug (más rápido).
6. El navegador se abrirá automáticamente en `/swagger`.

**Para correr los tests:** menú **Test → Run All Tests** (o `Ctrl+R, A`). Los 14 tests deben pasar en menos de 2 segundos.

> **Si VS muestra `MSB3027` o `MSB3021` ("file is locked by ClaroTest.Presentation.WebApi"):** es que hay una instancia previa del API aún corriendo en segundo plano. Soluciónalo con:
> 1. Cerrar todas las pestañas de navegador apuntando al API.
> 2. En cmd: `taskkill /F /IM dotnet.exe` (mata cualquier dotnet residual).
> 3. En VS: **Build → Clean Solution**, luego **Build → Rebuild Solution**.

---

### Opción 2 · Visual Studio Code

**Extensión obligatoria:** [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) (incluye también C# y .NET Install Tool).
**Recomendadas:** [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) para usar el archivo `.http` incluido.

1. Abre la carpeta del repo: **File → Open Folder...** y selecciona `claro-test-backend`.
2. VS Code detectará la solución `.sln`. Acepta cuando te pregunte si deseas restaurar dependencias.
3. Abre una terminal integrada (**Ctrl+`**) y ejecuta:

   ```bash
   dotnet build
   dotnet test          # opcional: corre los 14 tests unitarios
   ```

4. Para arrancar el API tienes dos opciones:

   **a) Con debugger** — en el panel **Run and Debug** (`Ctrl+Shift+D`), selecciona la configuración **`C#: ClaroTest.Presentation.WebApi`** generada automáticamente y presiona F5.

   **b) Sin debugger** — desde la terminal:

   ```bash
   cd src/ClaroTest.Presentation.WebApi
   dotnet run --launch-profile https
   ```

5. El API queda escuchando. Abre `https://localhost:7277/swagger` en el navegador.

---

### Opción 3 · Terminal (cualquier sistema operativo)

```bash
# 1. Restaurar dependencias
dotnet restore

# 2. Compilar
dotnet build

# 3. (Opcional) Ejecutar los 14 tests unitarios
dotnet test

# 4. Arrancar el API en https + http
cd src/ClaroTest.Presentation.WebApi
dotnet run --launch-profile https

# Para detener: Ctrl+C en la misma terminal.
```

---

### URLs disponibles cuando el API está arriba

| Recurso | URL |
|---|---|
| **Swagger UI** (recomendado para explorar) | [https://localhost:7277/swagger](https://localhost:7277/swagger) |
| Swagger UI sobre HTTP | http://localhost:5099/swagger |
| OpenAPI JSON crudo | http://localhost:5099/swagger/v1/swagger.json |
| Endpoints API (consumo desde frontend / curl) | http://localhost:5099/api/Books, /api/Authors |

### Probar los endpoints

Tres formas, elige la que prefieras:

1. **Swagger UI** — abre `https://localhost:7277/swagger`, expande cualquier endpoint, clic en *Try it out* → *Execute*.
2. **Archivo `.http`** — abre `src/ClaroTest.Presentation.WebApi/ClaroTest.WebApi.http`. En VS 2022 y en VS Code (con la extensión REST Client) aparecerá un botón *Send Request* sobre cada bloque.
3. **curl** — por ejemplo:

   ```bash
   curl http://localhost:5099/api/Books | jq
   curl http://localhost:5099/api/Authors/by-book/1 | jq
   ```

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

### Solución de problemas comunes

| Síntoma | Causa probable | Solución |
|---|---|---|
| Swagger muestra *"Unable to render this definition"* | Caché del navegador con un swagger.json viejo | Hard refresh con `Ctrl+Shift+R`. Si persiste, abrir DevTools → Network → marcar *Disable cache* y recargar. |
| Advertencia de certificado al abrir HTTPS | Cert de desarrollo no confiado | Ejecutar `dotnet dev-certs https --trust` y reiniciar el navegador. |
| `MSB3027` / `MSB3021` "file is locked" al compilar | Una instancia previa del API quedó corriendo | `taskkill /F /IM dotnet.exe` (Windows) o `pkill dotnet` (Linux/Mac), luego rebuild. |
| `Unable to bind to https://localhost:7277` | Otro proceso ocupa el puerto | `netstat -ano \| findstr :7277` para ver el PID culpable, o cambiar el puerto en `Properties/launchSettings.json`. |
| `dotnet run` falla con *"Could not find a part of the path 'bin/Debug/net9.0/...'"* | Falta restaurar/compilar | Ejecutar `dotnet restore && dotnet build` primero. |
| Frontend no puede llamar al API por CORS | El origen del frontend no está en `appsettings.json` | Agregar la URL del frontend a `Cors:AllowedOrigins` y reiniciar el API. |

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
- **Swashbuckle 6.7.3 en lugar de 7.x** — Swashbuckle 7.x emite `openapi: 3.0.4`, una versión muy reciente del spec que algunos bundles de Swagger UI todavía no parsean correctamente y produce el error "Unable to render this definition". 6.7.3 emite `openapi: 3.0.1`, el más universalmente compatible. Sin dependencia directa a `Microsoft.OpenApi`.

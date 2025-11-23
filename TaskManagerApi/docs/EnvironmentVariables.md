# .env Configuration

## 1. Loading `.env`

Right at the beginning of `Program.cs`, there is this snippet:

```csharp
if (builder.Environment.IsDevelopment())
{
  Env.Load();
}

builder.Configuration.AddEnvironmentVariables();
```

**What happens here:**

* `Env.Load()` reads the `.env` file located at the API project root (`TaskManagerApi/.env`);
* Each variable found in `.env` is **injected into system environment variables** (as if you had exported them in the terminal);
* Then, `builder.Configuration.AddEnvironmentVariables()` makes ASP.NET Core also fetch values from the environment (including those loaded by DotNetEnv).

In other words: any value defined in `.env` **overwrites** the corresponding value in `appsettings.json`.

---

## 2. How values are accessed in code

After `.env` is loaded, ASP.NET Core configurations can read these values automatically.
For example:

### Connection String (database)

```csharp
var cs = builder.Configuration["ConnectionStrings__Default"];
```

These values come from `.env` if it has something like:

```env
ConnectionStrings__Default=Host=localhost;Database=TaskManager;Username=postgres;Password=admin;
```

Note that we use **two underscores (`__`)** to indicate hierarchy (`Jwt:Key` → `Jwt__Key`).

---

#### CORS

```env
Cors__AllowedOrigins=http://localhost:5173,https://app.seusite.com
```

And in the code:

```csharp
var allowedOrigins = builder.Configuration["Cors__AllowedOrigins"]?.Split(',');
```

---

## 3. Security and Best Practices

* The `.env` file **should not be versioned** — that's why it is listed in `.gitignore`.
* In **production**, variables can be defined directly in the environment (e.g., in Docker, Azure, AWS, or other server settings).
* This ensures no secrets (like passwords or JWT keys) appear in the source code.

Development `.env` file:

```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__Default=Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres
Cors__AllowedOrigins=http://localhost:5173
Jwt__Issuer=TaskManagerApi
Jwt__Audience=TaskManagerApi
Jwt__Key=E%7@J5@4#1IGn&!T2p6hPEE%6x$5%X@1
```

---

### Summary:

| Step | What happens                                                              |
| ---- | ------------------------------------------------------------------------- |
| 1️⃣   | `Env.Load()` reads `.env` and injects variables into the environment      |
| 2️⃣   | `AddEnvironmentVariables()` makes ASP.NET Core recognize them             |
| 3️⃣   | `builder.Configuration` reads these values at runtime                     |
| 4️⃣   | Configurations (DB, JWT, CORS, etc.) use these values automatically       |

---

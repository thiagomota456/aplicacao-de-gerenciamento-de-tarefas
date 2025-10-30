# Documetação do .env
## 1. Carregamento do `.env`

Logo no início do `Program.cs`, há este trecho:

```csharp
if (builder.Environment.IsDevelopment())
{
  Env.Load();
}

builder.Configuration.AddEnvironmentVariables();
```

O que acontece aqui:

* `Env.Load()` lê o arquivo `.env` que está na raiz do projeto da API (`TaskManagerApi/.env`);
* Cada variável encontrada no `.env` é **injetada nas variáveis de ambiente do sistema** (como se você tivesse exportado elas no terminal);
* Em seguida, `builder.Configuration.AddEnvironmentVariables()` faz com que o ASP.NET Core também busque valores do ambiente (inclusive os carregados pelo DotNetEnv).

Ou seja: qualquer valor definido no `.env` **sobrescreve** o valor correspondente no `appsettings.json`.

---

## 2. Como os valores são acessados no código

Depois que o `.env` é carregado, as configurações do ASP.NET Core conseguem ler esses valores automaticamente.
Por exemplo:

### Connection String (banco de dados)

```csharp
var cs = builder.Configuration["ConnectionStrings__Default"];
```

Esses valores vêm do `.env`, se ele tiver algo como:

```env
ConnectionStrings__Default=Host=localhost;Database=TaskManager;Username=postgres;Password=admin;
```

Note que usamos **dois underscores (`__`)** para indicar a hierarquia (`Jwt:Key` → `Jwt__Key`).

---

#### CORS

```env
Cors__AllowedOrigins=http://localhost:5173,https://app.seusite.com
```

E no código:

```csharp
var allowedOrigins = builder.Configuration["Cors__AllowedOrigins"]?.Split(',');
```

---

## 3. Segurança e boas práticas

* O arquivo `.env` **não deve ser versionado** — por isso ele está listado no `.gitignore`.
* Em **produção**, as variáveis podem ser definidas diretamente no ambiente (por exemplo, nas configurações do Docker, Azure, AWS ou outro servidor).
* Isso garante que nenhum segredo (como senhas ou chaves JWT) apareça no código-fonte.

---

### Em resumo:

| Etapa | O que acontece                                                            |
| ----- | ------------------------------------------------------------------------- |
| 1️⃣   | `Env.Load()` lê o `.env` e injeta variáveis no ambiente                   |
| 2️⃣   | `AddEnvironmentVariables()` faz o ASP.NET Core reconhecê-las              |
| 3️⃣   | O `builder.Configuration` lê esses valores em tempo de execução           |
| 4️⃣   | As configurações (DB, JWT, CORS, etc.) usam esses valores automaticamente |

---
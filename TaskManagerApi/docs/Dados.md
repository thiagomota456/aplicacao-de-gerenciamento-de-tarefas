# Banco de dados — instruções rápidas

Este guia mostra **como criar a migration inicial**, **gerar o banco** e **popular com dados de exemplo**.
Funciona com **EF Core** e **PostgreSQL** (DDL abaixo).

---

## ✅ Passo a passo

### 1) Criar a primeira migration

```bash
dotnet ef migrations add InitialCreate
```

### 2) Criar/atualizar o banco (a partir das migrations)

```bash
dotnet ef database update
```

**O que esses comandos fazem:**

* Criam a pasta `Migrations/`;
* Geram o SQL para criar as tabelas conforme seus modelos;
* Aplicam as alterações no banco configurado no `DbContext`.

> 💡 Dica: garanta que o `dotnet-ef` está instalado (`dotnet tool install --global dotnet-ef`) e que a **connection string** do seu banco está correta no projeto.

---

## 🧱 Esquema do banco (DDL)

> Use se precisar criar/validar a estrutura diretamente no banco.

```sql
CREATE TABLE users (
    "Id" uuid NOT NULL,
    "Username" text NOT NULL,
    "PasswordHash" text NOT NULL,
    "CreatedAt" timestamptz NOT NULL,
    CONSTRAINT "PK_users" PRIMARY KEY ("Id")
);

CREATE TABLE categories (
    "Id" int4 NOT NULL,
    "UserId" uuid NOT NULL,
    "Description" text NULL,
    CONSTRAINT "PK_categories" PRIMARY KEY ("UserId", "Id"),
    CONSTRAINT "FK_categories_users_UserId" FOREIGN KEY ("UserId") REFERENCES users("Id") ON DELETE CASCADE
);
CREATE INDEX "IX_categories_UserId_Description" ON public.categories USING btree ("UserId", "Description");

CREATE TABLE tasks (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Title" varchar(160) NOT NULL,
    "Description" text NOT NULL,
    "IsCompleted" bool NOT NULL,
    "CategoryId" int4 NOT NULL,
    "Created" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NOT NULL,
    CONSTRAINT "PK_tasks" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_tasks_categories_UserId_CategoryId" FOREIGN KEY ("UserId","CategoryId") REFERENCES categories("UserId","Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_tasks_users_UserId" FOREIGN KEY ("UserId") REFERENCES users("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_tasks_UserId_CategoryId" ON public.tasks USING btree ("UserId", "CategoryId");
CREATE INDEX "IX_tasks_UserId_IsCompleted" ON public.tasks USING btree ("UserId", "IsCompleted");

```

---

## 🧪 Dados de exemplo (seed)

> Executar este script **após** a estrutura existir.
> As senhas são **mocks**. (Todas as senhas são admin12345)

```sql
BEGIN;
-- Opcional: limpar dados mantendo a estrutura
-- TRUNCATE TABLE tasks, categories, users RESTART IDENTITY CASCADE;

-- =====================
-- USERS
-- =====================
INSERT INTO public.users ("Id","Username","PasswordHash","CreatedAt") VALUES
('8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','alice','$2a$11$S5WrXqkfaM/mj6t7H0oKZ.n6roFh.vFluYSl56SssGBPo5Dl/mHTm','2025-05-05 09:12:00+00'),
('9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','bruno','$2a$11$S5WrXqkfaM/mj6t7H0oKZ.n6roFh.vFluYSl56SssGBPo5Dl/mHTm','2025-04-18 14:40:00+00'),
('0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','carla','$2a$11$S5WrXqkfaM/mj6t7H0oKZ.n6roFh.vFluYSl56SssGBPo5Dl/mHTm','2025-03-22 08:25:00+00');

-- =====================
-- CATEGORIES (por usuário)
-- PK: ("UserId","Id")
-- =====================
-- Alice
INSERT INTO public.categories ("Id","UserId","Description") VALUES
(1,'8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Pessoal'),
(2,'8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Trabalho'),
(3,'8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Saúde'),
(4,'8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Finanças');

-- Bruno
INSERT INTO public.categories ("Id","UserId","Description") VALUES
(1,'9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Pessoal'),
(2,'9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Projetos'),
(3,'9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Família'),
(4,'9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Lazer');

-- Carla
INSERT INTO public.categories ("Id","UserId","Description") VALUES
(1,'0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Pessoal'),
(2,'0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Pesquisa'),
(3,'0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Viagens'),
(4,'0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Saúde');

-- =====================
-- TASKS
-- =====================
-- Alice
INSERT INTO public.tasks
("Id","UserId","Title","Description","IsCompleted","CategoryId","Created","UpdatedAt") VALUES
('11111111-1111-4111-8111-aaaaaaaaaaa1','8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Marcar consulta com dentista','Ligar para a clínica e agendar limpeza semestral.',false,3,'2025-08-10 10:00:00+00','2025-08-10 10:05:00+00'),
('11111111-1111-4111-8111-aaaaaaaaaaa2','8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Revisar orçamento mensal','Atualizar planilha e conferir gastos do cartão.',true,4,'2025-08-01 18:20:00+00','2025-08-02 07:10:00+00'),
('11111111-1111-4111-8111-aaaaaaaaaaa3','8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Preparar apresentação sprint','Slide deck com métricas e próximos passos.',true,2,'2025-09-05 09:15:00+00','2025-09-06 11:30:00+00'),
('11111111-1111-4111-8111-aaaaaaaaaaa4','8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Comprar presente para Ana','Ideias: livro, chocolate artesanal, vela aromática.',false,1,'2025-09-28 16:45:00+00','2025-10-01 12:00:00+00'),
('11111111-1111-4111-8111-aaaaaaaaaaa5','8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Backup das notas do celular','Verificar iCloud/Drive e espaço disponível.',true,1,'2025-07-22 21:00:00+00','2025-07-22 21:03:00+00'),
('11111111-1111-4111-8111-aaaaaaaaaaa6','8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Atualizar status do projeto X','Enviar update no Slack com bloqueios e riscos.',false,2,'2025-10-12 08:40:00+00','2025-10-12 08:41:00+00');

-- Bruno
INSERT INTO public.tasks
("Id","UserId","Title","Description","IsCompleted","CategoryId","Created","UpdatedAt") VALUES
('22222222-2222-4222-8222-bbbbbbbbbbb1','9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Planejar churrasco de família','Definir lista de compras e confirmar presença.',false,3,'2025-08-14 12:10:00+00','2025-08-14 12:20:00+00'),
('22222222-2222-4222-8222-bbbbbbbbbbb2','9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Refatorar módulo de pagamentos','Separar responsabilidades e cobrir com testes.',true,2,'2025-07-30 10:00:00+00','2025-08-02 16:30:00+00'),
('22222222-2222-4222-8222-bbbbbbbbbbb3','9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Comprar passagens para o feriado','Pesquisar preços e horários noturnos.',true,4,'2025-09-03 19:25:00+00','2025-09-03 20:00:00+00'),
('22222222-2222-4222-8222-bbbbbbbbbbb4','9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Check-up anual','Exames de sangue e consulta clínica geral.',false,1,'2025-10-05 07:55:00+00','2025-10-07 09:00:00+00'),
('22222222-2222-4222-8222-bbbbbbbbbbb5','9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Criar cronograma do app side-project','Roadmap MVP e milestones quinzenais.',false,2,'2025-08-22 13:40:00+00','2025-08-22 13:45:00+00'),
('22222222-2222-4222-8222-bbbbbbbbbbb6','9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Pagar IPVA','Verificar desconto e emitir guia.',true,1,'2025-06-10 09:00:00+00','2025-06-10 09:10:00+00');

-- Carla
INSERT INTO public.tasks
("Id","UserId","Title","Description","IsCompleted","CategoryId","Created","UpdatedAt") VALUES
('33333333-3333-4333-8333-ccccccccccc1','0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Desenhar protocolo do estudo','Hipóteses, indicadores e plano de análise.',true,2,'2025-05-16 11:30:00+00','2025-05-18 08:00:00+00'),
('33333333-3333-4333-8333-ccccccccccc2','0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Reservar pousada para congresso','Perto do centro de convenções, com café da manhã.',true,3,'2025-07-02 15:20:00+00','2025-07-02 15:50:00+00'),
('33333333-3333-4333-8333-ccccccccccc3','0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Consulta com nutricionista','Montar cardápio semanal equilibrado.',false,4,'2025-09-21 09:10:00+00','2025-09-21 09:12:00+00'),
('33333333-3333-4333-8333-ccccccccccc4','0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Organizar fotos da viagem','Selecionar e criar álbum compartilhado.',false,1,'2025-10-03 20:05:00+00','2025-10-04 10:00:00+00'),
('33333333-3333-4333-8333-ccccccccccc5','0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Revisar bibliografia do paper','Checar referências e DOI.',true,2,'2025-08-11 08:45:00+00','2025-08-12 18:00:00+00'),
('33333333-3333-4333-8333-ccccccccccc6','0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Comprar seguro de viagem','Cobertura médica internacional e extravio.',false,3,'2025-09-30 17:30:00+00','2025-10-01 09:00:00+00');

COMMIT;
```
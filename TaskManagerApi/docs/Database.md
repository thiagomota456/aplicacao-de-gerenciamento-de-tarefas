# Database Configuration Guide

This guide shows **how to create the initial migration**, **generate the database**, and **seed it with sample data**.
Works with **EF Core** and **PostgreSQL** (DDL below).

---

## ✅ Step by Step

### 1) Create the initial migration

```bash
dotnet ef migrations add InitialCreate
```

### 2) Create/update the database (from migrations)

```bash
dotnet ef database update
```

**What these commands do:**

* Create the `Migrations/` folder;
* Generate the SQL to create tables according to your models;
* Apply changes to the database configured in `DbContext`.

> 💡 Tip: ensure `dotnet-ef` is installed (`dotnet tool install --global dotnet-ef`) and your database **connection string** is correct in the project.

---

## 🧱 Database Schema (DDL)

> Use if you need to create/validate the structure directly in the database.

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

## 🧪 Sample Data (Seed)

> Execute this script **after** the structure exists.
> Passwords are **mocks**. (All passwords are admin12345)

<details>
<summary>📄 Click to expand SQL Seed script</summary>

```sql
BEGIN;
-- Optional: clear data keeping structure
-- TRUNCATE TABLE tasks, categories, users RESTART IDENTITY CASCADE;

-- =====================
-- USERS
-- =====================
INSERT INTO public.users ("Id","Username","PasswordHash","CreatedAt") VALUES
('8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','alice','$2a$11$S5WrXqkfaM/mj6t7H0oKZ.n6roFh.vFluYSl56SssGBPo5Dl/mHTm','2025-05-05 09:12:00+00'),
('9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','bruno','$2a$11$S5WrXqkfaM/mj6t7H0oKZ.n6roFh.vFluYSl56SssGBPo5Dl/mHTm','2025-04-18 14:40:00+00'),
('0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','carla','$2a$11$S5WrXqkfaM/mj6t7H0oKZ.n6roFh.vFluYSl56SssGBPo5Dl/mHTm','2025-03-22 08:25:00+00');

-- =====================
-- CATEGORIES (per user)
-- PK: ("UserId","Id")
-- =====================
-- Alice
INSERT INTO public.categories ("Id","UserId","Description") VALUES
(1,'8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Personal'),
(2,'8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Work'),
(3,'8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Health'),
(4,'8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Finance');

-- Bruno
INSERT INTO public.categories ("Id","UserId","Description") VALUES
(1,'9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Personal'),
(2,'9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Projects'),
(3,'9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Family'),
(4,'9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Leisure');

-- Carla
INSERT INTO public.categories ("Id","UserId","Description") VALUES
(1,'0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Personal'),
(2,'0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Research'),
(3,'0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Travel'),
(4,'0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Health');

-- =====================
-- TASKS
-- =====================
-- Alice
INSERT INTO public.tasks
("Id","UserId","Title","Description","IsCompleted","CategoryId","Created","UpdatedAt") VALUES
('11111111-1111-4111-8111-aaaaaaaaaaa1','8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Book dentist appointment','Call the clinic and schedule cleaning.',false,3,'2025-08-10 10:00:00+00','2025-08-10 10:05:00+00'),
('11111111-1111-4111-8111-aaaaaaaaaaa2','8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Review monthly budget','Update spreadsheet and check credit card expenses.',true,4,'2025-08-01 18:20:00+00','2025-08-02 07:10:00+00'),
('11111111-1111-4111-8111-aaaaaaaaaaa3','8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Prepare sprint presentation','Slide deck with metrics and next steps.',true,2,'2025-09-05 09:15:00+00','2025-09-06 11:30:00+00'),
('11111111-1111-4111-8111-aaaaaaaaaaa4','8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Buy gift for Ana','Ideas: book, handmade chocolate, scented candle.',false,1,'2025-09-28 16:45:00+00','2025-10-01 12:00:00+00'),
('11111111-1111-4111-8111-aaaaaaaaaaa5','8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Backup phone notes','Check iCloud/Drive and available space.',true,1,'2025-07-22 21:00:00+00','2025-07-22 21:03:00+00'),
('11111111-1111-4111-8111-aaaaaaaaaaa6','8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701','Update Project X status','Send update on Slack with blockers and risks.',false,2,'2025-10-12 08:40:00+00','2025-10-12 08:41:00+00');

-- Bruno
INSERT INTO public.tasks
("Id","UserId","Title","Description","IsCompleted","CategoryId","Created","UpdatedAt") VALUES
('22222222-2222-4222-8222-bbbbbbbbbbb1','9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Plan family BBQ','Define shopping list and confirm attendance.',false,3,'2025-08-14 12:10:00+00','2025-08-14 12:20:00+00'),
('22222222-2222-4222-8222-bbbbbbbbbbb2','9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Refactor payment module','Separate responsibilities and cover with tests.',true,2,'2025-07-30 10:00:00+00','2025-08-02 16:30:00+00'),
('22222222-2222-4222-8222-bbbbbbbbbbb3','9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Buy tickets for holiday','Search prices and night schedules.',true,4,'2025-09-03 19:25:00+00','2025-09-03 20:00:00+00'),
('22222222-2222-4222-8222-bbbbbbbbbbb4','9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Annual check-up','Blood tests and general clinical consultation.',false,1,'2025-10-05 07:55:00+00','2025-10-07 09:00:00+00'),
('22222222-2222-4222-8222-bbbbbbbbbbb5','9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Create side-project schedule','MVP roadmap and bi-weekly milestones.',false,2,'2025-08-22 13:40:00+00','2025-08-22 13:45:00+00'),
('22222222-2222-4222-8222-bbbbbbbbbbb6','9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802','Pay car tax','Check discount and issue slip.',true,1,'2025-06-10 09:00:00+00','2025-06-10 09:10:00+00');

-- Carla
INSERT INTO public.tasks
("Id","UserId","Title","Description","IsCompleted","CategoryId","Created","UpdatedAt") VALUES
('33333333-3333-4333-8333-ccccccccccc1','0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Design study protocol','Hypotheses, indicators, and analysis plan.',true,2,'2025-05-16 11:30:00+00','2025-05-18 08:00:00+00'),
('33333333-3333-4333-8333-ccccccccccc2','0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Book inn for congress','Near convention center, with breakfast.',true,3,'2025-07-02 15:20:00+00','2025-07-02 15:50:00+00'),
('33333333-3333-4333-8333-ccccccccccc3','0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Nutritionist appointment','Create balanced weekly menu.',false,4,'2025-09-21 09:10:00+00','2025-09-21 09:12:00+00'),
('33333333-3333-4333-8333-ccccccccccc4','0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Organize trip photos','Select and create shared album.',false,1,'2025-10-03 20:05:00+00','2025-10-04 10:00:00+00'),
('33333333-3333-4333-8333-ccccccccccc5','0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Review paper bibliography','Check references and DOI.',true,2,'2025-08-11 08:45:00+00','2025-08-12 18:00:00+00'),
('33333333-3333-4333-8333-ccccccccccc6','0c3c7d30-5e4f-4bac-8d3e-34c56d78e903','Buy travel insurance','International medical coverage and lost luggage.',false,3,'2025-09-30 17:30:00+00','2025-10-01 09:00:00+00');

COMMIT;
```
</details>

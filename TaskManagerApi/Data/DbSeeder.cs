using TaskManagerApi.Models;

namespace TaskManagerApi.Data;

public static class DbSeeder
{
    public static void Seed(TaskDbContext context)
    {
        // Se já existem usuários, não faz nada
        if (context.Users.Any())
        {
            return;
        }

        // =====================
        // USERS
        // =====================
        var aliceId = Guid.Parse("8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701");
        var brunoId = Guid.Parse("9b2b6c2f-4d3e-4a9b-8c2d-23b45c67d802");
        var carlaId = Guid.Parse("0c3c7d30-5e4f-4bac-8d3e-34c56d78e903");

        var users = new[]
        {
            new User
            {
                Id = aliceId,
                Username = "alice",
                PasswordHash = "$2a$11$S5WrXqkfaM/mj6t7H0oKZ.n6roFh.vFluYSl56SssGBPo5Dl/mHTm",
                CreatedAt = DateTime.Parse("2025-05-05 09:12:00+00").ToUniversalTime()
            },
            new User
            {
                Id = brunoId,
                Username = "bruno",
                PasswordHash = "$2a$11$S5WrXqkfaM/mj6t7H0oKZ.n6roFh.vFluYSl56SssGBPo5Dl/mHTm",
                CreatedAt = DateTime.Parse("2025-04-18 14:40:00+00").ToUniversalTime()
            },
            new User
            {
                Id = carlaId,
                Username = "carla",
                PasswordHash = "$2a$11$S5WrXqkfaM/mj6t7H0oKZ.n6roFh.vFluYSl56SssGBPo5Dl/mHTm",
                CreatedAt = DateTime.Parse("2025-03-22 08:25:00+00").ToUniversalTime()
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();

        // =====================
        // CATEGORIES
        // =====================
        var categories = new List<Category>();

        // Alice
        categories.Add(new Category { Id = 1, UserId = aliceId, Description = "Pessoal" });
        categories.Add(new Category { Id = 2, UserId = aliceId, Description = "Trabalho" });
        categories.Add(new Category { Id = 3, UserId = aliceId, Description = "Saúde" });
        categories.Add(new Category { Id = 4, UserId = aliceId, Description = "Finanças" });

        // Bruno
        categories.Add(new Category { Id = 1, UserId = brunoId, Description = "Pessoal" });
        categories.Add(new Category { Id = 2, UserId = brunoId, Description = "Projetos" });
        categories.Add(new Category { Id = 3, UserId = brunoId, Description = "Família" });
        categories.Add(new Category { Id = 4, UserId = brunoId, Description = "Lazer" });

        // Carla
        categories.Add(new Category { Id = 1, UserId = carlaId, Description = "Pessoal" });
        categories.Add(new Category { Id = 2, UserId = carlaId, Description = "Pesquisa" });
        categories.Add(new Category { Id = 3, UserId = carlaId, Description = "Viagens" });
        categories.Add(new Category { Id = 4, UserId = carlaId, Description = "Saúde" });

        context.Categories.AddRange(categories);
        context.SaveChanges();

        // =====================
        // TASKS
        // =====================
        var tasks = new List<Models.Task>();

        // Alice
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("11111111-1111-4111-8111-aaaaaaaaaaa1"),
            UserId = aliceId,
            Title = "Marcar consulta com dentista",
            Description = "Ligar para a clínica e agendar limpeza semestral.",
            IsCompleted = false,
            CategoryId = 3,
            Created = DateTime.Parse("2025-08-10 10:00:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-08-10 10:05:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("11111111-1111-4111-8111-aaaaaaaaaaa2"),
            UserId = aliceId,
            Title = "Revisar orçamento mensal",
            Description = "Atualizar planilha e conferir gastos do cartão.",
            IsCompleted = true,
            CategoryId = 4,
            Created = DateTime.Parse("2025-08-01 18:20:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-08-02 07:10:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("11111111-1111-4111-8111-aaaaaaaaaaa3"),
            UserId = aliceId,
            Title = "Preparar apresentação sprint",
            Description = "Slide deck com métricas e próximos passos.",
            IsCompleted = true,
            CategoryId = 2,
            Created = DateTime.Parse("2025-09-05 09:15:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-09-06 11:30:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("11111111-1111-4111-8111-aaaaaaaaaaa4"),
            UserId = aliceId,
            Title = "Comprar presente para Ana",
            Description = "Ideias: livro, chocolate artesanal, vela aromática.",
            IsCompleted = false,
            CategoryId = 1,
            Created = DateTime.Parse("2025-09-28 16:45:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-10-01 12:00:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("11111111-1111-4111-8111-aaaaaaaaaaa5"),
            UserId = aliceId,
            Title = "Backup das notas do celular",
            Description = "Verificar iCloud/Drive e espaço disponível.",
            IsCompleted = true,
            CategoryId = 1,
            Created = DateTime.Parse("2025-07-22 21:00:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-07-22 21:03:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("11111111-1111-4111-8111-aaaaaaaaaaa6"),
            UserId = aliceId,
            Title = "Atualizar status do projeto X",
            Description = "Enviar update no Slack com bloqueios e riscos.",
            IsCompleted = false,
            CategoryId = 2,
            Created = DateTime.Parse("2025-10-12 08:40:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-10-12 08:41:00+00").ToUniversalTime()
        });

        // Bruno
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("22222222-2222-4222-8222-bbbbbbbbbbb1"),
            UserId = brunoId,
            Title = "Planejar churrasco de família",
            Description = "Definir lista de compras e confirmar presença.",
            IsCompleted = false,
            CategoryId = 3,
            Created = DateTime.Parse("2025-08-14 12:10:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-08-14 12:20:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("22222222-2222-4222-8222-bbbbbbbbbbb2"),
            UserId = brunoId,
            Title = "Refatorar módulo de pagamentos",
            Description = "Separar responsabilidades e cobrir com testes.",
            IsCompleted = true,
            CategoryId = 2,
            Created = DateTime.Parse("2025-07-30 10:00:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-08-02 16:30:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("22222222-2222-4222-8222-bbbbbbbbbbb3"),
            UserId = brunoId,
            Title = "Comprar passagens para o feriado",
            Description = "Pesquisar preços e horários noturnos.",
            IsCompleted = true,
            CategoryId = 4,
            Created = DateTime.Parse("2025-09-03 19:25:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-09-03 20:00:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("22222222-2222-4222-8222-bbbbbbbbbbb4"),
            UserId = brunoId,
            Title = "Check-up anual",
            Description = "Exames de sangue e consulta clínica geral.",
            IsCompleted = false,
            CategoryId = 1,
            Created = DateTime.Parse("2025-10-05 07:55:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-10-07 09:00:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("22222222-2222-4222-8222-bbbbbbbbbbb5"),
            UserId = brunoId,
            Title = "Criar cronograma do app side-project",
            Description = "Roadmap MVP e milestones quinzenais.",
            IsCompleted = false,
            CategoryId = 2,
            Created = DateTime.Parse("2025-08-22 13:40:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-08-22 13:45:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("22222222-2222-4222-8222-bbbbbbbbbbb6"),
            UserId = brunoId,
            Title = "Pagar IPVA",
            Description = "Verificar desconto e emitir guia.",
            IsCompleted = true,
            CategoryId = 1,
            Created = DateTime.Parse("2025-06-10 09:00:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-06-10 09:10:00+00").ToUniversalTime()
        });

        // Carla
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("33333333-3333-4333-8333-ccccccccccc1"),
            UserId = carlaId,
            Title = "Desenhar protocolo do estudo",
            Description = "Hipóteses, indicadores e plano de análise.",
            IsCompleted = true,
            CategoryId = 2,
            Created = DateTime.Parse("2025-05-16 11:30:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-05-18 08:00:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("33333333-3333-4333-8333-ccccccccccc2"),
            UserId = carlaId,
            Title = "Reservar pousada para congresso",
            Description = "Perto do centro de convenções, com café da manhã.",
            IsCompleted = true,
            CategoryId = 3,
            Created = DateTime.Parse("2025-07-02 15:20:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-07-02 15:50:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("33333333-3333-4333-8333-ccccccccccc3"),
            UserId = carlaId,
            Title = "Consulta com nutricionista",
            Description = "Montar cardápio semanal equilibrado.",
            IsCompleted = false,
            CategoryId = 4,
            Created = DateTime.Parse("2025-09-21 09:10:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-09-21 09:12:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("33333333-3333-4333-8333-ccccccccccc4"),
            UserId = carlaId,
            Title = "Organizar fotos da viagem",
            Description = "Selecionar e criar álbum compartilhado.",
            IsCompleted = false,
            CategoryId = 1,
            Created = DateTime.Parse("2025-10-03 20:05:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-10-04 10:00:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("33333333-3333-4333-8333-ccccccccccc5"),
            UserId = carlaId,
            Title = "Revisar bibliografia do paper",
            Description = "Checar referências e DOI.",
            IsCompleted = true,
            CategoryId = 2,
            Created = DateTime.Parse("2025-08-11 08:45:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-08-12 18:00:00+00").ToUniversalTime()
        });
        tasks.Add(new Models.Task
        {
            Id = Guid.Parse("33333333-3333-4333-8333-ccccccccccc6"),
            UserId = carlaId,
            Title = "Comprar seguro de viagem",
            Description = "Cobertura médica internacional e extravio.",
            IsCompleted = false,
            CategoryId = 3,
            Created = DateTime.Parse("2025-09-30 17:30:00+00").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-10-01 09:00:00+00").ToUniversalTime()
        });

        context.Tasks.AddRange(tasks);
        context.SaveChanges();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TaskManagement.Application.Handlers;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskManagement API",
        Version = "v1",
        Description = "API de gerenciamento de tarefas e projetos"
    });
});

// Connection string (appsettings.json ou variável de ambiente)
var conn = builder.Configuration.GetConnectionString("MySql")
           ?? Environment.GetEnvironmentVariable("ConnectionStrings__MySql");

if (string.IsNullOrWhiteSpace(conn))
    throw new InvalidOperationException(
        "Connection string 'MySql' não encontrada. Defina em appsettings.json ou na variável de ambiente ConnectionStrings__MySql.");

// Use AutoDetect quando possível; caso contrário ajuste ServerVersion.Parse(...)
//var serverVersion = ServerVersion.AutoDetect(conn);
var serverVersion = ServerVersion.Parse("8.0.32");

builder.Services.AddDbContext<TaskDbContext>(opt =>
{
    opt.UseMySql(conn, serverVersion, mySqlOptions =>
    {
        mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 15,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
        mySqlOptions.CommandTimeout(60);
    });
});

// Repositórios
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskHistoryRepository, TaskHistoryRepository>();
builder.Services.AddScoped<ITaskCommentRepository, TaskCommentRepository>();

// Registra handlers automaticamente
var appAssembly = typeof(CreateProjectCommandHandler).Assembly;
var handlerTypes = appAssembly.GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic && t.Name.EndsWith("Handler"));
foreach (var type in handlerTypes)
    builder.Services.AddScoped(type);

var app = builder.Build();

// Teste de conexão ao iniciar (retry para diagnóstico)
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    Exception lastEx = null;
    const int maxAttempts = 10;
    const int delaySeconds = 3;
    bool connected = false;

    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            logger.LogInformation("Tentativa {Attempt}/{Max} de conectar ao banco MySQL...", attempt, maxAttempts);
            var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
            if (db.Database.CanConnect())
            {
                logger.LogInformation("Conexão com o banco estabelecida.");
                connected = true;
                break;
            }

            logger.LogWarning("db.Database.CanConnect() retornou false na tentativa {Attempt}.", attempt);
        }
        catch (Exception ex)
        {
            lastEx = ex;
            logger.LogWarning(ex, "Erro na tentativa {Attempt} de conectar ao banco.", attempt);
        }

        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
    }

    if (!connected)
    {
        if (lastEx != null) logger.LogCritical(lastEx, "Não foi possível conectar ao banco após {Max} tentativas.", maxAttempts);
        throw new InvalidOperationException("Não foi possível conectar ao banco de dados MySQL. Verifique a connection string e a disponibilidade do servidor.");
    }
}

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManagement API v1");
    c.RoutePrefix = "swagger";
});

//app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapGet("/_ping", () => Results.Ok(new { status = "pong", at = DateTimeOffset.Now })).WithName("Ping");
app.Run();

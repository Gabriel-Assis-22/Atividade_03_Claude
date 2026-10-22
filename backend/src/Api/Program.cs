using System.Text;
using Application.UseCases.Comments;
using Application.UseCases.Favorites;
using Application.UseCases.Payments;
using Application.UseCases.Profile;
using Domain.Repositories;
using Domain.Services;
using FluentMigrator.Runner;
using Infrastructure.ExternalServices;
using Infrastructure.Migrations;
using Infrastructure.Payments;
using Infrastructure.Persistence;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── Variáveis de ambiente ──────────────────────────────────────────────────────
var tmdbApiKey  = Environment.GetEnvironmentVariable("TMDB_API_KEY")  ?? throw new Exception("TMDB_API_KEY não configurada");
var dbHost      = Environment.GetEnvironmentVariable("DB_HOST")       ?? "localhost";
var dbPort      = Environment.GetEnvironmentVariable("DB_PORT")       ?? "3306";
var dbUser      = Environment.GetEnvironmentVariable("DB_USER")       ?? throw new Exception("DB_USER não configurada");
var dbPassword  = Environment.GetEnvironmentVariable("DB_PASSWORD")   ?? throw new Exception("DB_PASSWORD não configurada");
var dbName      = Environment.GetEnvironmentVariable("DB_NAME")       ?? throw new Exception("DB_NAME não configurada");
var jwtSecret   = Environment.GetEnvironmentVariable("JWT_SECRET")    ?? throw new Exception("JWT_SECRET não configurada");

var connString = $"Server={dbHost};Port={dbPort};Database={dbName};Uid={dbUser};Pwd={dbPassword};";

// ── Infrastructure ─────────────────────────────────────────────────────────────
builder.Services.AddSingleton(new DbConnectionFactory(connString));
builder.Services.AddScoped<IFavoritoRepository, FavoritoRepository>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();
builder.Services.AddScoped<ICaronaRepository, CaronaRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IPaymentService, StripePaymentService>();

// ── Garage / S3 Object Storage ────────────────────────────────────────────────
builder.Services.AddHttpClient<IStorageService, S3StorageService>();

// ── TMDB HttpClient ────────────────────────────────────────────────────────────
builder.Services.AddHttpClient<TmdbService>(client =>
{
    client.BaseAddress = new Uri("https://api.themoviedb.org");
    client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tmdbApiKey);
});

// ── Auth Service HttpClient (Comunicação Interna Docker) ──────────────────────
var authServiceUrl = Environment.GetEnvironmentVariable("AUTH_SERVICE_URL") ?? "http://auth-service:8081";
builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri(authServiceUrl);
});

// ── Log Service HttpClient (Comunicação Interna Docker) ───────────────────────
var logServiceUrl = Environment.GetEnvironmentVariable("LOG_SERVICE_URL") ?? "http://log-service:8082";
builder.Services.AddHttpClient("LogService", client =>
{
    client.BaseAddress = new Uri(logServiceUrl);
});
builder.Services.AddHttpClient<IAuditClient, AuditClient>(client =>
{
    client.BaseAddress = new Uri(logServiceUrl);
});

// ── Use Cases ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<GetFavoritesUseCase>();
builder.Services.AddScoped<AddFavoriteUseCase>();
builder.Services.AddScoped<RemoveFavoriteUseCase>();
builder.Services.AddScoped<CheckFavoriteUseCase>();
builder.Services.AddScoped<GetCommentsUseCase>();
builder.Services.AddScoped<AddCommentUseCase>();
builder.Services.AddScoped<DeleteCommentUseCase>();
builder.Services.AddScoped<GetProfileUseCase>();
builder.Services.AddScoped<UpdateProfileUseCase>();
builder.Services.AddScoped<UploadProfilePhotoUseCase>();
builder.Services.AddScoped<CreateCheckoutSessionUseCase>();
builder.Services.AddScoped<CreateCustomerPortalUseCase>();
builder.Services.AddScoped<HandleStripeWebhookUseCase>();
builder.Services.AddScoped<Application.UseCases.Veiculos.VeiculoUseCases>();
builder.Services.AddScoped<Application.UseCases.Caronas.CaronaUseCases>();
builder.Services.AddScoped<Application.UseCases.Reservas.ReservaUseCases>();


// ── JWT Auth ───────────────────────────────────────────────────────────────────
var keyBytes = Encoding.UTF8.GetBytes(jwtSecret);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
        };
    });
builder.Services.AddAuthorization();

// ── CORS ───────────────────────────────────────────────────────────────────────
builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(p => p
        .WithOrigins("http://localhost:4200", "https://gabriel-assis-isw055.lapps.studio")
        .AllowAnyHeader()
        .AllowAnyMethod()));

// ── FluentMigrator ─────────────────────────────────────────────────────────────
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(r => r
        .AddMySql5()
        .WithGlobalConnectionString(connString)
        .ScanIn(typeof(_001_CreateUsuarios).Assembly).For.All())
    .AddLogging(lb => lb.AddConsole());

// ── Controllers + Swagger / OpenAPI ──────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Catálogo Tom Hanks API",
        Version = "v1",
        Description = "API REST de Microsserviços para o Catálogo Tom Hanks. Inclui Catálogo TMDB, Favoritos (com enforcement de limite no plano gratuito e ilimitado no Premium), Comentários, Perfil de Usuário, Upload de Avatar no Garage S3, Pagamentos Stripe e Trilha de Auditoria.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Gabriel Assis",
            Url = new Uri("https://github.com/Gabriel-Assis-22/Atividade_02_Claude")
        }
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Autenticação JWT via cabeçalho Authorization. Exemplo: 'Bearer {seu_token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ── Executar Migrations e Inicializar Bucket MinIO no Startup ─────────────────
using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    // MigrateUp() é idempotente — só aplica migrations ainda não registradas na tabela VersionInfo
    runner.MigrateUp();

    var storage = scope.ServiceProvider.GetRequiredService<IStorageService>();
    await storage.EnsureBucketCreatedAsync();
}

// ── Pipeline ───────────────────────────────────────────────────────────────────
app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catálogo Tom Hanks API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

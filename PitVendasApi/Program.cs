using System.Text;
using System.Text.Json.Serialization;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PitVendasApi.DataContexts;
using PitVendasApi.Identity;
using PitVendasApi.Services;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperProfile>());
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    x.JsonSerializerOptions.WriteIndented = true;
});

// ---------------------------------------------------------------------
// ---------------------------------------------------------------------
const string PoliticaCors = "PitVendasFront";
var origensPermitidas = builder.Configuration
    .GetSection("Cors:Origens").Get<string[]>() ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy(PoliticaCors, policy => policy
        .WithOrigins(origensPermitidas)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// ---------------------------------------------------------------------
// ---------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Pit Vendas API",
        Version = "v1",
        Description = "API do sistema de controle e emissao de vendas para representantes comerciais."
    });

    var esquema = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe apenas o token JWT (sem o prefixo 'Bearer').",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, esquema);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { esquema, Array.Empty<string>() }
    });
});

// ---------------------------------------------------------------------
// ---------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'default' nao configurada. Em desenvolvimento, rode: " +
        "dotnet user-secrets set \"ConnectionStrings:default\" " +
        "\"Server=localhost;Database=pitvendas_bd;User=root;Password=SUA_SENHA;Port=3306;CharSet=utf8mb4;\"");
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options
    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    .UseSnakeCaseNamingConvention()
);

// ---------------------------------------------------------------------
// ---------------------------------------------------------------------
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IPedidoPdfService, PedidoPdfService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(EmailSettings.SectionName));
builder.Services.AddScoped<IEmailService, EmailService>();

var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName).Get<JwtSettings>();

if (string.IsNullOrWhiteSpace(jwtSettings?.Key))
{
    throw new InvalidOperationException(
        "Chave JWT nao configurada. Em desenvolvimento, rode: " +
        "dotnet user-secrets set \"Jwt:Key\" \"<uma chave longa e aleatoria>\"");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddAuthorization();

builder.Services.AddExceptionHandler<TratadorDeErros>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(PoliticaCors);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { }

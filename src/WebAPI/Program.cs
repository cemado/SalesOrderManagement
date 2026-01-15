using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text;
using MediatR;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Domain.Repositories;
using Application.Mapping;
using Application.CQRS.Commands;
using Application.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// ===== SWAGGER CON AUTORIZACIÓN JWT =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Información de la API
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sales Order Management API",
        Version = "v1",
        Description = "API REST para gestión de órdenes de venta con autenticación JWT",
        Contact = new OpenApiContact
        {
            Name = "Cesar Mamani Domínguez",
            Email = "cedominguez@gmail.com",
            Url = new Uri("https://github.com/cemado/SalesOrderManagement")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // ===== AGREGAR DEFINICIÓN DE SEGURIDAD BEARER JWT =====
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = @"
Ingresa tu token JWT aquí.

**Pasos:**
1. POST a `/api/auth/login` con:
   - Username: admin@test.com
   - Password: admin123
2. Copia el token de la respuesta
3. Pega aquí: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

**Nota:** No incluyas la palabra 'Bearer', solo el token.",
        Name = "Authorization",
        In = ParameterLocation.Header
    });

    // ===== REQUERIR JWT EN TODOS LOS ENDPOINTS PROTEGIDOS =====
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "bearer",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    // ===== INCLUIR COMENTARIOS XML PARA DOCUMENTACIÓN =====
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// ===== SERVICIOS BASE =====
builder.Services.AddControllers();

// ===== ENTITY FRAMEWORK CORE =====
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== INYECCIÓN DE DEPENDENCIAS =====
builder.Services.AddScoped<IOrdenRepository, OrdenRepository>();

// ===== MEDIATR (CQRS) =====
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CrearOrdenCommand).Assembly);
});

// ===== AUTOMAPPER =====
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ===== AUTENTICACIÓN JWT =====
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? "tu-clave-secreta-super-mega-larga-para-produccion-minimo-32-caracteres";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "SalesOrderAPI",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "SalesOrderAPIUsers",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ===== AUTORIZACIÓN =====
builder.Services.AddAuthorization();

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ===== FLUENT VALIDATION =====
builder.Services.AddValidatorsFromAssemblyContaining<CrearOrdenValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ActualizarOrdenValidator>();

// ===== BUILD APP =====
var app = builder.Build();

// ===== PIPELINE =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sales Order Management API v1");
        options.RoutePrefix = string.Empty; // Swagger en la raíz: http://localhost:5001/
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelExpandDepth(2);
        
        // ===== AGREGAR ESTILOS PERSONALIZADOS =====
        options.InjectStylesheet("/swagger-ui.css");
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ===== CREAR/ACTUALIZAR BD AUTOMÁTICAMENTE =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();

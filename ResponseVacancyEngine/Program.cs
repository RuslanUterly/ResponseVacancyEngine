using System.Text;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ResponseVacancyEngine.Application.Interfaces;
using ResponseVacancyEngine.Application.Interfaces.CryptoHelper;
using ResponseVacancyEngine.Application.Interfaces.JwtProvider;
using ResponseVacancyEngine.Application.Interfaces.Services.HeadHunterApi;
using ResponseVacancyEngine.Application.Services;
using ResponseVacancyEngine.Infrastructure.Helpers;
using ResponseVacancyEngine.Infrastructure.JwtProvider;
using ResponseVacancyEngine.Infrastructure.Options;
using ResponseVacancyEngine.Infrastructure.Persistence;
using ResponseVacancyEngine.Infrastructure.Persistence.DataAccess;
using ResponseVacancyEngine.Infrastructure.Services.HeadHunterAPI;
using ResponseVacancyEngine.Persistence.Interfaces;
using ResponseVacancyEngine.Persistence.Models;
using ResponseVacancyEngine.Persistence.Models.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddHttpClient();

//mapster
builder.Services.AddMapster();

//crypto
builder.Services.AddDataProtection();

//options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.Configure<CryptoOptions>(builder.Configuration.GetSection("CryptoOptions"));
builder.Services.Configure<HeadHunterUriOptions>(builder.Configuration.GetSection("HHUriOptions"));

//service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ICryptoHelper, CryptoHelper>();
builder.Services.AddScoped<IHeadHunterOAuthClient, HeadHunterOAuthClient>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IExcludedWordService, ExcludedWordService>();

//repository
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IExcludedWordRepository, ExcludedWordRepository>();

//db
builder.Services.AddDbContextPool<VacancyContext>(options =>
{
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), opt =>
        {
            opt.CommandTimeout(60);
        })
        .EnableSensitiveDataLogging();
});

//auth
builder.Services.AddIdentity<Account, IdentityRole<long>>()
    .AddEntityFrameworkStores<VacancyContext>()
    .AddDefaultTokenProviders();

var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:SecretKey"]);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Введите JWT токен так: Bearer {токен}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseRouting();

app.UseCors(options => options
    .WithOrigins(new[] {"http://localhost:5173"})
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();
    string[] roles = [Roles.User, Roles.Admin];

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<long>(role));
        }
    }
}

app.Run();

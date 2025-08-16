
using Core.Interfaces;
using Core.Settings;
using Infrastructure.UnitOfWork;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Core.Models;
using Microsoft.AspNetCore.Identity;

using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

using System.Drawing;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using ProjectApi.Services;
using Core.Services;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ProjectApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddDbContext<AppDBContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("ProductionConnection"),
                new MySqlServerVersion(new Version(8, 0, 36))));
            var authorizationBuilder = builder.Services.AddAuthorizationBuilder();
            authorizationBuilder.AddPolicy("AdminRole", p => p.RequireRole("admin"));
            authorizationBuilder.AddPolicy("EditorRole", p => p.RequireRole("admin", "editor"));
            authorizationBuilder.AddPolicy("AuthorRole", p => p.RequireRole("admin","author"));


            builder.WebHost.UseUrls("http://+:8080 ");  
            builder.Configuration.AddEnvironmentVariables();
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
            builder.Services.Configure<EmailConfgSettings>(builder.Configuration.GetSection("EMAILCONFG"));



            builder.Services.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            builder.Services.AddTransient<StoreImage>();
            builder.Services.AddTransient<SendEmailServices>();
            builder.Services.AddTransient<GeneratePassword>();
            builder.Services.AddTransient<MessageRole>();
            builder.Services.AddTransient<GenerateSlugService>();
            builder.Services.AddTransient<TagsServices>();




            builder.Services.AddIdentity<AppUser, IdentityRole>().
                AddEntityFrameworkStores<AppDBContext>();

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });




            builder.Services.AddScoped<PasswordHasher<AppUser>>();


            builder.Services.AddTransient<IAuthentication, Authentication>();
            builder.Services.AddTransient<AppDBContext>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:Key"]))

                };

            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors();
            builder.Services.AddSwaggerGen(options =>
            {



                options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,

                });



                options.AddSecurityRequirement(securityRequirement: new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()

                    }

                });

            });

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "/app/ProjectApi/uploads")),
                RequestPath = "/uploads"
            });

            app.UseHttpsRedirection();
            app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}

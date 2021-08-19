using System;
using System.IO.Abstractions;
using System.Reflection;

using FluentValidation.AspNetCore;

using MediatR;

using MicroElements.Swashbuckle.FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using ToDoList.Core;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.Core.Validators;
using ToDoList.Extensions;
using ToDoList.Infrastructure.Data;
using ToDoList.Infrastructure.Extensions;
using ToDoList.WebApi.Jwt;
using ToDoList.WebApi.Jwt.Models;
using ToDoList.WebApi.Services;
using ToDoList.WebApi.Validators;

namespace ToDoList.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInfrastructure(Configuration.GetConnectionString("DefaultConnection"));

            services.AddSingleton(Configuration.GetSection(ApiOptions.Apis).GetValid<ApiOptions>());

            services.AddScoped<IFileSystem, FileSystem>();

            services.AddTransient<IGeocodingService, GoogleGeocodingService>();
            services.AddTransient<ICreateWithAddressService, CreateWithAddressService>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddAutoMapper(typeof(CategoryResponse));
            services.AddMediatR(typeof(GetAllQuery<,>));

            var authenticationConfig = Configuration.GetSection("AuthenticationConfigs").GetValid<AuthenticationConfig>();

            services.AddSingleton(authenticationConfig);

            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            services.AddScoped<ITokenValidator, JwtTokenValidator>();

            services.AddScoped<IAuthenticator, Authenticator>();

            services.AddScoped<IFileStorage, PhysicalFileStorage>();
            services.AddScoped<IImageMinifier, MagickImageMinifier>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("https://localhost:44358", "https://localhost:44306")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            services.AddAuthentication(options =>
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
                        ValidateIssuer = true,
                        ValidIssuer = authenticationConfig.Issuer,

                        ValidateAudience = true,
                        ValidAudience = authenticationConfig.Audience,

                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey = authenticationConfig.SymmetricSecurityAccessKey,
                        ClockSkew = TimeSpan.Zero
                    };
                });


            // Uncomment when using Redis
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = Configuration.GetConnectionString("Redis");
            //    options.InstanceName = "TodoListApp_";
            //});

            // Uncomment when not using Redis
            services.AddDistributedMemoryCache();

            services.AddControllers()
                    .AddFluentValidation(config =>
                    {
                        var assemblies = new Assembly[]
                        {
                            typeof(GeoCoordinateValidator).Assembly,
                            typeof(FormFileValidator).Assembly
                        };

                        config.RegisterValidatorsFromAssemblies(assemblies);
                    });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoList.WebApi", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme"
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

            services.AddFluentValidationRulesToSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<TodoListContext>();
                context.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDoList.WebApi v1"));
            }

            if (env.IsProduction())
                app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;

using MediatR;

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
using ToDoList.Diagnostic;
using ToDoList.Extensions;
using ToDoList.Infrastructure.Data;
using ToDoList.Infrastructure.Extensions;
using ToDoList.WebApi.Jwt;
using ToDoList.WebApi.Jwt.Models;
using ToDoList.WebApi.Services;

namespace ToDoList.WebApi
{
    [ExcludeFromCodeCoverage]
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

            services.AddTransient<IGeocodingService, GoogleGeocodingService>();
            services.AddTransient<ICreateWithAddressService, CreateWithAddressService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddAutoMapper(typeof(CategoryResponse));
            services.AddMediatR(typeof(GetAllQuery<,>));

            var jwtTokenConfig = Configuration.GetSection("AuthenticationConfigs").GetValid<AuthenticationConfig>();

            services.AddSingleton(jwtTokenConfig);

            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<ITokenValidator, TokenValidator>();
            services.AddScoped<IAuthenticator, Authenticator>();

            services.AddCors();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtTokenConfig.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtTokenConfig.Audience,

                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = jwtTokenConfig.SymmetricSecurityAccessKey,
                ClockSkew = TimeSpan.Zero
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = tokenValidationParameters;
                });

            services.AddSingleton<ProccessTimeCounterSource>();

            // Uncomment when using Redis
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = Configuration.GetConnectionString("Redis");
            //    options.InstanceName = "TodoListApp_";
            //});

            // Uncomment when not using Redis
            services.AddDistributedMemoryCache();

            services.AddControllers(options => options.Filters.Add<ProccesTimeActionFilterAttribute>());

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(x => x.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

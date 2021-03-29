
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using ToDoList.Core;
using ToDoList.Core.Mediator.Queries.Generics;
using ToDoList.Core.Mediator.Response;
using ToDoList.Core.Services;
using ToDoList.Infrastructure.Extensions;
using ToDoList.WebApi.Jwt;

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

            services.AddSingleton(Configuration.GetSection(ApiOptions.Apis).Get<ApiOptions>());

            services.AddTransient<IGeocodingService, GoogleGeocodingService>();
            services.AddTransient<ICreateWithAddressService, CreateWithAddressService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddAutoMapper(typeof(CategoryResponse));
            services.AddMediatR(typeof(GetAllQuery<,>));

            var jwtTokenConfig = Configuration.GetSection("JwtTokenConfigs").Get<JwtTokenConfig>();

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(jwtTokenConfig, serviceProvider: null, items: null);

            if (!Validator.TryValidateObject(jwtTokenConfig, validationContext, validationResults, validateAllProperties: true))
            {
                foreach (var item in validationResults)
                {
                    throw new ValidationException(item.ErrorMessage);
                }
            }

            services.AddSingleton(jwtTokenConfig);
            services.AddScoped<ITokenGenerator, TokenGenerator>();

            services.AddCors();

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
                        ValidIssuer = jwtTokenConfig.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtTokenConfig.Audience,

                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey = jwtTokenConfig.GetSymmetricSecurityKey()
                    };
                });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoList.WebApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDoList.WebApi v1"));
            }

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

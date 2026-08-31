using DomainLayer.Contracts___Repo_Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using perisistence;
using perisistence.Data;
using perisistence.Repositories;
using Shared.ErrorModels;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text.Json.Serialization;
using ToDoApp.Extentinos;
using ToDoApp.Factories;

namespace ToDoApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container

            //builder.Services.AddControllers();
            builder.Services.AddSwaggerServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddWebApplicationServices();
            // Connecting the frontend part with the backend part of the application using CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            #endregion

            var app = builder.Build();

            #region Data Seeding
            await app.SeedDataBaseAsync();
            #endregion

            #region Configure the HTTP request pipeline

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.ConfigObject = new Swashbuckle.AspNetCore.SwaggerUI.ConfigObject()
                    {
                        DisplayRequestDuration = true,
                    };
                    options.DocumentTitle = "ToDo API Project";
                    options.DocExpansion(DocExpansion.None);
                    options.EnableFilter();
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Enable CORS policy
            app.UseCors("AllowAll");
            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}

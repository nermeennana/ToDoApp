using DomainLayer.Contracts___Repo_Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using perisistence;
using perisistence.Data;
using perisistence.Repositories;
using Presentation.Hubs;
using Shared.ErrorModels;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text.Json.Serialization;
using ToDoApp.Extentinos;
using ToDoApp.Factories;
using System.Threading.RateLimiting;
using ToDoApp.Background_Services;
using ToDoApp.CustomMiddlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddSignalR();
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("ToDoPolicy", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromSeconds(10),
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    });
                });
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
            builder.Services.AddMemoryCache();
            builder.Services.AddHostedService<BackgroundReminderService>();
            
            #endregion

            var app = builder.Build();

            app.UseMiddleware<CustomExceptionHandlerMiddleWare>();

            // Enable CORS policy
            app.UseCors("AllowAll");

            app.UseRateLimiter();

            app.MapHub<NotificationHub>("/notificationHub");

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
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}

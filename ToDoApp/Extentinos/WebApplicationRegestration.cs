using DomainLayer.Contracts___Repo_Interface;

namespace ToDoApp.Extentinos
{
    public static class WebApplicationRegestration
    {
        public async static Task SeedDataBaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var seedObj = scope.ServiceProvider.GetRequiredService<IDataSeeding>();
            await seedObj.DataSeedAsync();
        }
    }
}

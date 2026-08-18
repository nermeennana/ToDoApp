using DomainLayer.Contracts___Repo_Interface;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using perisistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace perisistence
{
    public class DataSeeding : IDataSeeding
    {
        public readonly ToDoDbContext _toDoDbContext;
        public DataSeeding(ToDoDbContext toDoDbContext)
        {
            _toDoDbContext = toDoDbContext;
        }
        public async Task DataSeedAsync()
        {
            try
            {
                if ((await _toDoDbContext.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _toDoDbContext.Database.MigrateAsync();
                }

                if (!(await _toDoDbContext.ToDos.AnyAsync()))
                {
                    var todos = File.OpenRead(@"..\Infrastructure\perisistence\DataSeed\todos.json");
                    var options = new JsonSerializerOptions
                    {
                        Converters = { new JsonStringEnumConverter() }
                    };
                    var todosList = await JsonSerializer.DeserializeAsync<List<ToDo>>(todos, options);

                    if (todosList != null && todosList.Any())
                    {
                        await _toDoDbContext.ToDos.AddRangeAsync(todosList);
                    }
                }

                await _toDoDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Data seeding failed: {ex.Message}");
                throw;
            }
        }
    }
}

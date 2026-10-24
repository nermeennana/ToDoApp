using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace perisistence.Data
{
    public class ToDoDbContext : IdentityDbContext<AppUser>
    {
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options)
        {
        }

        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
    }
}

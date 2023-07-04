using Domain_Data.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ProjectDetailsDbContext : DbContext
    {
        public ProjectDetailsDbContext(DbContextOptions<ProjectDetailsDbContext> options) 
            : base(options)
        {

        }

        public DbSet<Projects> Projects { get; set; }
    }
}

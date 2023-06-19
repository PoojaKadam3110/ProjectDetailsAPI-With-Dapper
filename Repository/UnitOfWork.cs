using Domain_Data.Models.Domain;
using Domain_Data.Models.DTO;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbConnection dbConnection1;
        public UnitOfWork(IProjectsRepo projectsRepo, IDbConnection _dbConnection1)
        {
            Projects = projectsRepo;
            dbConnection1 = _dbConnection1;
        }

        public IProjectsRepo Projects { get; }
        public void Dispose()
        {
           dbConnection1.Dispose();
        }
    }
}

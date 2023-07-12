using Domain_Data.Models.Domain;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using static Dapper.SqlMapper;
using System.Xml.Linq;
using Data;
using Microsoft.EntityFrameworkCore;
using Domain_Data.Models.DTO;
using System.Data;

namespace Application.Repository
{
    public class ProjectsRepo : GenericRepository<Projects>, IProjectsRepo 
    {
        private readonly IDbConnection _connection;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectsRepo(IDbConnection dbConnection) : base(dbConnection)
        {

        }
    }
}

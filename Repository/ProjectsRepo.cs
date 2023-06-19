using Domain_Data.Models.Domain;
using Interfaces;
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

namespace Repository
{
    public class ProjectsRepo : GenericRepository<Projects>, IProjectsRepo 
    {
        private readonly IDbConnection _connection;
        public ProjectsRepo(IDbConnection dbConnection): base(dbConnection)
        {
            
        }
        //private readonly IConfiguration _configuration;
        //private readonly ProjectDetailsDbContext _dbcontext;
        //public ProjectsRepo(IConfiguration configuration , ProjectDetailsDbContext projectDetailsDbContext)
        //{
        //    _configuration = configuration;
        //    _dbcontext = projectDetailsDbContext;
        //}
        //public async Task<int> AddAsync(Projects entity)
        //{
        //    try
        //    {                
        //        //entity.CreatedDate = DateTime.Now;
        //        //entity.UpdatedDate = DateTime.Now;
        //        var sql = "Insert into Projects (ProjectName,ClientName,projectCost,projectManager,ratePerHour,projectUsers,description,isActive,isDeleted,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) VALUES (@ProjectName,@ClientName,@projectCost,@projectManager,@ratePerHour,@projectUsers,@description,@isActive,@isDeleted,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)";
        //        var parameters = new
        //        {
        //            ProjectName = entity.ProjectName,
        //            ClientName = entity.ClientName,
        //            projectCost = entity.projectCost,
        //            projectManager = entity.projectManager,
        //            ratePerHour = entity.ratePerHour,
        //            projectUsers = entity.projectUsers, // Manually filled value
        //            description = entity.description,
        //            isActive = true, // Manually filled value
        //            isDeleted = false, // Manually filled value
        //            CreatedBy = "Pooja",
        //            UpdatedBy = "Pooja", // Manually filled value
        //            CreatedDate = DateTime.Now,
        //            UpdatedDate = DateTime.Now
        //        };
        //        using (var connection = new SqlConnection(_configuration.GetConnectionString("ProjectDetailsConnectionStrings")))
        //        {
        //            connection.Open();
        //            var result = await connection.ExecuteAsync(sql, parameters);
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Add exception", ex);
        //    }
        //}

        //public async Task<int> DeleteAsync(int id)
        //{
        //    try
        //    {
        //        var demo = "SELECT * FROM Projects WHERE Id = @Id";
        //        using (var connection = new SqlConnection(_configuration.GetConnectionString("ProjectDetailsConnectionStrings")))
        //        {
        //            connection.Open();
        //            var result = await connection.QuerySingleOrDefaultAsync<Projects>(demo, new { Id = id });
        //            if (result == null || result.isDeleted == true)
        //                return 0;
        //        }

        //        var sql = "UPDATE Projects SET isActive = 'false',isDeleted = 'true'  WHERE Id = @Id";
        //        using (var connection = new SqlConnection(_configuration.GetConnectionString("ProjectDetailsConnectionStrings")))
        //        {
        //            connection.Open();
        //            var result = await connection.ExecuteAsync(sql, new { Id = id });
        //            return result;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        throw new Exception("Delete exception", ex);
        //    }
        //}

        //public async Task<IReadOnlyList<Projects>> GetAllAsync()
        //{
        //    var sql = "SELECT * FROM Projects";
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("ProjectDetailsConnectionStrings")))
        //    {
        //        connection.Open();
        //        var result = await connection.QueryAsync<Projects>(sql);
        //        return result.ToList();
        //    }
        //}
        //public async Task<List<Projects>> GetAll(string? filterOn = null, string? filterQuery = null,
        //  string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)  //modified thid method for filtering and middle two for sorting and last two for pagginatiom
        //{
        //    try
        //    {
        //        var data = _dbcontext.Projects.AsQueryable();
        //        //Filterring
        //        if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
        //        {
        //            if (filterOn.Equals("Project Name", StringComparison.OrdinalIgnoreCase))
        //            {
        //                data = data.Where(x => x.ProjectName.Contains(filterQuery));
        //            }
        //        }

        //        //sorting
        //        if (string.IsNullOrWhiteSpace(sortBy) == false)
        //        {
        //            if (sortBy.Equals("Date", StringComparison.OrdinalIgnoreCase))
        //            {
        //                data = isAscending ? data.OrderBy(x => x.CreatedDate) : data.OrderByDescending(x => x.CreatedDate); //ternary operator
        //            }
        //            else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
        //            {
        //                data = isAscending ? data.OrderBy(x => x.CreatedDate) : data.OrderByDescending(x => x.CreatedDate);
        //            }
        //        }

        //        //Paggination
        //        var skipResults = (pageNumber - 1) * pageSize;
        //        //_logger.LogInformation("Executing GetAllProjects");
        //        return await data.Skip(skipResults).Take(pageSize).ToListAsync();     // for paggination
        //                                                                              //return await walks.ToListAsync();    

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("You are not able to see the project list", ex);
        //    }
        //}



        //public async Task<Projects> GetByIdAsync(int id)
        //{
        //   try
        //    {
        //        var sql = "SELECT * FROM Projects WHERE Id = @Id";
        //        using (var connection = new SqlConnection(_configuration.GetConnectionString("ProjectDetailsConnectionStrings")))
        //        {
        //            connection.Open();
        //            var result = await connection.QuerySingleOrDefaultAsync<Projects>(sql, new { Id = id });
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("id not found", ex);
        //    }
        //}

        //public async Task<int> UpdateAsync(Projects entity)
        //{

        //    try
        //    {
        //        var demo = "SELECT * FROM Projects WHERE Id = @Id";
        //        using (var connection = new SqlConnection(_configuration.GetConnectionString("ProjectDetailsConnectionStrings")))
        //        {
        //            connection.Open();
        //            var result = await connection.QuerySingleOrDefaultAsync<Projects>(demo, new { Id = entity.Id });
        //            if (result == null || result.isDeleted == true)
        //                return 0;
        //        }

        //        entity.UpdatedDate = DateTime.Now;
        //        var sql = "UPDATE Projects SET ProjectName = @ProjectName,ClientName = @ClientName,projectCost = @projectCost,projectManager = @projectManager,ratePerHour = @ratePerHour,projectUsers = @projectUsers,description = @description,isActive = @isActive,isDeleted = @isDeleted,CreatedDate = @CreatedDate,CreatedBy= @CreatedBy,UpdatedDate = @UpdatedDate,UpdatedBy = @UpdatedBy  WHERE Id = @Id";
        //        using (var connection = new SqlConnection(_configuration.GetConnectionString("ProjectDetailsConnectionStrings")))
        //        {
        //            connection.Open();
        //            var result = await connection.ExecuteAsync(sql, entity);
        //            return result;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        throw new Exception("Update exception", ex);                 
        //    }
        //}
    }
}

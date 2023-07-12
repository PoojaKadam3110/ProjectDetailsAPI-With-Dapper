using Application.Interfaces;
using Dapper;
using Data;
using Domain_Data.Models.Domain;
using Domain_Data.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly IConfiguration _configuration;
        private ProjectDetailsDbContext dbConnection;
        private IDbConnection dbConnection1;

        public GenericRepository(IDbConnection dbConnection1)
        {
            this.dbConnection1 = dbConnection1;
        }
        public async Task<int> AddAsync(T entity)
        {
            // Set additional fields manually
            var isActiveProperty = typeof(T).GetProperty("isActive");
            var isDeletedProperty = typeof(T).GetProperty("isDeleted");
            var createdDateProperty = typeof(T).GetProperty("CreatedDate");
            var updatedDateProperty = typeof(T).GetProperty("UpdatedDate");
            var createdByProperty = typeof(T).GetProperty("CreatedBy");
            var updatedByProperty = typeof(T).GetProperty("UpdatedBy");

            isActiveProperty?.SetValue(entity, true);
            isDeletedProperty?.SetValue(entity, false);
            createdDateProperty?.SetValue(entity, DateTime.Now);
            updatedDateProperty?.SetValue(entity, DateTime.Now);
            createdByProperty?.SetValue(entity, "pooja");
            updatedByProperty?.SetValue(entity, "pooja");

            // Insert the entity into the database
            string tableName = typeof(T).Name;
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != "Id" && p.CanWrite && !string.IsNullOrEmpty(p.Name))
                .ToList();

            string columnNames = string.Join(", ", properties.Select(p => p.Name));
            string parameterNames = string.Join(", ", properties.Select(p => "@" + p.Name));
            string query = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames})";
           

            return await dbConnection1.ExecuteAsync(query, entity);

        }

        public async Task<int> DeleteAsync(int id)
        {
            string tableName = typeof(T).Name;
            var sql = "SELECT isDeleted FROM " + tableName + " WHERE Id = @Id";
            var parameters = new { Id = id };

            bool isAlreadyDeleted = await dbConnection1.QueryFirstOrDefaultAsync<bool>(sql, parameters);
            if (isAlreadyDeleted)
            {
                return 0;
            }

            string query = $"UPDATE {tableName} SET isActive = 0, isDeleted = 1 WHERE Id = @Id";

            return await dbConnection1.ExecuteAsync(query, parameters);
        }
        public async Task<IEnumerable<T>> GetAllAsync(string projectName, string orderByColumn, bool isDescending, int pageSize = 1000, int pageNumber = 1)
        {
            string tableName = typeof(T).Name;
            string query = $"SELECT * FROM {tableName} WHERE ProjectName LIKE @ProjectName " +
                           $"ORDER BY {orderByColumn} {(isDescending ? "DESC" : "ASC")} ";

            if (dbConnection1 is SqlConnection) // SQL Server
            {
                query += $"OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            }
            else
            {
                // Handle other database types or throw an exception if unsupported
                throw new NotSupportedException("Database type not supported.");
            }

            var parameters = new
            {
                ProjectName = $"%{projectName}%",
                PageSize = pageSize,
                Offset = (pageNumber - 1) * pageSize
            };

            dbConnection1.Open();
            var result = await dbConnection1.QueryAsync<T>(query, parameters);
            dbConnection1.Close();

            return result;
        }
        public async Task<T> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM " + typeof(T).Name + " WHERE Id = @Id";
            return await dbConnection1.QueryFirstOrDefaultAsync<T>(query, new { Id = id });

        }

        public async Task<int> UpdateAsync(T entity)
        {
            // Set the UpdatedDate 
            var updatedDateProperty = typeof(T).GetProperty("UpdatedDate");
            updatedDateProperty?.SetValue(entity, DateTime.Now);

            // Update the entity in the database
            string tableName = typeof(T).Name;
            var properties = typeof(T).GetProperties()
        .Where(p => p.Name != "Id" && p.CanWrite && !string.IsNullOrEmpty(p.Name) &&
                    p.Name != "CreatedDate" && p.Name != "CreatedBy" &&
                    p.Name != "UpdatedBy" && p.Name != "isActive" && p.Name != "isDeleted")
        .ToList();

            string columnValues = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
            string query = $"UPDATE {tableName} SET {columnValues} WHERE Id = @Id";

            return await dbConnection1.ExecuteAsync(query, entity);
        }


        public int GetRecordCount()
        {

            string tableName = typeof(T).Name;
            string query = $"SELECT COUNT(*) FROM {tableName} WHERE isDeleted = 'false'";
            int recordCount = dbConnection1.QuerySingle<int>(query);

            return recordCount;

        }
        public int GetDeletedRecordCount()
        {
            string tableName = typeof(T).Name;
            string query = $"SELECT COUNT(*) FROM {tableName} WHERE isDeleted = 'true'";
            int recordCount = dbConnection1.QuerySingle<int>(query);

            return recordCount;
        }
    }
}

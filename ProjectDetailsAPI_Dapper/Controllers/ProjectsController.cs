using Application.CustomeActionFilters;
using AutoMapper;
using Azure;
using Domain_Data.Models.Domain;
using Domain_Data.Models.DTO;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Extensions.Configuration;
using Repository;
using System.Data.SqlClient;
using static Dapper.SqlMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectDetailsAPI_Dapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProjectsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IProjectsRepo _projectRepo;

        public ProjectsController(IUnitOfWork unitOfWork, ILogger<ProjectsController> logger, IConfiguration configuration, IMapper mapper, IProjectsRepo projectsRepo)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
            _projectRepo = projectsRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string projectName = null, string orderByColumn = "Id", bool isDescending = false, int pageSize = 1000, int pageNumber = 1)
        {
            var data = await _unitOfWork.Projects.GetAllAsync(projectName, orderByColumn, isDescending, pageSize, pageNumber);
            var result = data.Where(x => !x.isDeleted);

            if (result != null && result.Any())
            {
                _logger.LogInformation("Getting all the projects");
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("Project name not found in the DB !!!");
                return NotFound("This Project Name is not present in the database, please enter another name!");
            }
        }



        [HttpPost("Save")]
        [ValidateModule]
        public async Task<IActionResult> Add(ProjectsDto product)
        {
            if (string.IsNullOrWhiteSpace(product.ProjectName))
            {
                return BadRequest("Project name is required.");
            }
            var entity = _mapper.Map<Projects>(product);
            var data = await _unitOfWork.Projects.AddAsync(entity);

            if (data == 1)
            {
                var createdProductDto = _mapper.Map<ProjectsDto>(entity);
                var response = "Project Save successfully!!!";
                return CreatedAtAction("GetById", new { id = createdProductDto.Id }, response);
            }
            return BadRequest("please entered valid data!!!");
        }

        [HttpDelete("Delete")]
        [ValidateModule]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _unitOfWork.Projects.DeleteAsync(id);
            if (data == 0)
            {
                _logger.LogError("Not able to delete this project id " + id + " from the db!!!");
                return NotFound("you are not able to Delete this project Id " + id + " May be already deleted OR please entered some data first and after that only you are able to delete this data!!!");
            }
            _logger.LogInformation("Project deleted successfully!!!");
            return Ok("Deleted Successfully!!!");
        }


        [HttpPut("Update")]
        [ValidateModule]
        public async Task<IActionResult> Update(ProjectsDto product)
        {
            var entity = _mapper.Map<Projects>(product);
            if (entity.isDeleted == true)
            {
                return NotFound("Id " + product.Id + " Not present in the database");
            }
            var result = _unitOfWork.Projects.UpdateAsync(entity);
            if (result.Result != 1)
            {
                _logger.LogWarning("Id " + product.Id + " is Not present in the database please first insert id then try to update that!!!");
                return NotFound("Id " + product.Id + " is Not present in the database please first insert id then try to update that!!!");
            }
            _logger.LogInformation("Id " + product.Id + " updated successfully!!!");
            return Ok("Updated Successfully!!!");
        }

        [HttpGet("id")]
        [ValidateModule]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            //throw new UnauthorizedAccessException();
            //throw new NotImplementedException();
            if (id < 0)
            {
                throw new ArgumentException();
            }

            var data = await _unitOfWork.Projects.GetByIdAsync(id);

            if (data == null || data.isDeleted == true)
            {
                _logger.LogWarning($"Project with ID {id} not found.");
                return NotFound($"Project with ID {id} not found. It may have been deleted or not inserted yet. Please try again.");
            }

            _logger.LogInformation("Your request is displayed on the screen, please check!!!");
            return Ok(data);          
        }

        [HttpGet("ProjectsCount")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetRecordCountProjects()
        {
            int count = _unitOfWork.Projects.GetRecordCount();
            return Ok("Number of active projects in the database is: " + count);
        }

        [HttpGet("DeletedProjectsCount")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetDeletedRecordCountProjects()
        {
            int count = _unitOfWork.Projects.GetDeletedRecordCount();
            return Ok("Number of Deleted projects is: " + count);
        }


    }
}

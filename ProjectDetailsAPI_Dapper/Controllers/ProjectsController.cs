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
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving projects.");
                return StatusCode(500, "An error occurred while retrieving projects. Please try again later.");
            }
        }



        [HttpPost("Save")]
        [ValidateModule]
        public async Task<IActionResult> Add(ProjectsDto product)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(product.ProjectName))
                {
                    return BadRequest("Project name is required.");
                }
                var entity = _mapper.Map<Projects>(product);
                var data =  await _unitOfWork.Projects.AddAsync(entity);

                if(data == 1)
                {
                    var createdProductDto = _mapper.Map<ProjectsDto>(entity);
                    var response = "Project Save successfully!!!";
                    return CreatedAtAction("GetById", new { id = createdProductDto.Id }, response);
                }
                return BadRequest("please entered valid data!!!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the product.");
            }
        }

        [HttpDelete("Delete")]
        [ValidateModule]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var data = await _unitOfWork.Projects.DeleteAsync(id);
                if (data == 0)
                {
                    _logger.LogWarning($"Not able to delete this project with ID {id} from the database.");
                    return NotFound($"You are not able to delete this project with ID {id}. It may have already been deleted or please enter some data first before deleting.");
                }

                _logger.LogInformation($"Project with ID {id} deleted successfully.");
                return Ok("Deleted Successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the project with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while deleting the project with ID {id}. Please try again later.");
            }
        }


        [HttpPut("Update")]
        [ValidateModule]
        public async Task<IActionResult> Update(ProjectsDto product)
        {
            try
            {
                var entity = _mapper.Map<Projects>(product);
                var result = await _unitOfWork.Projects.UpdateAsync(entity);
                if (result != 1)
                {
                    _logger.LogWarning($"ID {product.Id} is not present in the database. Please first insert the ID and then try to update it.");
                    return NotFound($"ID {product.Id} is not present in the database. Please first insert the ID and then try to update it.");
                }

                _logger.LogInformation($"ID {product.Id} updated successfully.");
                return Ok("Updated Successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the project with ID {product.Id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the project with ID {product.Id}. Please try again later.");
            }
        }

        [HttpGet("id")]
        [ValidateModule]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _unitOfWork.Projects.GetByIdAsync(id);

                if (data == null || data.isDeleted == true)
                {
                    _logger.LogWarning($"Project with ID {id} not found.");
                    return NotFound($"Project with ID {id} not found. It may have been deleted or not inserted yet. Please try again.");
                }

                _logger.LogInformation("Your request is displayed on the screen, please check!!!");
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the project with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the project with ID {id}. Please try again later.");
            }
        }
    }
}

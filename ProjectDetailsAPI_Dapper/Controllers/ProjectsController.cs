﻿using Application.CustomeActionFilters;
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
       

        public ProjectsController(IUnitOfWork unitOfWork, ILogger<ProjectsController> logger, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string projectName = null, string orderByColumn = "Id", bool isDescending = false, int pageSize = 1000, int pageNumber = 1)
        {
            var data = await _unitOfWork.Projects.GetAllAsync(projectName,orderByColumn,isDescending,pageSize,pageNumber);
            var result = data.Where(x => x.isDeleted == false);
            if (result != null && result.Any())
            {
                _logger.LogInformation("Getting all the projects");
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("Project name not in the DB !!!");
                return NotFound("This Project Name is not present in the database, please enter other name!!!!");
            }
        }
        
        [HttpPost("Save")]
        [ValidateModule]
        public async Task<IActionResult> Add(Projects product)
        {
            _unitOfWork.Projects.AddAsync(product);
            return CreatedAtAction("GetById", new { id = product.Id });
        }

        [HttpDelete("Delete")]
        [ValidateModule]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _unitOfWork.Projects.DeleteAsync(id);
            if (data == 0)
            {
                _logger.LogError("Not able to delete project from the db!!!");
                return NotFound("you are not able to Delete this project Id " + id + " May be already deleted OR please entered some data first and after that only you are able to delete this data!!!");
            }
            _logger.LogInformation("Project deleted successfully!!!");
            return Ok("Deleted Successfully!!!");
        }

        [HttpPut("Update")]
        [ValidateModule]
        public async Task<IActionResult> Update(Projects product)
        {
            var result = _unitOfWork.Projects.UpdateAsync(product);
            if(result.Result != 1)
            {
                _logger.LogWarning("Id" + product.Id + " is Not present in the database please first insert id then try to update that!!!");
                return NotFound("Id" + product.Id+ " is Not present in the database please first insert id then try to update that!!!");
            }

            return Ok("Updated Successfully!!!");
        }


        [HttpGet("id")]
        [ValidateModule]
        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _unitOfWork.Projects.GetByIdAsync(id);

            if (data == null || data.isDeleted == true)
            {
                _logger.LogWarning($"Project with Id {id} not Found");
                return NotFound("Id " + id + " Not found may be deleted Or not inserted yet,please try again");
            }
            _logger.LogInformation("Your request is disply on the screen please check!!!");
            return Ok(data);
        }
    }
}
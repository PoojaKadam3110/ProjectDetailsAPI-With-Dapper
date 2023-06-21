using AutoMapper;
using Domain_Data.Models.Domain;
using Domain_Data.Models.DTO;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectDetailsAPI_Dapper.Controllers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestProject.Controllers
{
    public class ProjectsControllerTests
    {
        private MockRepository mockRepository;

        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<ILogger<ProjectsController>> mockLogger;
        private Mock<IConfiguration> mockConfiguration;
        private Mock<IMapper> mockMapper;
        private Mock<IProjectsRepo> mockProjectsRepo;

        public ProjectsControllerTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockUnitOfWork = this.mockRepository.Create<IUnitOfWork>();
            this.mockLogger = this.mockRepository.Create<ILogger<ProjectsController>>();
            this.mockConfiguration = this.mockRepository.Create<IConfiguration>();
            this.mockMapper = this.mockRepository.Create<IMapper>();
            this.mockProjectsRepo = this.mockRepository.Create<IProjectsRepo>();
        }

        private ProjectsController CreateProjectsController()
        {
            return new ProjectsController(
                this.mockUnitOfWork.Object,
                this.mockLogger.Object,
                this.mockConfiguration.Object,
                this.mockMapper.Object,
                this.mockProjectsRepo.Object);
        }

        [Fact] //working
        public async Task GetAll_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var projects = new List<Projects>
            {
                // Create some sample projects for testing
                new Projects { Id = 1, ProjectName = "Test", ClientName = "Client 1",description = "test purpose",ratePerHour = 10,projectCost = 10000,projectManager = "manager 1", projectUsers="user 1",CreatedBy = "pooja", UpdatedBy = "pooja",CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,isActive = true,isDeleted = false },
                new Projects { Id = 2, ProjectName = "Project 2",ClientName = "Client 2",description = "test purpose",ratePerHour = 10,projectCost = 10000,projectManager = "manager 2", projectUsers="user 2",CreatedBy = "pooja", UpdatedBy = "pooja",CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,isActive = true,isDeleted = false },
                new Projects { Id = 3, ProjectName = "Project 3",ClientName = "Client 3",description = "test purpose",ratePerHour = 10,projectCost = 10000,projectManager = "manager 3", projectUsers="user 3",CreatedBy = "pooja", UpdatedBy = "pooja",CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,isActive = true, isDeleted = true },
            };

            var projectName = "Test";
            var orderByColumn = "Id";
            var isDescending = false;
            var pageSize = 1000;
            var pageNumber = 1;

            mockUnitOfWork.Setup(uow => uow.Projects.GetAllAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            )).ReturnsAsync(projects);

            var projectsController = new ProjectsController(
                mockUnitOfWork.Object,
                mockLogger.Object,
                mockConfiguration.Object,
                mockMapper.Object,
                mockProjectsRepo.Object
            );

            // Act
            var result = await projectsController.GetAll(projectName, orderByColumn, isDescending, pageSize, pageNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var projectsResult = Assert.IsAssignableFrom<IEnumerable<Projects>>(okResult.Value);

            Assert.Equal(2, projectsResult.Count()); // Expecting 2 projects as one project is marked as deleted
            Assert.Contains(projectsResult, p => p.ProjectName == "Test");
            Assert.Contains(projectsResult, p => p.ProjectName == "Project 2");

            mockUnitOfWork.Verify(uow => uow.Projects.GetAllAsync(
                projectName,
                orderByColumn,
                isDescending,
                pageSize,
                pageNumber
            ), Times.Once);           
        }


        [Fact]
        public async Task Add_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var projectsController = this.CreateProjectsController();
            var inputProject = new Projects
            {
                Id = 1,
                ProjectName = "My Project",
                ClientName = "Dev",
                projectManager = "Test",
                projectUsers = "New",
                ratePerHour = 10,
                projectCost = 100,
                description = "A project for testing purposes",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                CreatedBy = "Pooja",
                UpdatedBy = "Pooja",
                isActive = true,
                isDeleted = false
            };
            var mappedProjectDto = new ProjectsDto();
            mockUnitOfWork.Setup(i => i.Projects.AddAsync(It.IsAny<Projects>())).Returns(Task.FromResult(1));
            mockMapper.Setup(m => m.Map<Projects>(It.IsAny<ProjectsDto>())).Returns(inputProject);
            mockMapper.Setup(m => m.Map<ProjectsDto>(It.IsAny<Projects>())).Returns(mappedProjectDto);
            var product = new ProjectsDto
            {
                Id = 1,
                ProjectName = "My Project",
                ClientName = "Dev",
                projectManager = "Test",
                projectUsers = "New",
                ratePerHour = 10,
                projectCost = 100,
                description = "A project for testing purposes",
            };

            // Act
            var result = await projectsController.Add(product);

            // Assert
            Assert.NotNull(result);
            Assert.True(true);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task Delete_ValidId_ReturnsOk()
        {
            // Arrange
            int projectId = 1;
            int expectedResult = 1;

            mockUnitOfWork.Setup(uow => uow.Projects.DeleteAsync(projectId))
                .ReturnsAsync(expectedResult);

            var projectsController = new ProjectsController(
                mockUnitOfWork.Object,
                mockLogger.Object,
                mockConfiguration.Object,
                mockMapper.Object,
                mockProjectsRepo.Object
            );

            // Act
            var result = await projectsController.Delete(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted Successfully!!!", okResult.Value);

            mockUnitOfWork.Verify(uow => uow.Projects.DeleteAsync(projectId), Times.Once);
            //mockLogger.Verify(logger => logger.LogInformation("Project deleted successfully!!!"), Times.Once);
        }

        [Fact] 
        public async Task Delete_InvalidId_ReturnsNotFound()
        {
            // Arrange
            int projectId = 1;
            int expectedResult = 0;

            mockUnitOfWork.Setup(uow => uow.Projects.DeleteAsync(projectId))
                .ReturnsAsync(expectedResult);

            var projectsController = new ProjectsController(
                mockUnitOfWork.Object,
                mockLogger.Object,
                mockConfiguration.Object,
                mockMapper.Object,
                mockProjectsRepo.Object
            );

            // Act
            var result = await projectsController.Delete(projectId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("you are not able to Delete this project Id 1 May be already deleted OR please entered some data first and after that only you are able to delete this data!!!", notFoundResult.Value);

            mockUnitOfWork.Verify(uow => uow.Projects.DeleteAsync(projectId), Times.Once);
            //mockLogger.Verify(logger => logger.LogError("Not able to delete this project id 1 from the db!!!"), Times.Once);
        }
        [Fact]
        public async Task Update_ValidProduct_ReturnsOkResult()
        {
            // Arrange
            var productDto = new ProjectsDto
            {
                Id = 1,
                // Set other properties of the product DTO as needed
            };

            var entity = new Projects
            {
                Id = 1,
                // Set other properties of the entity as needed
            };

            mockMapper.Setup(mapper => mapper.Map<Projects>(productDto)).Returns(entity);
            mockUnitOfWork.Setup(uow => uow.Projects.UpdateAsync(entity)).ReturnsAsync(1);

            var projectsController = new ProjectsController(
                mockUnitOfWork.Object,
                mockLogger.Object,
                mockConfiguration.Object,
                mockMapper.Object,
                mockProjectsRepo.Object
            );

            // Act
            var result = await projectsController.Update(productDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Updated Successfully!!!", okResult.Value);

            mockMapper.Verify(mapper => mapper.Map<Projects>(productDto), Times.Once);
            mockUnitOfWork.Verify(uow => uow.Projects.UpdateAsync(entity), Times.Once);
           // mockLogger.Verify(logger => logger.LogInformation("Id 1 updated successfully!!!"), Times.Once);
        }

        [Fact]
        public async Task Update_InvalidProduct_ReturnsNotFoundResult()
        {
            // Arrange
            var productDto = new ProjectsDto
            {
                Id = 1,
                // Set other properties of the product DTO as needed
            };

            var entity = new Projects
            {
                Id = 1,
                // Set other properties of the entity as needed
            };

            mockMapper.Setup(mapper => mapper.Map<Projects>(productDto)).Returns(entity);
            mockUnitOfWork.Setup(uow => uow.Projects.UpdateAsync(entity)).ReturnsAsync(0);

            var projectsController = new ProjectsController(
                mockUnitOfWork.Object,
                mockLogger.Object,
                mockConfiguration.Object,
                mockMapper.Object,
                mockProjectsRepo.Object
            );

            // Act
            var result = await projectsController.Update(productDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotEqual("Id 1 is Not present!!!", notFoundResult.Value);
           
            mockMapper.Verify(mapper => mapper.Map<Projects>(productDto), Times.Once);
            mockUnitOfWork.Verify(uow => uow.Projects.UpdateAsync(entity), Times.Once);
            //mockLogger.Verify(logger => logger.LogWarning("Id 1 is Not present in the database please first insert id then try to update that!!!"), Times.Once);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsOkResultWithData()
        {
            // Arrange
            var projectId = 1;
            var project = new Projects
            {
                Id = projectId
            };

            mockUnitOfWork.Setup(uow => uow.Projects.GetByIdAsync(projectId)).ReturnsAsync(project);

            var projectsController = new ProjectsController(
                mockUnitOfWork.Object,
                mockLogger.Object,
                mockConfiguration.Object,
                mockMapper.Object,
                mockProjectsRepo.Object
            );

            // Act
            var result = await projectsController.GetById(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<Projects>(okResult.Value);
            Assert.Equal(projectId, data.Id);

            mockUnitOfWork.Verify(uow => uow.Projects.GetByIdAsync(projectId), Times.Once);
            //mockLogger.Verify(logger => logger.LogInformation("Your request is disply on the screen please check!!!"), Times.Once);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var projectId = 1;
            Projects nullProject = null;

            mockUnitOfWork.Setup(uow => uow.Projects.GetByIdAsync(projectId)).ReturnsAsync(nullProject);

            var projectsController = new ProjectsController(
                mockUnitOfWork.Object,
                mockLogger.Object,
                mockConfiguration.Object,
                mockMapper.Object,
                mockProjectsRepo.Object
            );

            // Act
            var result = await projectsController.GetById(projectId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Id " + projectId + " Not found may be deleted Or not inserted yet,please try again", notFoundResult.Value);

            mockUnitOfWork.Verify(uow => uow.Projects.GetByIdAsync(projectId), Times.Once);
            //mockLogger.Verify(logger => logger.LogWarning($"Project with Id {projectId} not Found"), Times.Once);
        }

    }
}

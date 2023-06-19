using AutoMapper;
using Domain_Data.Models.Domain;
using Domain_Data.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain_Data.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Projects, ProjectsDto>().ReverseMap();
            CreateMap<ProjectsDto, Projects>().ReverseMap(); //use for getAll() and getBtID() 1st is sorce and 2nd is dest  dto to domain model
            CreateMap<UpdateProjectsDto, Projects>().ReverseMap();  //Used for update()

        }
    }
}

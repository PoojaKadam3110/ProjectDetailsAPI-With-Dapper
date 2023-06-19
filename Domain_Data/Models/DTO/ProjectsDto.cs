using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Data.Models.DTO
{
    public class ProjectsDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public double projectCost { get; set; }
        public string projectManager { get; set; }
        public double ratePerHour { get; set; }
        public string projectUsers { get; set; }
        [MaxLength(1000)]
        public string description { get; set; }
    }
}

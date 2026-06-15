using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_JobService.Application.Dtos
{

        public class CreateJobFromImageDto
        {
            public int UserId { get; set; }
            public string ImageUrl { get; set; } = null!;
            public string? Brand { get; set; }
            public string? ModelNumber { get; set; }
            public string? SerialNumber { get; set; }
            // при необходимости можно добавить поля под распознавания
        }
    

}

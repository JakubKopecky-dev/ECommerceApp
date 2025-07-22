using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.DTOs.Category
{
    public class CreateUpdateCategoryDto
    {
        [MinLength(2)]
        public string Title { get; set; } = "";
    }
}

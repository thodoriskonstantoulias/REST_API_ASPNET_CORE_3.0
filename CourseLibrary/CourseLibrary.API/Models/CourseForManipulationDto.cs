using CourseLibrary.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    [CourseValidTitleDesc(ErrorMessage = "Title must be different from the description")]
    public abstract class CourseForManipulationDto
    {
        [Required(ErrorMessage = "Fill the title")]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(1500)]
        public virtual string Description { get; set; }
    }
}

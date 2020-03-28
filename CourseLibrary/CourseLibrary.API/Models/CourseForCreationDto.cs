﻿using CourseLibrary.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    [CourseValidTitleDesc(ErrorMessage ="Title must be different from the description")]
    public class CourseForCreationDto // : IValidatableObject
    {
        [Required(ErrorMessage ="Fill the title")]
        [MaxLength(100)]
        public string Title { get; set; }
         
        [MaxLength(1500)]
        public string Description { get; set; }
        
        //Create custom validation if data annotations are not enough
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title == Description)
        //    {
        //        yield return new ValidationResult("The description should be different from the title", new[] {"CourseForCreationDto"}); 
        //    }
        //}
    }
}

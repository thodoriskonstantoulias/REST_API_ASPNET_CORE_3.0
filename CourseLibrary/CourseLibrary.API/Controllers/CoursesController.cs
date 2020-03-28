﻿using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CoursesDto>> GetAuthorCourses(Guid authorId)
        {
            //Check if author exists
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var authorCourses = _courseLibraryRepository.GetCourses(authorId);

            return Ok(_mapper.Map<IEnumerable<CoursesDto>>(authorCourses));
        }

        [HttpGet("{courseId}", Name ="GetCourse")]
        public ActionResult<CoursesDto> GetAuthorCourse(Guid authorId, Guid courseId)
        {
            //Check if author exists
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var authorCourse = _courseLibraryRepository.GetCourse(authorId, courseId);

            if(authorCourse == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CoursesDto>(authorCourse));
        }

        [HttpPost]
        public ActionResult<CoursesDto> CreateCourseForAuthor(Guid authorId, CourseForCreationDto courseForCreationDto)
        {
            //Check if author exists
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseEntity = _mapper.Map<Course>(courseForCreationDto);

            _courseLibraryRepository.AddCourse(authorId, courseEntity);
            _courseLibraryRepository.Save();

            var courseToReturn = _mapper.Map<CoursesDto>(courseEntity);

            return CreatedAtRoute("GetCourse", new { authorId = authorId, courseId = courseToReturn.Id }, courseToReturn);
        }

        [HttpPut("{courseId}")]
        public ActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId, CourseForUpdateDto courseForUpdate)
        {
            //Check if author exists
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var authorCourse = _courseLibraryRepository.GetCourse(authorId, courseId);
            if (authorCourse == null)
            {
                return NotFound();
            }

            _mapper.Map(courseForUpdate, authorCourse);

            _courseLibraryRepository.UpdateCourse(authorCourse);
            _courseLibraryRepository.Save();

            return NoContent();
        }
    }
}

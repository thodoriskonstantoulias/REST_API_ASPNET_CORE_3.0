using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
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
        public IActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId, CourseForUpdateDto courseForUpdate)
        {
            //Check if author exists
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var authorCourse = _courseLibraryRepository.GetCourse(authorId, courseId);
            if (authorCourse == null)
            {
                //Upserting
                var courseToAdd = _mapper.Map<Course>(courseForUpdate);
                courseToAdd.Id = courseId;

                _courseLibraryRepository.AddCourse(authorId, courseToAdd);
                _courseLibraryRepository.Save();

                var courseToReturn = _mapper.Map<CoursesDto>(courseToAdd);

                return CreatedAtRoute("GetCourse", new { authorId = authorId, courseId = courseToReturn.Id }, courseToReturn);
            }

            _mapper.Map(courseForUpdate, authorCourse);

            _courseLibraryRepository.UpdateCourse(authorCourse);
            _courseLibraryRepository.Save();

            return NoContent(); 
        }

        [HttpPatch("{courseId}")]
        public ActionResult PartiallyUpdateCourseForAuthor(Guid authorId, Guid courseId, JsonPatchDocument<CourseForUpdateDto> patchDocument)
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

            var courseToPatch = _mapper.Map<CourseForUpdateDto>(authorCourse);
            patchDocument.ApplyTo(courseToPatch);

            _mapper.Map(courseToPatch, authorCourse);

            _courseLibraryRepository.UpdateCourse(authorCourse);
            _courseLibraryRepository.Save();

            return NoContent();
        }
    }
}

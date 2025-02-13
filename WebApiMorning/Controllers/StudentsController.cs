using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiMorning.Data;
using WebApiMorning.Dtos;
using WebApiMorning.Entities;
using WebApiMorning.Repositories.Abstract;

namespace WebApiMorning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;

        public StudentsController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<StudentDto>> Get()
        {
            var items = await _studentRepository.GetAll();
            var dataToReturn = items.Select(i => new StudentDto
            {
                Id = i.Id,
                Age = i.Age,
                Fullname = i.Fullname,
                Score = i.Score,
                SeriaNo = i.SeriaNo,
            });
            return dataToReturn;
        }

        [HttpGet("{id}")]
        public async Task<StudentDto> Get(int id)
        {
            var item = await _studentRepository.Get(s => s.Id == id);
            var dataToReturn = new StudentDto
            {
                Id = item.Id,
                Age = item.Age,
                Fullname = item.Fullname,
                Score = item.Score,
                SeriaNo = item.SeriaNo,
            };
            return dataToReturn;
        }

        [HttpPost]
        public async Task<ActionResult<StudentAddDto>> Post([FromBody] StudentAddDto dto)
        {
            var entity = new Student
            {
                Age = dto.Age,
                Fullname = dto.Fullname,
                Score = dto.Score,
                SeriaNo = dto.SeriaNo,
            };
            await _studentRepository.Add(entity);
            return Created();
        }
    }
}

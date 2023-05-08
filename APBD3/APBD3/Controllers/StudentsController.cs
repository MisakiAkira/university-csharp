using Microsoft.AspNetCore.Mvc;

namespace APBDS24610.Controllers
{

    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {

        [HttpGet]
        public ObjectResult GetAllStudents()
        {
            return Ok(Students.GetStudents());
        }

        [HttpGet("{index}")]
        public ObjectResult GetStudentByIndex(string index)
        {
            Student? tmp = Students.GetStudentByIndex(index);
            return tmp != null ? Ok(tmp) : NotFound("Student with this index not found");
        }

        [HttpPut("{index}")]
        public StatusCodeResult PutIndex(string index, Student student)
        {
            return Students.Put(index, student) ? Ok() : BadRequest();
        }

        [HttpPost]
        public StatusCodeResult Post(Student student)
        {
            return Students.Post(student) ? Ok() : BadRequest();
        }

        [HttpDelete]
        public StatusCodeResult Delete(string index)
        {
            return Students.Delete(index) ? Ok() : BadRequest();
        }
    }
}

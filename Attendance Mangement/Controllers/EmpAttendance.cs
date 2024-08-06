using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Attendance_Mangement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpAttendance : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmpAttendance(IConfiguration config)
        {
            _config = config;
        }


        [HttpGet("{Empid}")]

        public async Task<ActionResult<Employee>> GetEmployee(int Empid)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var employee = await connection.QueryFirstAsync<Employee>("select * from Employees where Id=@id",
                new { id = Empid });

            return Ok(employee);
        }
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn(int empId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var employeecheck = await connection.QueryFirstOrDefaultAsync<Employee>(
                "Select * from Employees where Id=@id", new { id = empId });
            if (employeecheck == null)
            {
                return BadRequest("Employee not found !!!");
            }

            var today = DateTime.Today;
            var attendance = await connection.QueryFirstOrDefaultAsync<Attendance>(
                "SELECT * FROM Attendance WHERE EmpId = @EmpId AND CAST(CheckIn AS DATE) = @Today",
                new { EmpId = empId, Today = today });
            

            if (attendance != null)
            {
                return BadRequest("Already checked in today.");
            }
           
                var checkIn = new Attendance { EmpId = empId, CheckIn = DateTime.Now };
                var query = "INSERT INTO Attendance (EmpId, CheckIn) VALUES (@EmpId, @CheckIn)";
                await connection.ExecuteAsync(query, checkIn);

                return Ok("Checked in successfully.");
            
        }
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut(int empId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var today = DateTime.Today;
            var attendance = await connection.QueryFirstOrDefaultAsync<Attendance>(
                "SELECT * FROM Attendance WHERE EmpId = @EmpId AND CAST(CheckIn AS DATE) = @Today",
                new { EmpId = empId, Today = today });


            if (attendance != null && attendance.CheckOut== null)
            {
                var checkOut = new Attendance { EmpId = empId, CheckOut = DateTime.Now };
                var query = "UPDATE Attendance SET CheckOut = @CheckOut WHERE EmpId = @EmpId";
                await connection.ExecuteAsync(query, checkOut);

                return Ok("Checked out successfully.");
            }
            else
            {
                return BadRequest("No Check in available or already checked out");
            }

            
        }
        [HttpGet]

        public async Task<ActionResult<AttendanceWithEmployee>> ShowEmployee()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var today = DateTime.Today;
            var employee = await connection.QueryAsync<AttendanceWithEmployee>("Select e.[Name],* from Attendance A left join Employees e on A.EmpId=e.id where A.CheckIn is NOT NULL and CAST(CheckIn AS DATE) = @Today",
            new { Today = today });

            return Ok(employee);
        }
    }
}

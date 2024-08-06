namespace Attendance_Mangement
{
        public class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Designation { get; set; }
        }

        public class Attendance
        {
            public int Id { get; set; }
            public int EmpId { get; set; }
            public DateTime? CheckIn { get; set; }
            public DateTime? CheckOut { get; set; }
        }
    public class AttendanceWithEmployee
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string Name { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        
    }


}

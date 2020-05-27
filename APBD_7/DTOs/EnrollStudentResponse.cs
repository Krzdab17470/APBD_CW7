using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_7.DTOs
{
	public class EnrollStudentResponse
	{

		public string LastName { get; set; }

		public string Index { get; set; }

		public DateTime StartDate { get; set; }

		public int Semester { get; set; }

		public int IdEnrollment { get; set; }

		public int IdStudies { get; set; }

		public string StudiesName { get; set; }
	}
}
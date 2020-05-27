using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_7.DTOs
{
	public class PromoteStudents
	{

		[Required]
		public string Studies { get; set; }

		[Required]
		public int Semester { get; set; }
	}
}
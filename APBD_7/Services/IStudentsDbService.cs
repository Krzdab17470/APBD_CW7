using APBD_7.DTOs;
using APBD_7.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_7.Services
{
    public interface IStudentsDbService
    {
        Enrollment EnrollStudent(EnrollStudentRequest request);

        Enrollment PromoteStudents(int semester, string studies);
    }
}
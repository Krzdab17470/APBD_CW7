using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Net;
using System.Data;
using APBD_7.Services;
using APBD_7.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace APBD_Cw3.Controllers
{

    //https://localhost:44362/api/students
    //http://localhost:51042/api/enrollments
    //http://localhost:51042/api/enrollments/promotions

    //ZAD 3
    [Microsoft.AspNetCore.Mvc.Route("api/enrollments")]
    [ApiController]
    [Authorize(Roles = "employee")]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentsDbService _service;

        public EnrollmentsController(IStudentsDbService service)
        {
            _service = service;
        }
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            return Ok(_service.EnrollStudent(request));
        }


    }

}
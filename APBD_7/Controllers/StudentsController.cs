using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using APBD_7.DTOs;
using APBD_7.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace APBD_7.Controllers
{
    //https://localhost:44362/api/students
    //http://localhost:51042/api/enrollments
    //http://localhost:51042/api/enrollments/promotions


    [Route("api/[controller]")]
    [ApiController]
   
    public class StudentsController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }
        public StudentsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetStudents()
        {
            var list = new List<Student>();
            list.Add(new Student
            {
               // IdStudent = 1,
                FirstName = "Jan",
                LastName = "Kowalski"
            });
            list.Add(new Student
            {
               // IdStudent = 2,
                FirstName = "Andrzej",
                LastName = "Malewski"
            });

            return Ok(list);
        }
        

        [HttpPost]
        public IActionResult Login(LoginRequestDto request)
        {

            //..TODO sprawdzic w bazie danych czy login i haslo sa poprawne..
            var claims = new[]
{
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "jan123"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "student")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            return Ok(new
            {
               accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = Guid.NewGuid()
            });
            
        }

        [HttpPost("refresh-token/{token}")]
        public IActionResult RefreshToken(string refToken){

            //..TODO sprawdz czy refreshtoken jest w bazie, a wcześniej musieliśmy go zapisać w bazie danych
            return Ok();
        }
        
    }
}
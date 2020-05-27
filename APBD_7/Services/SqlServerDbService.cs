using APBD_7.DTOs;
using APBD_7.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_7.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        public Enrollment EnrollStudent(EnrollStudentRequest request)
        {
            var st = new Student();
            st.LastName = request.LastName;
            st.FirstName = request.FirstName;
            st.BirthDate = request.BirthDate;
            st.Studies = request.Studies;
            st.Semester = 1;

            var enrollment = new Enrollment();
            enrollment.Semester = 1;

            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s17470;Integrated Security=True"))
            using (var com = new SqlCommand())
            {

                com.Connection = con;
                con.Open(); //otiweramy polaczenie
                var tran = con.BeginTransaction(); //otwieramy nowa transakcje

                try
                {
                    //1. Spr czy istnieja studia
                    com.CommandText = "SELECT IdStudy FROM Studies WHERE name=@name"; //SQL command
                    com.Parameters.AddWithValue("name", request.Studies);  //set value to @name parameter
                    //response.StudiesName = request.Studies;
                    com.Transaction = tran; //musi byc przed ExecuteReader
                    var dr = com.ExecuteReader(); //odczytujemy efekt zapytania
                    if (!dr.Read()) //jesli zapytanie NIC nie zwrocilo..
                    {
                        dr.Close();
                        tran.Rollback(); //wycofujemy transakcje
                                         // return BadRequest("Studia nie istnieja.");  //musimy zwrocic blad
                    }
                    int idStudies = (int)dr["IdStudy"]; //bierzemy number id studiow, przyda sie pozniej...
                    //response.IdStudies = idStudies;
                    enrollment.IdStudies = idStudies;
                    dr.Close();

                    //2. Spr czy nr indexu studenta jest unikalny
                    com.CommandText = "SELECT INDEXNUMBER FROM STUDENT WHERE INDEXNUMBER = @Index";
                    com.Parameters.AddWithValue("Index", request.IndexNumber);
                    dr = com.ExecuteReader(); //odczytujemy efekt zapytania

                    if (dr.Read()) //jesli zapytanie cos zwrocilo...
                    {
                        dr.Close();
                        tran.Rollback(); //wycofujemy transakcje
                        //return BadRequest("W bazie istnieje juz ten number indeksu.");  //musimy zwrocic blad
                    }

                    dr.Close();

                    int IdEnrollment = 0;

                    //3. odnajdujemy najnowszy wpis w tabeli Enrollments zgodny ze studiami studenta i wartoscia Semester = 1
                    com.CommandText = "Select IdEnrollment FROM Enrollment WHERE Semester = 1 AND IdStudy =" + idStudies;
                    dr = com.ExecuteReader(); //odczytujemy efekt zapytania
                    if (dr.Read()) //jesli zapytanie cos zwrocilo...
                    {

                        IdEnrollment = ((int)dr["IdEnrollment"]);
                        dr.Close();

                    }
                    else if (!dr.Read())
                    {
                        dr.Close();
                        com.CommandText = "Select IdEnrollment FROM Enrollment WHERE IdEnrollment = (Select MAX(IdEnrollment) FROM Enrollment)";
                        dr = com.ExecuteReader(); //Dodane
                        dr.Read();
                        IdEnrollment = ((int)dr["IdEnrollment"]) + 1;
                        dr.Close();
                        com.CommandText = "INSERT INTO ENROLLMENT (IDENROLLMENT,SEMESTER,IDSTUDY,STARTDATE) VALUES (" + IdEnrollment + ",1," + idStudies + ", '2021-09-10')";

                        com.ExecuteNonQuery();
                    }
                    else
                    {
                        tran.Rollback();
                        // return BadRequest("ERROR!");
                    };


                    //response.IdEnrollment = IdEnrollment;
                    enrollment.IdEnrollment = IdEnrollment;

                    com.CommandText = "INSERT INTO Student(IndexNumber, FirstName,LastName,BirthDate,IdEnrollment) VALUES (@index2,@firstName,@lastName,@birthDate,@idEnrollment)"; //nowe zapytanie, a raczej wypchniecie danych do tabeli
                    com.Parameters.AddWithValue("index2", request.IndexNumber); //przypisanie wartosci do powyzszych paramterow
                    com.Parameters.AddWithValue("firstName", request.FirstName);    //j.w.
                    com.Parameters.AddWithValue("lastName", request.LastName);
                    com.Parameters.AddWithValue("birthDate", "1993-09-11");
                    com.Parameters.AddWithValue("idEnrollment", IdEnrollment);

                    com.ExecuteNonQuery(); //wypychamy zapytanie
                    tran.Commit(); //potwierdzamy transakcje

                    dr.Close();
                    com.CommandText = "Select * From Enrollment WHERE IdEnrollment = " + IdEnrollment;
                    dr = com.ExecuteReader();
                    string message = "";
                    while (dr.Read())
                    {
                        //message = string.Concat(message, '\n', "Enrollment ID:" , dr["IdEnrollment"].ToString(), ", Semester: ", dr["Semester"].ToString());
                        message = string.Concat(message, '\n', "Enrollment ID:", enrollment.IdEnrollment.ToString(), ", Semester: ", enrollment.Semester.ToString(), ", ID Studies: :", enrollment.IdStudies.ToString());
                    }


                    //return Ok(message);


                }
                catch (SqlException exc) //wylapujemy ewentualny blad...
                {
                    tran.Rollback(); //...i w razie czego robimy rollback

                    // return BadRequest(exc.ToString());
                }

            }

            return enrollment;

        }


        public Enrollment PromoteStudents(int semester, string studies)
        {
            using var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s17470;Integrated Security=True");
            using var com = new SqlCommand
            {
                Connection = con
            };
            con.Open(); //otiweramy polaczenie
            var tran = con.BeginTransaction(); //otwieramy nowa transakcje

            com.CommandText = "EXEC PROMOTESTUDENTS @STUDIES = @studies, @SEMESTER = @semester;"; //SQL command

            com.Parameters.AddWithValue("studies", studies);  //set value to @name parameter
            com.Parameters.AddWithValue("semester", semester);  //set value to @name parameter


            com.Transaction = tran; //musi byc przed ExecuteReader
            var dr = com.ExecuteReader(); //odczytujemy efekt zapytania
            int idEnrollment;


            Enrollment enrollment = new Enrollment();

            if (!dr.Read()) //jesli zapytanie NIC nie zwrocilo..
            {
                tran.Rollback();
                //return BadRequest("Wrong request.");
            }
            else
            {

                enrollment.IdEnrollment = (int)dr["IdEnrollment"];
                enrollment.IdStudies = (int)dr["IdStudy"];
                enrollment.Semester = (int)dr["Semester"];
                //enrollment.StartDate = (string) dr["StartDate"];

                idEnrollment = (int)dr[0];

            }
            dr.Close();

            tran.Commit();



            return enrollment;
        }
    }
}
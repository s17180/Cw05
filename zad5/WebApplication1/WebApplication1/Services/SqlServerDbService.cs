using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DTOs.Requests;
using WebApplication1.DTOs.Responses;

namespace WebApplication1.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
    
                
                public EnrollStudentResponse EnrollStudent(EnrollStudentRequest studentRequest)
                {
                    var insertedStudent = new EnrollStudentResponse();

                    using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s17180;Integrated Security=True"))
                    using (var com = new SqlCommand())

                    {
                com.Connection = con;
                com.CommandText = "select IdStudy from Studies where name =@name";
                com.Parameters.AddWithValue("name", studentRequest.Studies);
                con.Open();
                var tran = con.BeginTransaction();
                // idStudies 
                com.Transaction = tran;

                var dr = com.ExecuteReader();
                            if (!dr.Read())
                            {
                                dr.Close();
                                tran.Rollback();
                            }
                          //  dr.Close();
                            string idstudies = dr["IdStudy"].ToString();
                            dr.Close();
                           
                            int idEnrollment =0 ;

                com.CommandText = "select  ISNULL(max(  IdEnrollment), 0)'maxid'  from Enrollment;";
                dr.Close();
                dr = com.ExecuteReader();
                if (dr.Read())
                {
                    idEnrollment =(int) dr["maxid"];
                }
                else
                {
                    tran.Rollback();
                   
                }

                int newIdEnrolment = idEnrollment + 1;



                com.CommandText = $"INSERT INTO Enrollment VALUES({ newIdEnrolment}, 1, {idstudies}, GETDATE());";


                dr.Close();

                            com.Parameters.AddWithValue("IdStudies", studentRequest.Studies);
                            com.Parameters.AddWithValue("date", DateTime.Now.ToString());
                            com.ExecuteNonQuery();

                            com.CommandText = "Insert into Student(IndexNumber, Firstname, lastname, birthdate,IdEnrollment) values(@Index,@fname,@lname,@bday,@IdEnrollment)";
                            com.Parameters.AddWithValue("index", studentRequest.IndexNumber);
                            com.Parameters.AddWithValue("fname", studentRequest.FirstName);
                            com.Parameters.AddWithValue("lname", studentRequest.LastName);
                            com.Parameters.AddWithValue("bday", DateTime.Parse(studentRequest.Birthdate));
                          //  com.Parameters.AddWithValue("stud", studentRequest.Studies);
                            com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                            com.ExecuteNonQuery();

                dr = com.ExecuteReader();

                insertedStudent.IdEnrollment = idEnrollment;
                            insertedStudent.Semester = 1;
                            insertedStudent.Name = dr["name"].ToString();
                            insertedStudent.StartDate = DateTime.Parse(dr["date"].ToString());

                            tran.Commit();
                    
                    }

                    return insertedStudent;
                }
            }

    }

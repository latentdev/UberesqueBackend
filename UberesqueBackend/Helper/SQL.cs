using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using UberesqueBackend.Models;

namespace UberesqueBackend.Helper
{
    public class SQL
    {
        SqlConnection uberesqueDB;
       public SQL()
        {
            string cString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            //var temp = temp2.ConnectionString;
            uberesqueDB = new SqlConnection(cString);
        }

        public Response RegisterUser(User in_user, Vehicle in_vehicle=null)
        {
            Response response = null;            
            try
            {
                uberesqueDB.Open();
                using (SqlCommand checkUserExist = new SqlCommand("dbo.CheckUser", uberesqueDB))
                {
                    checkUserExist.CommandType = System.Data.CommandType.StoredProcedure;
                    checkUserExist.Parameters.AddWithValue("@UserName", in_user.UserName);
                    SqlDataReader reader = checkUserExist.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        reader.Close();
                        checkUserExist.Dispose();
                        using (SqlCommand command = new SqlCommand("dbo.RegisterUser", uberesqueDB))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@UserName", in_user.UserName);
                            command.Parameters.AddWithValue("@Password", in_user.Password);
                            command.Parameters.AddWithValue("@FirstName", in_user.FirstName);
                            command.Parameters.AddWithValue("@LastName", in_user.LastName);
                            command.Parameters.AddWithValue("@Driver", in_user.Driver);

                            if (in_user.Driver != false)
                            {
                                command.Parameters.AddWithValue("@Color", in_vehicle.Color);
                                command.Parameters.AddWithValue("@Make", in_vehicle.Make);
                                command.Parameters.AddWithValue("@Model", in_vehicle.Model);
                                command.Parameters.AddWithValue("@Plate", in_vehicle.Plate);
                            }
                            command.ExecuteNonQuery();
                            command.Dispose();

                            response = new ResponseUser(in_user, in_vehicle);
                        }
                    }
                    else
                    {
                        response = new Response();
                        response.success = false;
                        response.error = "Username already exists";
                    }

                }
                uberesqueDB.Close();
            }
            catch (Exception e)
            {
                response = new Response();
                response.success = false;
                response.error = e.ToString();
            }
            return response;
        }

        public Response login(string username, string password)
        {
            Response response;

            try
            {
                uberesqueDB.Open();

                User user = null;
                Vehicle vehicle = null;
                using (SqlCommand command = new SqlCommand("dbo.LoginUser", uberesqueDB))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserName", username.ToString());
                    command.Parameters.AddWithValue("@Password", password.ToString());
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        user = new User();
                        user.UserID = (int)reader["UserID"];
                        user.UserName = (string)reader["UserName"];
                        user.Password = (string)reader["Password"];
                        user.FirstName = (string)reader["FirstName"];
                        user.LastName = (string)reader["LastName"];
                        user.Driver = (bool)reader["Driver"];
                        reader.Close();


                        if (user.Driver == true)
                        {
                            using (SqlCommand command2 = new SqlCommand("dbo.CheckVehicle", uberesqueDB))
                            {
                                command2.CommandType = System.Data.CommandType.StoredProcedure;
                                command2.Parameters.AddWithValue("@UID", user.UserID);
                                SqlDataReader reader2 = command2.ExecuteReader();
                                if (reader2.HasRows)
                                {
                                    reader2.Read();
                                    vehicle = new Vehicle();
                                    vehicle.Color = (string)reader2["Color"];
                                    vehicle.Make = (string)reader2["Make"];
                                    vehicle.Model = (string)reader2["Model"];
                                    vehicle.Plate = (string)reader2["Plate"];
                                }
                            }

                        }
                        response = new ResponseUser(user, vehicle);
                        response.success = true;
                        response.error = null;
                    }



                    else
                    {
                        response = new Response();
                        response.success = false;
                        response.error = "user does not exist";

                    }
                    uberesqueDB.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                response = new Response();
                response.success = false;
                response.error = e.ToString();
            }
            return response;
        }
        

        public void Dispose()
        {
            uberesqueDB.Dispose();
        }

    }
}

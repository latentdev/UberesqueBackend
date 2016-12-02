using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using UberesqueBackend.Models;
using System.Web.Script.Serialization;

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
        //need to do: change success to true upon creating user
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
                            command.Parameters.AddWithValue("@Email", in_user.Email);
                            command.Parameters.AddWithValue("@UserName", in_user.UserName);
                            command.Parameters.AddWithValue("@Password", in_user.Password);
                            command.Parameters.AddWithValue("@FirstName", in_user.FirstName);
                            command.Parameters.AddWithValue("@LastName", in_user.LastName);
                            command.Parameters.AddWithValue("@Driver", in_user.Driver);

                            if (in_user.Driver != false)
                            {
                                command.Parameters.AddWithValue("@Color", in_vehicle.Color);
                                command.Parameters.AddWithValue("@Year", in_vehicle.Year);
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
                        user.Email = (string)reader["Email"];
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
                                    vehicle.Year = (int)reader2["Year"];
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
        public Response RequestRide(Ride ride)
        {
            Response response = null;
            try
            {
                uberesqueDB.Open();
                using (SqlCommand rideRequest = new SqlCommand("dbo.RideRequest", uberesqueDB))
                {
                    rideRequest.CommandType = System.Data.CommandType.StoredProcedure;
                    rideRequest.Parameters.AddWithValue("@UserID", ride.UserID);
                    rideRequest.Parameters.AddWithValue("@Location", ride.Location);
                    rideRequest.Parameters.AddWithValue("@Location_Lat", ride.Location_Lat);
                    rideRequest.Parameters.AddWithValue("@Location_Long", ride.Location_Long);
                    rideRequest.Parameters.AddWithValue("@Destination", ride.Destination);
                    rideRequest.Parameters.AddWithValue("@Destination_Lat", ride.Destination_Lat);
                    rideRequest.Parameters.AddWithValue("@Destination_Long", ride.Destination_Long);
                    rideRequest.ExecuteNonQuery();
                    rideRequest.Dispose();
                    response = new Response();
                    response.success = true;
                    response.error = null;
                }
                uberesqueDB.Close();
            }
            catch(Exception e)
            {
                response = new Response();
                response.success = true;
                response.error = e.ToString();
            }
            return response;
        }
        public Response Rides(string username, string password)
        {
            Response response = null;
            List<Ride> rides = new List<Ride>();
            try
            {
                uberesqueDB.Open();
                using (SqlCommand checkUserExist = new SqlCommand("dbo.CheckUser", uberesqueDB))
                {
                    checkUserExist.CommandType = System.Data.CommandType.StoredProcedure;
                    checkUserExist.Parameters.AddWithValue("@UserName", username);
                    SqlDataReader reader = checkUserExist.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        bool isDriver = (bool)reader["Driver"];
                        reader.Close();
                        if (isDriver)
                        {
                            using (SqlCommand getRides = new SqlCommand("dbo.GetRides", uberesqueDB))
                            {
                                getRides.CommandType = System.Data.CommandType.StoredProcedure;
                                SqlDataReader reader2 = getRides.ExecuteReader();
                                if (reader2.HasRows)
                                {

                                    while (reader2.Read())
                                    {
                                        Ride ride = null;
                                        ride = new Ride();
                                        ride.User = (string)reader2["UserName"];
                                        ride.Location = (string)reader2["Location"];
                                        ride.Location_Lat = (double)reader2["Location_Lat"];
                                        ride.Location_Long = (double)reader2["Location_Long"];
                                        ride.Destination = (string)reader2["Destination"];
                                        ride.Destination_Lat = (double)reader2["Destination_Lat"];
                                        ride.Destination_Long = (double)reader2["Destination_Long"];
                                        ride.Accepted = (bool)reader2["Accepted"];
                                        ride.Completed = (bool)reader2["Completed"];
                                        rides.Add(ride);
                                    }
                                }
                            }

                            //var x = new JavaScriptSerializer().Serialize(rides);
                            response = new ResponseRides(rides);
                            response.success = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
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

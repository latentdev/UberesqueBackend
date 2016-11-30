using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UberesqueBackend.Models;
using UberesqueBackend.Helper;

namespace UberesqueBackend.Controllers
{
    public class AccountController : ApiController
    {
        /*
        // GET: api/Account
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }*/
        /*
        // GET: api/Account/5
        public string Get(int id)
        {
            return "value";
        }*/

        // POST: api/Account/Register
        public IHttpActionResult Register(string email,string username,string pass,string firstname,string lastname, bool driver, string color=null,int? year=null,string make=null,string model=null,string plate=null)
        {
            User user = new User();
            Vehicle vehicle = null;
            user.Email = email;
            user.UserName = username;
            user.Password = pass;
            user.FirstName = firstname;
            user.LastName = lastname;
            user.Driver = driver;
            if (driver!=false)
            {
                vehicle = new Vehicle();
                vehicle.Color = color;
                vehicle.Year = (int)year;
                vehicle.Make = make;
                vehicle.Model = model;
                vehicle.Plate = plate;
            }
            SQL sql = new SQL();
            Response response=sql.RegisterUser(user, vehicle);
            sql.Dispose();
            return Json(response);
        }
        // GET: api/Account/Login
        //[Route("Login")]
        [HttpGet]
        public IHttpActionResult Login(string username, string password)
        {
            SQL sql = new SQL();
            Response response=sql.login(username, password);
            sql.Dispose();
            
            return Json(response);
        }

        [HttpPost]
        public IHttpActionResult RequestRide(int uid,string location,float location_lat,float location_long,string destination,float destination_lat,float destination_long)
        {
            User user= new Models.User();
            user.UserID = uid;
            Ride ride = new Ride(user);
            ride.Location = location;
            ride.Location_Lat = location_lat;
            ride.Location_Long = location_long;
            ride.Destination = destination;
            ride.Destination_Lat = destination_lat;
            ride.Destination_Long = destination_long;
            SQL sql = new SQL();
            Response response = sql.RequestRide(ride);
            return Json(response);
        }

        [HttpGet]
        public IHttpActionResult Rides(string username, string password)
        {
            SQL sql = new Helper.SQL();
            Response rides = sql.Rides(username, password);
            return Json(rides);
        }

        // DELETE: api/Account/5
        public void Delete(int id)
        {
        }
    }
}

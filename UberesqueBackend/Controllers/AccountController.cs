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
        public IHttpActionResult Register(string uid,string pass,string firstname,string lastname, bool driver, string color=null,string make=null,string model=null,string plate=null)
        {
            User user = new User();
            Vehicle vehicle = null;
            user.UserName = uid;
            user.Password = pass;
            user.FirstName = firstname;
            user.LastName = lastname;
            user.Driver = driver;
            if (driver!=false)
            {
                vehicle = new Vehicle();
                vehicle.Color = color;
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

        // DELETE: api/Account/5
        public void Delete(int id)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UberesqueBackend.Models
{
    public class User
    {
        public int UserID { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public Boolean Driver { get; set; }
    }
}
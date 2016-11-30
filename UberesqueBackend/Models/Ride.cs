using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UberesqueBackend.Models
{
    public class Ride
    {
        public User user;
        public String Location { get; set; }
        public double Location_Lat { get; set; }
        public double Location_Long { get; set; }
        public String Destination { get; set; }
        public double Destination_Lat { get; set; }
        public double Destination_Long { get; set; }
        public bool Accepted { get; set; }
        public bool Completed { get; set; }

        public Ride(User in_user)
        {
            user = in_user;
        }
    }
}
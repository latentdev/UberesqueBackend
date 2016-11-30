using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UberesqueBackend.Models
{
    public class Response
    {
        public Boolean success { get; set; }
        public String error { get; set; }
    }

    public class ResponseUser:Response
    {
        public User user;
        public Vehicle vehicle;

        public ResponseUser(User in_user, Vehicle in_vehicle)
        {
            user = in_user;
            vehicle = in_vehicle;
        }
    }

    public class ResponseRides : Response
    {
        public List<Ride> rides;
       

        public ResponseRides(List<Ride> in_rides)
        {
            rides = in_rides;
        }
    }
}
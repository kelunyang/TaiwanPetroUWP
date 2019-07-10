using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaiwanPetroLibrary.Helpers
{
    public class GeoPoint
    {
        public double Latitude;
        public double Longitude;
        public string remark;
        public GeoPoint(double lat, double lon, string re = "none")
        {
            Latitude = lat;
            Longitude = lon;
            remark = re;
        }
    }
}

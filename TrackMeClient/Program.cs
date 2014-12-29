using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TrackMeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TrackMeClient.ServiceReference1.ILocationService service = new TrackMeClient.ServiceReference1.LocationServiceClient();

            double lat = 55, lon = 34;
            Random r = new Random();

            for (int i = 1; i < 100; i++)
            {
                service.SetLocation("1", lat, lon);
                lat += r.NextDouble() / 1000;
                lon += r.NextDouble() / 1000;
                Thread.Sleep(2000);
            }
        }
    }
}

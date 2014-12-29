using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace TrackMe.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ILocationService
    {
        [OperationContract]
        List<PhoneLocation> GetAll();
        [OperationContract]
        void SetLocation(string uid,double lat, double lng);
    }

    [DataContract]
    public class PhoneLocation
    {
        public PhoneLocation()
        {

        }

        public PhoneLocation(double lat, double lng, string uid)
        {
            Lat = lat;
            Lng = lng;
            Uid = uid;
        }
        [DataMember]
        public double Lat { get; set; }
        [DataMember]
        public double Lng { get; set; }
        [DataMember]
        public string Uid { get; set; }
    }
}

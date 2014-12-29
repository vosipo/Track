using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrackMe.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]// для каждого запуска единственный экземпляр сервиса, не в базе, а в памяти все храним 
    public class LocationService : ILocationService
    {
        public LocationService()
        {
            phones = new List<PhoneLocation>()
            {
               new PhoneLocation(-15.966688,32.580528, Guid.NewGuid().ToString()),
               //при создании сервиса эти две точки сразу вносятся
               new PhoneLocation(-15.766688,32.680528, Guid.NewGuid().ToString()) 
               //для того, чтобы продемонстрировать работу сервиса без использования устройств, передающих данные
            };
        }

        private List<PhoneLocation> phones;

        public List<PhoneLocation> GetAll()
        {
            return phones.ToList();//берется список телефонов
        }

        public void SetLocation(string uid,double lat, double lng)
        {
            var phone = phones.FirstOrDefault(x => x.Uid == uid);
            if (phone == null)// смотрим, что в базе нет такого значения
            {
                phone = new PhoneLocation(lat, lng, uid);// создаем новое устройство с широтой, долготой и уникальным номером
                phones.Add(phone);// и добавляем его в список
            }
            else
            {
                phone.Lat = lat;
                phone.Lng = lng;
            }
        }
    }
}

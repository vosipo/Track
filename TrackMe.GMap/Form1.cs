using System;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using TrackMe.GMap.ServiceReference1;

namespace TrackMe.GMap
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            gmap.MapProvider = GoogleMapProvider.Instance; //задаем провайдером ggoglemap
            GMaps.Instance.Mode = AccessMode.ServerAndCache;//здесь указываем, что можно кэшировать данные
            gmap.Position = new PointLatLng(-25.966688, 32.580528);//текущее местоположение
            gmap.MaxZoom = 16;//максимальный зум
            
            markersOverlay = new GMapOverlay("markers");// создаем слой с маркерами
            gmap.Overlays.Add(markersOverlay);//и этот слой добавляем в список слоев
            GetAll();//далее просто берем список всех наших точек
            InitTimer();
            
        }

        System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        public void InitTimer()
        {
            myTimer.Tick += myTimer_Tick;
            myTimer.Interval = 2000;//лучше секунды 2 и больше
            myTimer.Start();
        }

        void myTimer_Tick(object sender, EventArgs e)
        {
            RefreshMap();
        }

        
        async void GetAll()
        {
            var proxy = new LocationServiceClient();// создаем клиента к сервису WCF, класс LocationServiceClient генерируется 
                                                    // из метадаты, он описывает какие данные будут возвращаться

            var phones = await proxy.GetAllAsync(); //получаем список устройств
            for (var i = 0; i < phones.Count(); i++)
            {
                var phone = phones[i];
                AddMarker(phone.Uid, phone.Lat, phone.Lng); 
            }
        }

        private GMapOverlay markersOverlay; 

        void AddMarker(string uid, double lat, double lng)
        {
            var marker = new GMarkerGoogle(new PointLatLng(lat, lng), 
              GMarkerGoogleType.blue);  

            string name = string.Format("{0} - {1}:{2}", uid, lat, lng);
            marker.ToolTip = new GMapToolTip(marker);
            marker.ToolTipText = name;
            markersOverlay.Markers.Add(marker);
        }

        private void miRefresh_Click(object sender, EventArgs e)  
        {
            RefreshMap();
        }

        void RefreshMap()
        {
            markersOverlay.Markers.Clear();
            GetAll(); 
        }
    }
}

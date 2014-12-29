using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace TrackMe.Droid
{
	[Activity (Label = "TrackMe.Droid", MainLauncher = true, Icon = "@drawable/icon")]//
	public class MainActivity : Activity, ILocationListener//интерфейс listner слушает местоположение
	{
		Guid uid = Guid.NewGuid();
		Location _currentLocation;
		LocationManager _locationManager;
		TextView _locationText;
		String _locationProvider;

		LocationServiceClient _proxy;//создаем клиент к сервису WCF
		public static readonly EndpointAddress EndPoint = new EndpointAddress("http://localhost:39752/LocationService.svc");

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main); 	

			Button button = FindViewById<Button> (Resource.Id.get_location_button);//берем кнопку, которая у нас единственная
			button.Click += GetLocation;//на нажатие кнопки ставим обрпботчик GetLocation
			_locationText = FindViewById<TextView>(Resource.Id.location_text);//достаем текст, куда пишем текущее местоположение

			InitializeLocationManager();
			InitializeProxy();//инициализация WCF сервиса
		}



		private void InitializeProxy()
		{
			BasicHttpBinding binding = CreateBasicHttp();//метод, который создает соединение
			_proxy = new LocationServiceClient(binding, EndPoint);//
			_proxy.SetLocationCompleted+= HandleSetLocationCompleted;//подписка на события
		}

		void HandleSetLocationCompleted (object sender, System.ComponentModel.AsyncCompletedEventArgs e)//метод обработки события (ответ от сервера)
		{
			string msg = null;

			if (e.Error != null)
			{
				msg = e.Error.Message;//произошла ошибка в течение асинхронной операции
			}
			else if (e.Cancelled)
			{
				msg = "Request was cancelled.";// асинхронная операция была отменена 
			}

		}

		private static BasicHttpBinding CreateBasicHttp()//представляет собой привязку, которую служба WCF может и использовать для настройки и предоставления конечных точек
		{
			BasicHttpBinding binding = new BasicHttpBinding
			{
				Name = "basicHttpBinding",
				MaxBufferSize = 2147483647,
				MaxReceivedMessageSize = 2147483647
			};
			TimeSpan timeout = new TimeSpan(0, 0, 30);//если в течение 30 секунд нет ответа от сервера
			binding.SendTimeout = timeout;
			binding.OpenTimeout = timeout;
			binding.ReceiveTimeout = timeout;
			return binding;
		}

		void GetLocation(object sender, EventArgs eventArgs)
		{
			_currentLocation = _locationManager.GetLastKnownLocation (_locationProvider);//
			//mock if emu
			if (_currentLocation == null) {
				var rnd = new Random ();
				_currentLocation = new Location (_locationProvider) {

					Latitude = rnd.Next (-90, 90),
					Longitude = rnd.Next (-180, 180)
				};
			}
			UpdateLocation ();
		}

		void UpdateLocation()
		{
			if (_currentLocation == null)
			{
				_locationText.Text = "Unable to determine your location.";
			}
			else
			{

				var lat = _currentLocation.Latitude;
				var lng = _currentLocation.Longitude;
				_proxy.SetLocationAsync(uid.ToString(),lat,lng);//_proxy - наш WCF сервис, вызываем метод SetLocationAsync и отправляем текущее местоположение

				_locationText.Text = String.Format("{0},{1}",lat, lng);// текстовый файл с координатами 
			}
		}


		void InitializeLocationManager()
		{
			_locationManager = (LocationManager)GetSystemService(LocationService);//берется системный сервис LocationService 
			Criteria criteriaForLocationService = new Criteria//критерий точности расположения
			{
				Accuracy = Accuracy.Fine
			};
			IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);//список провайдеров, gps и другие
			if (acceptableLocationProviders.Any())
			{
				_locationProvider = acceptableLocationProviders.First();//если есть список провайдеров, то берем первый из них и используем его 
			}
			else
			{
				_locationProvider = String.Empty;//если ничего не найдено, то берем пустую строку
			}
		}

		public void OnLocationChanged(Location location)//если изменилось положение
		{
			_currentLocation = location;
			UpdateLocation ();
		}

		protected override void OnResume()
		{
			base.OnResume();
			_locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
		}

		protected override void OnPause()
		{
			base.OnPause();
			_locationManager.RemoveUpdates(this);
		}

		public void OnProviderDisabled (string provider)
		{
			throw new NotImplementedException ();//исключение, метод не реализован
		}

		public void OnProviderEnabled (string provider)//
		{
			throw new NotImplementedException ();
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)//
		{
			throw new NotImplementedException ();

		}


	}
}



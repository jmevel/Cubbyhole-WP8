using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Maps.Services;
using Windows.Devices.Geolocation;

namespace Cubbyhole.WP.Utils
{
    public delegate void PositionChangedEventHandler(object sender, MapLocation e);

    public class LocationManager
    {
        private Geolocator _geolocator { get; set; }

        public event PositionChangedEventHandler LocationRetrieved;

        public LocationManager()
        {
            _geolocator = new Geolocator();
        }
        public async void GetPhoneLocation()
        {
            try
            {
                _geolocator.DesiredAccuracyInMeters = 10;
                //_geolocator.DesiredAccuracy = PositionAccuracy.High;
                Geoposition geoposition = null;

                try
                {
                    geoposition = await _geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(10));
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                
                ReverseGeocodeQuery myReverseGeocodeQuery = new ReverseGeocodeQuery();
                myReverseGeocodeQuery.GeoCoordinate = new GeoCoordinate(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);
                IList<MapLocation> locations = await myReverseGeocodeQuery.GetMapLocationsAsync();
                if (LocationRetrieved != null)
                {
                    LocationRetrieved(this, locations.FirstOrDefault());
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

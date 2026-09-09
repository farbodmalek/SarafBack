using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Common.Maps
{
    public sealed class GeoCoordinates
    {
        private static readonly GeoCoordinates instance = null;
        private GeoCoordinates() { }
        public static GeoCoordinates Instance
        {
            get
            {
                if (instance == null)
                    return new GeoCoordinates();
                return instance;
            }
        }
        public double ComputeDistance(string startLatitude, string startLongitude, string endLatitude, string endLongitude, char unit = 'K')
        {
           return ComputeDistance(double.Parse(startLatitude), double.Parse(startLongitude), double.Parse(endLatitude), double.Parse(endLongitude), unit);
        }

        public double ComputeDistance(double startLatitude, double startLongitude, double endLatitude, double endLongitude, char unit='K')
        {
            if ((startLatitude == endLatitude) && (startLongitude == endLongitude))
            {
                return 0;
            }
            else
            {
                double theta = startLongitude - endLongitude;
                double dist = Math.Sin(deg2rad(startLatitude)) * Math.Sin(deg2rad(endLatitude)) + Math.Cos(deg2rad(startLatitude)) * Math.Cos(deg2rad(endLatitude)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }
                return (dist);
            }
        }
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Common
{
    public class DistanceHelper
    {
        /// <summary>
        /// 地球半径(单位：米)
        /// </summary>
        public const double EarthRadius = 6378137.0;
        /// <summary>
        /// 
        /// </summary>
        private const double PI = 3.141592653589793;
        private static double getRad(double lat)
        {
            return (double)lat * PI / 180.0;
        }
        /// <summary>
        /// 计算地球两点距离(单位:米)
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lng1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static double GetGreatCircleDistance(double lat1, double lng1, double lat2, double lng2)
        {
            var radLat1 = getRad(lat1);
            var radLat2 = getRad(lat2);
            var a = radLat1 - radLat2;
            var b = getRad(lng1) - getRad(lng2);

            var s = 2 * Math.Asin(Math.Sqrt((Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))));
            return Math.Round(s * EarthRadius * 10000) / 10000.0;
        }
    }
}

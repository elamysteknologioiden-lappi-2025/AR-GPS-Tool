/******************************************************************************
* File         : pLab_GeoTools.cs
* Lisence      : BSD 3-Clause License
* Copyright    : Lapland University of Applied Sciences
* Authors      : Arto Söderström
* BSD 3-Clause License
*
* Copyright (c) 2019, Lapland University of Applied Sciences
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
* 
* 1. Redistributions of source code must retain the above copyright notice, this
*  list of conditions and the following disclaimer.
*
* 2. Redistributions in binary form must reproduce the above copyright notice,
*  this list of conditions and the following disclaimer in the documentation
*  and/or other materials provided with the distribution.
*
* 3. Neither the name of the copyright holder nor the names of its
*  contributors may be used to endorse or promote products derived from
*  this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
* FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
* DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
* SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
* CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
* OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
* OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class pLab_GeoTools {

    const double R = 6371000f; // metres

    public static float DistanceBetweenPoints(pLab_LatLon startPoint, pLab_LatLon endPoint) {
        return DistanceBetweenPoints(startPoint.Lat, startPoint.Lon, endPoint.Lat, endPoint.Lon);
    }

    public static float DistanceBetweenPoints(double startPointLat, double startPointLon, double endPointLat, double endPointLon) {
        double startLatRad = pLab_MathConversions.DegToRad(startPointLat);
        double endLatRad = pLab_MathConversions.DegToRad(endPointLat);

        double deltaLatRad = pLab_MathConversions.DegToRad(endPointLat - startPointLat);
        double deltaLonRad = pLab_MathConversions.DegToRad(endPointLon - startPointLon);

        double a = System.Math.Sin(deltaLatRad/2) * System.Math.Sin(deltaLatRad/2) +
                    System.Math.Cos(startLatRad) * System.Math.Cos(endLatRad) *
                    System.Math.Sin(deltaLonRad/2) * System.Math.Sin(deltaLonRad/2);

        double c = 2 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1-a));
        //OR
        //double c = 2 * Mathf.Asin(Mathf.Sqrt(a));

        float d = (float) (R * c);

        return d;
    }

    public static float DistanceBetweenPointsPythagoras(pLab_LatLon startPoint, pLab_LatLon endPoint) {
        return DistanceBetweenPointsPythagoras(startPoint.Lat, startPoint.Lon, endPoint.Lat, endPoint.Lon);
    }

    public static float DistanceBetweenPointsPythagoras(double startPointLat, double startPointLon, double endPointLat, double endPointLon) {
        double x = pLab_MathConversions.DegToRad(endPointLon - startPointLon) * System.Math.Cos(pLab_MathConversions.DegToRad(startPointLat + endPointLat)/2);
        double y = pLab_MathConversions.DegToRad(endPointLat - startPointLat);
        double d = Math.Sqrt(x*x + y*y) * R;
        return (float) d;
    }

    public static float BearingFromPointAToBInDegrees(pLab_LatLon startPoint, pLab_LatLon endPoint) {
        return BearingFromPointAToBInDegrees(startPoint.Lat, startPoint.Lon, endPoint.Lat, endPoint.Lon);
    }

    public static float BearingFromPointAToBInDegrees(double startPointLat, double startPointLon, double endPointLat, double endPointLon) {
        float bearingInRad = BearingFromPointAToB(startPointLat, startPointLon, endPointLat, endPointLon);
        return Mathf.Rad2Deg * bearingInRad;
    }

    public static float BearingFromPointAToB(pLab_LatLon startPoint, pLab_LatLon endPoint) {

        return BearingFromPointAToB(startPoint.Lat, startPoint.Lon, endPoint.Lat, endPoint.Lon);
    }

    // public static double BearingFromPointAToBDouble(LatLon startPoint, LatLon endPoint) {

    // }

    /// <summary>
    /// θ = atan2( sin Δλ ⋅ cos φ2 , cos φ1 ⋅ sin φ2 − sin φ1 ⋅ cos φ2 ⋅ cos Δλ )
    /// </summary>
    /// <source>https://www.movable-type.co.uk/scripts/latlong.html</source>
    /// <param name="startPointLat"></param>
    /// <param name="startPointLon"></param>
    /// <param name="endPointLat"></param>
    /// <param name="endPointLon"></param>
    /// <returns></returns>
    public static float BearingFromPointAToB(double startPointLat, double startPointLon, double endPointLat, double endPointLon) {
        double startPointLatRad = pLab_MathConversions.DegToRad(startPointLat);
        double startPointLonRad = pLab_MathConversions.DegToRad(startPointLon);
        double endPointLatRad = pLab_MathConversions.DegToRad(endPointLat);
        double endPointLonRad = pLab_MathConversions.DegToRad(endPointLon);

        double deltaLonRad = endPointLonRad - startPointLonRad;

        //sin( ) cos( )
        double y = System.Math.Sin(deltaLonRad) * System.Math.Cos(endPointLatRad);
        double x = System.Math.Cos(startPointLatRad) * System.Math.Sin(endPointLatRad) - System.Math.Sin(startPointLatRad) * System.Math.Cos(endPointLatRad) * System.Math.Cos(deltaLonRad);
                
        // float bearing = (float) RadToDeg(System.Math.Atan2(y, x));
        float bearing = (float) (System.Math.Atan2(y, x));
        
        return bearing;
    }

    /// <summary>
    ///
    /// Convert lat/long to UTM coords. Equations from USGS Bulletin 1532
    /// 
    /// East Longitudes are positive, West longitudes are negative.
    /// North latitudes are positive, South latitudes are negative
    /// at and Long are in fractional degrees
    /// 
    /// written by Chuck Gantz- chuck.gantz@globalstar.com
    /// 
    /// </summary>
    /// <param name="Lat"></param>
    /// <param name="Long"></param>
    /// <param name="UTMNorthing"></param>
    /// <param name="UTMEasting"></param>
    public static void LatLongtoUTM(double Lat, double Long, out double UTMNorthing, out double UTMEasting) {
        double a = 6378137; 
        double eccSquared = 0.00669438;
        double k0 = 0.9996;

        double LongOrigin;
        double eccPrimeSquared;
        double N, T, C, A, M;

        double LongTemp = (Long + 180) - ((int)((Long + 180) / 360)) * 360 - 180;

        double LatRad = Lat * Mathf.Deg2Rad;
        double LongRad = LongTemp * Mathf.Deg2Rad;
        double LongOriginRad;
        int ZoneNumber;
        ZoneNumber = ((int)((LongTemp + 180) / 6)) + 1;
        if (Lat >= 56.0 && Lat < 64.0 && LongTemp >= 3.0 && LongTemp < 12.0)
            ZoneNumber = 32;
        if (Lat >= 72.0 && Lat < 84.0)
        {
            if (LongTemp >= 0.0 && LongTemp < 9.0) ZoneNumber = 31;
            else if (LongTemp >= 9.0 && LongTemp < 21.0) ZoneNumber = 33;
            else if (LongTemp >= 21.0 && LongTemp < 33.0) ZoneNumber = 35;
            else if (LongTemp >= 33.0 && LongTemp < 42.0) ZoneNumber = 37;
        }
        LongOrigin = (ZoneNumber - 1) * 6 - 180 + 3;
        LongOriginRad = LongOrigin * Mathf.Deg2Rad;
        eccPrimeSquared = (eccSquared) / (1 - eccSquared);

        N = a / Math.Sqrt(1 - eccSquared * Math.Sin(LatRad) * Math.Sin(LatRad));
        T = Math.Tan(LatRad) * Math.Tan(LatRad);
        C = eccPrimeSquared * Math.Cos(LatRad) * Math.Cos(LatRad);
        A = Math.Cos(LatRad) * (LongRad - LongOriginRad);

        M = a * ((1 - eccSquared / 4 - 3 * eccSquared * eccSquared / 64 - 5 * eccSquared * eccSquared * eccSquared / 256) * LatRad
        - (3 * eccSquared / 8 + 3 * eccSquared * eccSquared / 32 + 45 * eccSquared * eccSquared * eccSquared / 1024) * Math.Sin(2 * LatRad)
        + (15 * eccSquared * eccSquared / 256 + 45 * eccSquared * eccSquared * eccSquared / 1024) * Math.Sin(4 * LatRad)
        - (35 * eccSquared * eccSquared * eccSquared / 3072) * Math.Sin(6 * LatRad));

        UTMEasting = (double)(k0 * N * (A + (1 - T + C) * A * A * A / 6
        + (5 - 18 * T + T * T + 72 * C - 58 * eccPrimeSquared) * A * A * A * A * A / 120)
       + 500000.0);

        UTMNorthing = (double)(k0 * (M + N * Math.Tan(LatRad) * (A * A / 2 + (5 - T + 9 * C + 4 * C * C) * A * A * A * A / 24
        + (61 - 58 * T + T * T + 600 * C - 330 * eccPrimeSquared) * A * A * A * A * A * A / 720)));
        if (Lat < 0)
            UTMNorthing += 10000000.0; 
    }

    /// <summary>
    /// Get the "distance" between two points using UTM
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    public static Vector2 UTMDifferenceBetweenPoints(pLab_LatLon startPoint, pLab_LatLon endPoint) {
        double UTMNorthingCurrent = 0;
        double UTMEastingCurrent = 0;
        double UTMNorthingPOI = 0;
        double UTMEastingPOI = 0;

        pLab_GeoTools.LatLongtoUTM(startPoint.Lat, startPoint.Lon, out UTMNorthingCurrent, out UTMEastingCurrent);
        pLab_GeoTools.LatLongtoUTM(endPoint.Lat, endPoint.Lon, out UTMNorthingPOI, out UTMEastingPOI);

        return new Vector2((float) (UTMEastingPOI - UTMEastingCurrent), (float) (UTMNorthingPOI - UTMNorthingCurrent));

    }


}

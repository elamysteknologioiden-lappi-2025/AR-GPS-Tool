/******************************************************************************
 * File         : pLab_DebugLocationAndHeadingProvider.cs            
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
 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class pLab_DebugLocationAndHeadingProvider : MonoBehaviour
{
    [SerializeField]
    private Text headingText;

    [SerializeField]
    private Text locationText;

    [SerializeField]
    private Text locationAccuracyText;

    [SerializeField]
    private Text locationDistanceChangeText;

    [SerializeField]
    private Text gpsHeadingText;

    [SerializeField]
    private Text arHeadingText;

    [SerializeField]
    [FormerlySerializedAs("gpsArHeadingDifferenceText")]
    private Text headingDifferenceText;

    [SerializeField]
    private pLab_LocationProvider locationProvider;

    [SerializeField]
    private pLab_HeadingProvider headingProvider;

    [SerializeField]
    private pLab_ARTrueNorthFinder arTrueNorthFinder;

    private Color locationTextDefaultColor;

    private pLab_LatLon previousLocation;

    private void OnEnable() {
        if (locationProvider != null) {
            locationProvider.OnLocationUpdated += OnLocationUpdated;
        }

        if (headingProvider != null) {
            headingProvider.OnHeadingUpdated += OnHeadingUpdated;
        }

        if (arTrueNorthFinder != null) {
            arTrueNorthFinder.OnHeadingUpdated += OnTrueNorthHeadingUpdated;
            arTrueNorthFinder.OnHeadingFromMovementUpdated += OnMovementFromHeadingUpdated;

        }
    }

    private void OnDisable() {
        if (locationProvider != null) {
            locationProvider.OnLocationUpdated -= OnLocationUpdated;
        }

        if (headingProvider != null) {
            headingProvider.OnHeadingUpdated -= OnHeadingUpdated;
        }
        
        if (arTrueNorthFinder != null) {
            arTrueNorthFinder.OnHeadingUpdated -= OnTrueNorthHeadingUpdated;
            arTrueNorthFinder.OnHeadingFromMovementUpdated -= OnMovementFromHeadingUpdated;
        }
    }

    private void OnTrueNorthHeadingUpdated(object sender, pLab_NorthHeadingUpdatedEventArgs e)
    {
        if (headingDifferenceText != null) {
            headingDifferenceText.text = string.Format("Comp-Cam Dif {0} deg", e.heading);
        }
    }

    private void OnMovementFromHeadingUpdated(object sender, pLab_HeadingFromMovementUpdatedEventArgs e)
    {
        if (arHeadingText != null) {
            arHeadingText.text = string.Format("AR Heading: {0} deg", e.arHeading);
        }

        if (gpsHeadingText != null) {
            gpsHeadingText.text = string.Format("GPS Heading: {0} deg", e.gpsHeading);
        }

        if (headingDifferenceText != null) {
            headingDifferenceText.text = string.Format("GPS-AR {0} / {1} / {2} deg", e.headingDifference, e.medianHeadingDifference, e.averageHeadingDifference);
        }
    }

    private void OnLocationUpdated(object sender, pLab_LocationUpdatedEventArgs e)
    {
        if (locationText != null) {
            locationText.text = string.Format("Loc: {0},{1}", e.location.Lat, e.location.Lon);
            locationText.color = Color.red;
            StartCoroutine(LocationTextToDefault());
        }

        if (locationAccuracyText != null) {
            locationAccuracyText.text = string.Format("Loc. acc H:{0} V:{0}", e.horizontalAccuracy, e.verticalAccuracy);
        }

        if (locationDistanceChangeText != null) {
            float locationChange = 0f;

            if (previousLocation != null) {
                locationChange = pLab_GeoTools.DistanceBetweenPoints(previousLocation, e.location);
            } else {
                previousLocation = new pLab_LatLon();
            }

            locationDistanceChangeText.text = string.Format("Loc. distance change: {0}m", locationChange);
            previousLocation.Lat = e.location.Lat;
            previousLocation.Lon = e.location.Lon;
        }
    }

    private void Start() {
        if (locationText != null) {
            locationText.text = "No location data...";
            locationTextDefaultColor = locationText.color;
        }
    }

    private void OnHeadingUpdated(object sender, pLab_HeadingUpdatedEventArgs e)
    {
        if (headingText != null) {
            headingText.text = string.Format("C: {0} / {1} / {2}", (int) e.heading ,(int) e.filteredHeading, e.filteredHeadingVelocity.ToString("F1"));
        }
    }

    private IEnumerator LocationTextToDefault() {
        yield return new WaitForSeconds(1f);
        if (locationText != null) {
            locationText.color = locationTextDefaultColor;
        }
    }
}

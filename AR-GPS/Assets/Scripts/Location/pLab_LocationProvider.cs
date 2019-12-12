/******************************************************************************
* File         : pLab_LocationProvider.cs
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
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

/// <summary>
/// Location updated event arguments. 
/// </summary>
public class pLab_LocationUpdatedEventArgs : EventArgs
{
    public pLab_LatLon location;
    public float altitude;
    public float horizontalAccuracy;
    public float verticalAccuracy;
    public double timestamp;
}

public class pLab_LocationProvider : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Using higher value like 500 usually does not require to turn GPS chip on and thus saves battery power. 
    /// Values like 5-10 could be used for getting best accuracy.
    /// </summary>
    [SerializeField]
    [Range(0.2f, 10)]
    private float desiredAccuracyInMeters = 1f;

    /// <summary>
    /// The minimum distance (measured in meters) a device must move laterally before Input.location property is updated. 
    /// Higher values like 500 imply less overhead.
    /// </summary>
    [SerializeField]
    [Range(0.2f, 30)]
    private float updateDistanceInMeters = 1f;

    Coroutine pollRoutine;

    
    private double lastLocationTimestamp;

    private WaitForSeconds wait;

    private pLab_LatLon location;

    private pLab_LatLon latestAccurateLocation;

    private LocationInfo latestLocationInfo;


    #region Debug Variables
    [Header("Debug")]
    [SerializeField]
    private bool useFakeData = false;

    [SerializeField]
    private pLab_LatLon fakeCoordinates;

    #endregion

    #endregion

    #region Properties

    public pLab_LatLon Location { get => location; set => location = value; }

    public double LastLocationTimestamp { get { return lastLocationTimestamp; } }

    public LocationInfo LatestLocationInfo { get { return latestLocationInfo; } }
    

    #region Debug Properties

    public bool UseFakeData {
        get { return useFakeData; }
        set
        {
            bool previousVal = useFakeData;
            useFakeData = value;
            if (previousVal != useFakeData) {
                StartPollLocationRoutine();
            }
        }
    }
    
    #endregion Debug Properties

    #endregion

    #region Events

    /// <summary>
    /// Occurs when on location updates.
    /// </summary>
    public event EventHandler<pLab_LocationUpdatedEventArgs> OnLocationUpdated;

    #endregion

    #region Inherited Methods

    // Start is called before the first frame update
    void Start()
    {
        wait = new WaitForSeconds(1f);

        StartPollLocationRoutine();

    }

    #endregion

    #region IEnumerators/Coroutines
    
    /// <summary>
    /// Enable location and compass services.
    /// Sends continuous location and heading updates based on 
    /// _desiredAccuracyInMeters and _updateDistanceInMeters.
    /// </summary>
    /// <returns>The location routine.</returns>
    private IEnumerator PollLocationRoutine()
    {
        #if PLATFORM_ANDROID
            
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }

        #endif

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location not enabled by user");
            yield break;
        }

        Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);
        Input.compass.enabled = true;

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return wait;
            maxWait--;
        }

        if (maxWait < 1)
        {
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            yield break;
        }

        while (true)
        {
            double timestamp = Input.location.lastData.timestamp;

            if (Input.location.status == LocationServiceStatus.Running && timestamp > lastLocationTimestamp)
            {
                lastLocationTimestamp = timestamp;

                LocationInfo locationInfo = Input.location.lastData;
                latestLocationInfo = locationInfo;

                location = new pLab_LatLon(locationInfo.latitude, locationInfo.longitude);

                if (locationInfo.horizontalAccuracy < 5f) {
                    latestAccurateLocation = location;
                }

                SendUpdatedLocation();
            }

            yield return null;
        }
    }

    private IEnumerator PollLocationRoutineFake() {
        yield return new WaitForSeconds(1f);

        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        while(true) {
            
            double timestamp = (System.DateTime.UtcNow - epochStart).TotalMilliseconds;

            location = fakeCoordinates;
            latestLocationInfo = new LocationInfo();
            pLab_LocationUpdatedEventArgs locationEventArgs = new pLab_LocationUpdatedEventArgs()
            {
                location = location,
                altitude = 1f,
                horizontalAccuracy = UnityEngine.Random.Range(3.21f, 20f),
                verticalAccuracy = UnityEngine.Random.Range(3.21f, 20f),
                timestamp = timestamp
            };

            if (OnLocationUpdated != null) {
                OnLocationUpdated(this, locationEventArgs);
            }

            yield return new WaitForSeconds(2f);
        }
        
    }

    #endregion

    #region Private Methods

    private void StartPollLocationRoutine() {
        if (pollRoutine != null) {
            StopCoroutine(pollRoutine);
        }

        if (useFakeData) {
            pollRoutine = StartCoroutine(PollLocationRoutineFake());
        } else {
            pollRoutine = StartCoroutine(PollLocationRoutine());
        }
    }

    private void SendUpdatedLocation()
    {
        if (OnLocationUpdated != null)
        {
            pLab_LocationUpdatedEventArgs eventArgs = new pLab_LocationUpdatedEventArgs()
            {
                location = location,
                altitude = latestLocationInfo.altitude,
                horizontalAccuracy = latestLocationInfo.horizontalAccuracy,
                verticalAccuracy = latestLocationInfo.verticalAccuracy,
                timestamp = latestLocationInfo.timestamp
            };


            OnLocationUpdated(this, eventArgs);
        }
    }

    #endregion

}

/******************************************************************************
* File         : pLab_LocationScreenshotRecorder.cs
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
using System.IO;

/// <summary>
/// Record locations with screenshot, GPS-coordinates, filtered compass heading
/// </summary>
public class pLab_LocationScreenshotRecorder : MonoBehaviour
{
    #region Inner Classes and enums
    [System.Serializable]
    public class GPSDataRecorderSummary {
        //All the GPS locations
        public List<RecordData> recordList;
    }

    [System.Serializable]
    public class RecordData {
        public double latitude;
        public double longitude;
        public double timestamp;
        public float horizontalAccuracy;
        public float verticalAccuracy;
        public float compassHeading;
        public int index;
        public string screenshotPath;
    }

    public enum RecordingState {
        Stopped = 0,
        Recording = 1,
        Saving = 2
    }

    #endregion

    #region Variables

    [SerializeField]
    private pLab_LocationProvider locationProvider;

    [SerializeField]
    private pLab_HeadingProvider headingProvider;

    [Header("Save settings")]
    [SerializeField]
    private string fileName = "location_screenshot_data_";

    private double lastLocationTimestamp;

    private List<RecordData> recordList = new List<RecordData>();

    private int index = 1;

    private RecordingState state = RecordingState.Stopped;
    
    #if UNITY_EDITOR
    private System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
    #endif
    
    #endregion

    #region Properties

    public RecordingState State { get { return state; } }

    #endregion

    private void Start() {
        Reset();
    }

    public void Reset() {
        recordList = new List<RecordData>();
        index = 1;
        state = RecordingState.Stopped;
    }

    public void SaveRecording() {
        state = RecordingState.Saving;
        GPSDataRecorderSummary gpsDataRecorderSummary = new GPSDataRecorderSummary();
        gpsDataRecorderSummary.recordList = recordList;

        string json = JsonUtility.ToJson(gpsDataRecorderSummary);

        string dateTimeString = System.DateTime.UtcNow.ToString("yyyy-M-dd--HH-mm");
        string filePath = Application.persistentDataPath + "/" + fileName + "_" + dateTimeString + ".json";

        File.WriteAllText(filePath, json);
        state = RecordingState.Stopped;
    }

    public void RecordNewData()
    {
        state = RecordingState.Recording;

        string dateTimeString = System.DateTime.UtcNow.ToString("yyyy-M-dd--HH-mm-ss");
        string imageFilename = index + "_LocationScreenshot_" + dateTimeString + ".png";
        ScreenCapture.CaptureScreenshot(imageFilename);
        string screenshotPath = Application.persistentDataPath + "/" + imageFilename;

        double timestamp = locationProvider.LastLocationTimestamp;
        LocationInfo locInfo = locationProvider.LatestLocationInfo;

        RecordData recordData = new RecordData()
        {
            screenshotPath = screenshotPath,
            latitude = locInfo.latitude,
            longitude = locInfo.longitude,
            horizontalAccuracy = locInfo.horizontalAccuracy,
            verticalAccuracy = locInfo.verticalAccuracy,
            compassHeading = headingProvider.FilteredHeading,
            timestamp = timestamp,
            index = index
        };

        recordList.Add(recordData);
        index++;
    }
}

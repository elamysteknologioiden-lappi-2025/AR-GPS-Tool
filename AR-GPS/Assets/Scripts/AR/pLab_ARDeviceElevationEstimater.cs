/******************************************************************************
* File         : pLab_ARDeviceElevationEstimater.cs
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
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class pLab_ARDeviceElevationEstimater : MonoBehaviour
{

    #region Variables

    [SerializeField]
    private Transform arCameraTransform;

    [SerializeField]
    private ARPlaneManager arPlaneManager;

    [SerializeField]
    private ARRaycastManager arRaycastManager;

    [SerializeField]
    private float minAcceptedReading = 0.4f;

    [SerializeField]
    private float maxAcceptedReading = 2.3f;

    [SerializeField]
    private float defaultElevationEstimate = 1.35f;

    [SerializeField]
    private float calculateInterval = 0.1f;

    private float timer = 0;

    private Ray ray = new Ray();

    private List<ARRaycastHit> hitList = new List<ARRaycastHit>();

    private float deviceElevationEstimate;

    private float groundLevelEstimate = 0;

    #region Variables: Debug

    [SerializeField]
    private Text debugText;

    #endregion
    
    #endregion

    #region Properties

    public float DeviceElevationEstimate { get { return deviceElevationEstimate; } }
    
    public float GroundLevelEstimate { get { return groundLevelEstimate; } }
    
    #endregion

    #region Inherited Methods

    private void Awake() {
        deviceElevationEstimate = defaultElevationEstimate;
    }

    public void Update() {
        timer += Time.deltaTime;
        
        if (timer >= calculateInterval) {
            CalculateHeight();
            if (debugText != null) {
                debugText.text = "Dev H: " + deviceElevationEstimate.ToString("F2") + ", GL Y: " + groundLevelEstimate.ToString("F1");
            }
            timer = 0;
        }
    }

    #if UNITY_EDITOR
    private void Reset() {
        if (arCameraTransform == null) {
            Camera camera = this.GetComponentInChildren<Camera>();

            if (camera != null) {
                arCameraTransform = camera.gameObject.transform;
            }
        }

        if (arPlaneManager == null) {
            ARPlaneManager planeManager = this.GetComponentInChildren<ARPlaneManager>();

            if (planeManager != null) {
                arPlaneManager = planeManager;
            } else {
                planeManager = FindObjectOfType<ARPlaneManager>();
                
                if (planeManager != null) {
                    arPlaneManager = planeManager;
                }
            }
        }

        if (arRaycastManager == null) {
            ARRaycastManager raycastManager = this.GetComponentInChildren<ARRaycastManager>();

            if (raycastManager != null) {
                arRaycastManager = raycastManager;
            } else {
                raycastManager = FindObjectOfType<ARRaycastManager>();
                
                if (raycastManager != null) {
                    arRaycastManager = raycastManager;
                }
            }
        }

    }
    #endif


    #endregion

    #region Event Handlers

    #endregion

    #region Private Methods

    /// <summary>
    /// Calculate new device heist estimate, and ground level y estimate
    /// </summary>
    private void CalculateHeight() {
        ray.origin = arCameraTransform.position;
        ray.direction = Vector3.down;

        arRaycastManager.Raycast(ray, hitList, TrackableType.Planes);

        if (hitList.Count > 0) {

            //We assume the last plane is the floor
            ARRaycastHit hit = hitList[hitList.Count - 1];
            float rayDistance = hit.distance;

            if (rayDistance >= minAcceptedReading && rayDistance <= maxAcceptedReading) {
                deviceElevationEstimate = rayDistance;
                groundLevelEstimate = hit.pose.position.y;
            }
        }
    }

    #endregion
}

/******************************************************************************
* File         : pLab_ARTrueNorthFinder.cs
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
using UnityEngine.XR.ARFoundation;
using System.Linq;

/// <summary>
/// Location updated event arguments. 
/// </summary>
public class pLab_NorthHeadingUpdatedEventArgs : EventArgs
{
    public float heading;
    public double timestamp;
    public bool isPriority;
}

/// <summary>
/// AR True North Calculation mode changed event args
/// </summary>
public class pLab_CalculationModeChangedEventArgs : EventArgs
{
    public pLab_ARTrueNorthFinder.CalculationMode calculationMode;
}

/// <summary>
/// AR True North Calculation phase changed event args
/// </summary>
public class pLab_CalculationPhaseChangedEventArgs : EventArgs
{
    public pLab_ARTrueNorthFinder.CalculationPhase calculationPhase;
}

public class pLab_HeadingFromMovementUpdatedEventArgs : EventArgs
{
    public float arHeading;
    public float gpsHeading;
    public float headingDifference;
    public float medianHeadingDifference;
    public float averageHeadingDifference;
}

public class pLab_ARTrueNorthFinder : MonoBehaviour
{
    #region Inner Classes and Enums
    /// <summary>
    /// Calculation mode to be used when calculation true north
    /// </summary>
    public enum CalculationMode {
        /// <summary>
        /// Only use the compass
        /// </summary>
        Compass = 0,
        /// <summary>
        /// Use the GPS-differences and AR-movement differences. Compass is still used for the initial FastInterval-phase!
        /// </summary>
        GPSAndARMovement = 1,
        /// <summary>
        /// Use both the Compass and GPSAndARMovement modes
        /// </summary>
        Both = 2,
        /// <summary>
        /// No automatic mode is used, calculation is done manually by calling the offset functions
        /// </summary>
        Manual = 3
    }
    
    /// <summary>
    /// In which state is the calculation. Used for Compass-mode
    /// </summary>
    public enum CalculationPhase {
        Stopped = 0,
        /// <summary>
        /// Low intervals to get the original heading fast
        /// </summary>
        FastInterval = 1,
        /// <summary>
        /// Higher interval to modify original heading
        /// </summary>
        SlowInterval = 2
    }

    #endregion

    #region Variables

    [SerializeField]
    private bool isEnabled = false;

    [SerializeField]
    private bool resetAfterTrackingLost = true;

    private bool isSessionTracking = false;

    [SerializeField]
    private Camera arCamera;

    [Tooltip("How true north is calculated.")]
    [SerializeField]
    private CalculationMode calculationMode = CalculationMode.Compass;

    private Transform arCameraTransform;

    [SerializeField]
    [Tooltip("How much manual offset moves heading")]
    private float manualOffsetIncremental = 2f;

    [Header("GPS-AR Mode")]
    [SerializeField]
    private pLab_LocationProvider locationProvider;

    [SerializeField]
    private float minDistanceBeforeUpdate = 10f;

    [SerializeField]
    private float movementMinimumGPSAccuracy = 10f;

    [SerializeField]
    private int maxLastMovementHeadings = 5;

    private bool hasGPSARHeading = false;

    [Header("Compass Mode")]
    [SerializeField]
    private pLab_HeadingProvider headingProvider;

    /// <summary>
    /// How much time between recordings/readings
    /// </summary>
    private float updateIntervalCompass = 0f;

    /// <summary>
    /// Initial time between compass-mode readings
    /// </summary>
    [SerializeField]
    private float updateIntervalCompassInitial = 0.25f;

    /// <summary>
    /// How much time between recordings/readings when the first phase (half of max size) has been reached
    /// </summary>
    [SerializeField]
    private float updateIntervalCompassDelayed = 3f;

    /// <summary>
    /// Max size of readings before we start removing old values
    /// </summary>
    [SerializeField]
    private int maxCompassModeSampleSize = 50;

    private bool hasCompassHeading = false;

    private CalculationPhase calculationPhase = CalculationPhase.Stopped;

    private float heading = 0;

    private float compassModeHeading = 0;

    private double headingTimestamp;

    private Vector3 previousARCameraPosition = Vector3.zero;

    private pLab_LatLon previousGPSCoordinates;

    private float previousGPSAccuracy = 9999f;

    private List<float> lastGPSARHeadingDeltas = new List<float>();

    private float initialGPSARDifference = 0;

    private float arHeading;
    private float gpsHeading;
    private float medianGPSARHeadingDifference;
    private float averageGPSARHeadingDifference;
    private float distance;

    private float updateTimerCompass = 0;

    private float initialReading = -1f;

    private List<float> readingDeltas = new List<float>();

    private float manualOffset = 0f;

    #region Debug Variables

    [Header("Debug")]
    [SerializeField]
    private GameObject headingObjectToRotate;

    [SerializeField]
    private GameObject GPSARHeadingObjectToRotate;

    [SerializeField]
    private GameObject compassHeadingObjectToRotate;

    #endregion Debug Variables

    #endregion Variables
    #region Properties

    public bool IsEnabled {
        get { return isEnabled; }
        set {
            bool previousVal = isEnabled;
            isEnabled = value;

            if (isEnabled != previousVal && !isEnabled) {
                ResetBothModes();
            }
            
        }
    }

    public bool FastIntervalPhaseSizeReached { get { return readingDeltas.Count >= (maxCompassModeSampleSize / 2); } }
    public CalculationMode CalculationModeProp {
        get { return calculationMode; }

        set
        {

            CalculationMode prevMode = calculationMode;
            calculationMode = value;

            if (prevMode != calculationMode) {
                if (OnCalculationModeChanged != null) {
                    pLab_CalculationModeChangedEventArgs args = new pLab_CalculationModeChangedEventArgs() {
                        calculationMode = calculationMode
                    };
                    
                    OnCalculationModeChanged(this, args);
                }
            }
        }
    }
    public bool HeadingSampleSizeReached { get { return readingDeltas.Count >= maxCompassModeSampleSize; } }
    public float Heading { get { return heading; } }
    public float ARHeading { get { return arHeading; } }
    public float GPSHeading { get { return gpsHeading; } }

    public CalculationPhase CalculationPhaseProp { 
        get { return calculationPhase; }

        private set {
            CalculationPhase prevPhase = calculationPhase;

            calculationPhase = value;

            switch(calculationPhase) {
                //For Stopped-phase it doesn't really even matter which one is it
                case CalculationPhase.Stopped:
                case CalculationPhase.FastInterval:
                    updateIntervalCompass = updateIntervalCompassInitial;
                    break;
                case CalculationPhase.SlowInterval:
                    updateIntervalCompass = updateIntervalCompassDelayed;
                    break;
            }

            //If the phase actually was changed
            if (calculationPhase != prevPhase) {
                if (OnCalculationPhaseChanged != null) {
                    pLab_CalculationPhaseChangedEventArgs args = new pLab_CalculationPhaseChangedEventArgs() {
                        calculationPhase = calculationPhase
                    };

                    OnCalculationPhaseChanged(this, args);
                }
            }
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Triggered when CalculationMode has been changed
    /// </summary>
    public event EventHandler<pLab_CalculationModeChangedEventArgs> OnCalculationModeChanged;

    /// <summary>
    /// Triggered when CalculationPhase has been changed
    /// </summary>
    public event EventHandler<pLab_CalculationPhaseChangedEventArgs> OnCalculationPhaseChanged;

    /// <summary>
    /// Triggered when new true north has been calculated with Compass-mode
    /// </summary>
    public event EventHandler<pLab_NorthHeadingUpdatedEventArgs> OnHeadingUpdated;

    /// <summary>
    /// Triggered when new true north has been calculated with GPS-AR -mode
    /// </summary>
    public event EventHandler<pLab_HeadingFromMovementUpdatedEventArgs> OnHeadingFromMovementUpdated;

    #endregion

    #region Inherited Methods

    private void Awake() {
        CalculationPhaseProp = CalculationPhase.Stopped;
        updateIntervalCompass = updateIntervalCompassInitial;
        if (arCamera != null) {
            arCameraTransform = arCamera.transform;
        }
    }

    private void OnEnable() {

        if (locationProvider != null) {
            locationProvider.OnLocationUpdated += OnLocationUpdated;
        }

        isSessionTracking = ARSession.state == ARSessionState.SessionTracking;
        ResetBothModes();
        ARSession.stateChanged += OnARSessionStateChange;

    }

    private void OnDisable() {
        if (locationProvider != null) {
            locationProvider.OnLocationUpdated -= OnLocationUpdated;
        }

        ARSession.stateChanged -= OnARSessionStateChange;
    }

    private void Update() {
        if (isEnabled) {
            //The initial fast interval calculation is used for the initial heading even though CalculationMode is GPSAR
            if (calculationMode == CalculationMode.Compass || calculationMode == CalculationMode.Both || (calculationPhase == CalculationPhase.FastInterval && calculationMode != CalculationMode.Manual)) {
                
                if (calculationPhase == CalculationPhase.Stopped) {
                    CalculationPhaseProp = CalculationPhase.FastInterval;
                }

                updateTimerCompass += Time.deltaTime;

                if (updateTimerCompass >= updateIntervalCompass) {

                    //If RecordHeading returns true -> new record was recorded
                    if(RecordCompassHeading()) {
                        updateTimerCompass = 0;

                        if (CalculationPhaseProp == CalculationPhase.FastInterval && FastIntervalPhaseSizeReached) {
                            CalculationPhaseProp = CalculationPhase.SlowInterval;
                            RecalculateMedianCompassHeading();
                            RecalculateHeading();
                            TriggerNorthHeadingUpdatedEvent(true);
                        }
                    }
                }
            }
        }
    }

    #if UNITY_EDITOR
    
    private void Reset() {

        if (arCamera == null) {
            arCamera = this.GetComponentInChildren<Camera>();
        }

        if (locationProvider == null) {
            locationProvider = GameObject.FindObjectOfType<pLab_LocationProvider>();
        }

        if (headingProvider == null) {
            headingProvider = GameObject.FindObjectOfType<pLab_HeadingProvider>();
        }
    }

    #endif

    #endregion

    #region Event Handlers

    /// <summary>
    /// Event handler for OnLocationUpdatedEvent. Tries to calculate new GPS-AR -heading if functionality enabled
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLocationUpdated(object sender, pLab_LocationUpdatedEventArgs e)
    {
        if (calculationMode == CalculationMode.GPSAndARMovement || calculationMode == CalculationMode.Both) {
            CalculateGPSARHeadings(e);
        }
    }

    /// <summary>
    /// Event handler for ARSessionStateChangedEvent
    /// </summary>
    /// <param name="e"></param>
    private void OnARSessionStateChange(ARSessionStateChangedEventArgs e)
    {
        if (e.state == ARSessionState.SessionTracking) {
            isSessionTracking = true;
            ResetCompassMode();
            ResetGPSARMode();
        } else {
            isSessionTracking = false;
        }
    }

    #endregion


    #region Private Methods

    /// <summary>
    /// Calculates GPS-AR Mode headings
    /// </summary>
    /// <param name="e"></param>
    private void CalculateGPSARHeadings(pLab_LocationUpdatedEventArgs e) {

        if (previousGPSCoordinates == null) {
            previousGPSCoordinates = e.location;
            previousARCameraPosition = arCameraTransform.position;
            return;
        }

        bool isHeadingUsable = false;
        bool isGPSHeadingUsable = false;
        float newARHeading = 0;
        float newGPSHeading = 0;

        Vector3 newPosition = arCameraTransform.position;

        Vector3 positionDeltaVector = newPosition - previousARCameraPosition;

        distance = positionDeltaVector.magnitude;

        if (distance > minDistanceBeforeUpdate) {

            newARHeading = Vector3.SignedAngle(Vector3.forward, positionDeltaVector, Vector3.up);
            newARHeading = pLab_MathConversions.DegAngleToPositive(newARHeading);
            isHeadingUsable = true;
        }

        pLab_LatLon newGpsCoordinates = e.location;
        float deltaAccuracy = e.horizontalAccuracy - previousGPSAccuracy;

        if (e.horizontalAccuracy < movementMinimumGPSAccuracy || deltaAccuracy <= 5f) {

            double distanceBetween = pLab_GeoTools.DistanceBetweenPointsPythagoras(previousGPSCoordinates, newGpsCoordinates);

            if (distanceBetween > minDistanceBeforeUpdate) {
                newGPSHeading = pLab_GeoTools.BearingFromPointAToBInDegrees(previousGPSCoordinates, newGpsCoordinates);
                //Convert to 0 - 360
                newGPSHeading = pLab_MathConversions.DegAngle0To360(newGPSHeading);
                isGPSHeadingUsable = true;
            }
        }

        if (isHeadingUsable && isGPSHeadingUsable) {
            arHeading = newARHeading;
            gpsHeading = newGPSHeading;
            previousGPSCoordinates = newGpsCoordinates;
            previousARCameraPosition = newPosition;
            previousGPSAccuracy = e.horizontalAccuracy;

            float diff = CalculateGPSARHeadingDifference(arHeading, gpsHeading);

            if (lastGPSARHeadingDeltas.Count == 0) {
                initialGPSARDifference = diff;
                lastGPSARHeadingDeltas.Add(0);
            } else {
                if (lastGPSARHeadingDeltas.Count >= maxLastMovementHeadings) {
                    lastGPSARHeadingDeltas.RemoveAt(0);
                }

                float readingDelta = diff - initialGPSARDifference;

                if (readingDelta > 180) {
                    readingDelta -= 360;
                } else if (readingDelta < -180) {
                    readingDelta += 360;
                }

                lastGPSARHeadingDeltas.Add(readingDelta);
            }

            medianGPSARHeadingDifference = pLab_MathTools.GetMedian(lastGPSARHeadingDeltas) + initialGPSARDifference;
            averageGPSARHeadingDifference = lastGPSARHeadingDeltas.Average() + initialGPSARDifference;

            hasGPSARHeading = true;

            RecalculateHeading();
            RotateGPSARHeadingObject();
            TriggerNorthHeadingUpdatedEvent(false);
            
            
            if (OnHeadingFromMovementUpdated != null) {
                pLab_HeadingFromMovementUpdatedEventArgs eventArgs = new pLab_HeadingFromMovementUpdatedEventArgs() {
                    arHeading = arHeading,
                    gpsHeading = gpsHeading,
                    headingDifference = diff,
                    medianHeadingDifference = medianGPSARHeadingDifference,
                    averageHeadingDifference = averageGPSARHeadingDifference
                };

                OnHeadingFromMovementUpdated(this, eventArgs);
            }
        }
    }

    /// <summary>
    /// Calculates difference between given ARHeading and GPSHeading
    /// </summary>
    /// <param name="arHeading"></param>
    /// <param name="gpsHeading"></param>
    /// <returns>Difference between</returns>
    private float CalculateGPSARHeadingDifference(float arHeading, float gpsHeading) {
        return pLab_MathConversions.DegAngleToPositive(arHeading - gpsHeading);
    }

    /// <summary>
    /// Resets GPS-AR and Compass-mode datas and readings
    /// </summary>
    private void ResetBothModes() {
        ResetGPSARMode();
        ResetCompassMode();
    }

    /// <summary>
    /// Reset GPS-AR -mode related variables
    /// </summary>
    private void ResetGPSARMode() {
        if (resetAfterTrackingLost) {
            hasGPSARHeading = false;
            manualOffset = 0;
            previousGPSCoordinates = null;
            previousGPSAccuracy = 9999f;
            initialGPSARDifference = 0;
            lastGPSARHeadingDeltas.Clear();
        }
    }

    /// <summary>
    /// Reset Compass-mode related variables to default values
    /// </summary>
    private void ResetCompassMode() {
        if (resetAfterTrackingLost) {
            ResetCompassReadings();
            hasCompassHeading = false;
            manualOffset = 0;
            CalculationPhaseProp = CalculationPhase.Stopped;
        }
    }

    /// <summary>
    /// Delete all previous Compass-mode readings
    /// </summary>
    private void ResetCompassReadings() {
        initialReading = 0;
        readingDeltas.Clear();
    }



    /// <summary>
    /// Take a new compass true north heading recording.
    /// </summary>
    private bool RecordCompassHeading() {
        if (Mathf.Abs(headingProvider.HeadingSmoothVelocity) > 10f) return false;

        float reading = GetAngleFromCompassHeading(headingProvider.FilteredHeading);

        if (readingDeltas.Count == 0) {
            initialReading = reading;
            readingDeltas.Add(0f);
        } else {

            //Sample size reached, start removing old values
            if (HeadingSampleSizeReached) {
                readingDeltas.RemoveAt(0);
            }

            float readingDelta = reading - initialReading;

            if (readingDelta > 180) {
                readingDelta -= 360;
            } else if (readingDelta < -180) {
                readingDelta += 360;
            }

            readingDeltas.Add(readingDelta);


        }

        return true;
    }

    /// <summary>
    /// Recalculate the new heading based on CalculationMode and calculated Compass- and GPS-AR -headings.
    /// </summary>
    private void RecalculateHeading() {
        float recalculatedHeading = 0;

        switch(calculationMode) {
            case CalculationMode.Compass:
                recalculatedHeading = compassModeHeading;

                break;
            case CalculationMode.GPSAndARMovement:
                //If we don't yet have the GPS-AR -heading, fallback to compass heading if we have it
                if (hasGPSARHeading) {
                    recalculatedHeading = averageGPSARHeadingDifference;
                }
                else if (hasCompassHeading){
                    recalculatedHeading = compassModeHeading;
                }

                break;
            case CalculationMode.Both:
                //If both headings are actually calculated
                if (hasCompassHeading && hasGPSARHeading) {
                    //Average of two angles with Lerp
                    recalculatedHeading = pLab_MathConversions.DegAngle0To360(Mathf.LerpAngle(averageGPSARHeadingDifference, compassModeHeading, 0.5f));
                }
                //If we only have the compass heading calculated
                else if (hasCompassHeading) {
                    recalculatedHeading = compassModeHeading;
                }
                //If we only have the GPS-AR -heading calculated
                else if (hasGPSARHeading) {
                    recalculatedHeading = averageGPSARHeadingDifference;
                }

                break;
            case CalculationMode.Manual:
                //Do nothing
                break;
        }

        heading = recalculatedHeading + manualOffset;

        headingTimestamp = DateTime.Now.Millisecond;

        //DEBUG
        RotateTrueNorthIndicatorTo(heading);
        //END DEBUG
    }

    /// <summary>
    /// Recalculates median Compass-heading from readings.
    /// </summary>
    private void RecalculateMedianCompassHeading() {
        if (readingDeltas != null && readingDeltas.Count > 0) {
            float median = pLab_MathTools.GetMedian(readingDeltas);
            compassModeHeading = initialReading + median;

            hasCompassHeading = true;

            //DEBUG
            RotateCompassHeadingObject();
            //END DEBUG
        }
    }

    /// <summary>
    /// Get the difference between compass heading and AR-camera rotation to indicate the difference.
    /// </summary>
    /// <param name="heading"></param>
    /// <returns>Difference between compass heading and AR-camera rotation</returns>
    private float GetAngleFromCompassHeading(float heading) {
        float newRotY = 0;
        
        //Relative to Z-axis in Unity-coordinate system
        float cameraYRot = arCamera.transform.localRotation.eulerAngles.y;

        newRotY = cameraYRot - heading;
        newRotY = pLab_MathConversions.DegAngleToPositive(newRotY);

        return newRotY;

    }

    /// <summary>
    /// Trigger the NorthHeadingUpdatedEvent
    /// </summary>
    private void TriggerNorthHeadingUpdatedEvent(bool isPriority = false) {
        if (OnHeadingUpdated != null) {
            pLab_NorthHeadingUpdatedEventArgs args = new pLab_NorthHeadingUpdatedEventArgs() {
                heading = heading,
                timestamp = headingTimestamp,
                isPriority = isPriority
            };

            OnHeadingUpdated(this, args);
        }
    }

    #region Indicator Debug Rotate Methods

    /// <summary>
    /// Rotates the indicator to Unity-world north
    /// </summary>
    /// <param name="heading"></param>
    private void RotateTrueNorthIndicatorTo(float heading) {
        if (headingObjectToRotate != null) {
            Vector3 originalRotation = headingObjectToRotate.transform.rotation.eulerAngles;
            originalRotation.y = heading;
            headingObjectToRotate.transform.rotation = Quaternion.Euler(originalRotation);
        }
    }

    private void RotateCompassHeadingObject() {
        if (compassHeadingObjectToRotate != null) {
            Vector3 originalRotation = compassHeadingObjectToRotate.transform.rotation.eulerAngles;
            originalRotation.y = compassModeHeading;
            compassHeadingObjectToRotate.transform.rotation = Quaternion.Euler(originalRotation);
        }
    }
 
    private void RotateGPSARHeadingObject() {
        if (GPSARHeadingObjectToRotate != null) {
            Vector3 originalRotation = GPSARHeadingObjectToRotate.transform.rotation.eulerAngles;
            originalRotation.y = averageGPSARHeadingDifference;
            GPSARHeadingObjectToRotate.transform.rotation = Quaternion.Euler(originalRotation);
        }
    }

    #endregion

    #endregion

    #region Public Methods

    /// <summary>
    /// Manually offset the true north to East
    /// </summary>
    public void OffsetToEast() {
        manualOffset += manualOffsetIncremental;
        RecalculateHeading();
        TriggerNorthHeadingUpdatedEvent(true);
    }

    /// <summary>
    /// Manually offset the true north to West
    /// </summary>
    public void OffsetToWest() {
        manualOffset -= manualOffsetIncremental;
        RecalculateHeading();
        TriggerNorthHeadingUpdatedEvent(true);
    }

    #endregion

    #region IEnumerations

    #endregion
}

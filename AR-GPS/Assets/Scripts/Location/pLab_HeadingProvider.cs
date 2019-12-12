/******************************************************************************
* File         : pLab_HeadingProvider.cs
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

/// <summary>
/// Heading updated event arguments.
/// Heading represents a facing angle between 0-359.
/// </summary>
public class pLab_HeadingUpdatedEventArgs : EventArgs
{
    public float heading;
    public float filteredHeading;
    public float filteredHeadingVelocity;
    public double timestamp;
}


public class pLab_HeadingProvider : MonoBehaviour
{
    #region Variables

    double lastHeadingTimestamp;

    [SerializeField]
    [Tooltip("How much smoothing is happening. 0 = No smoothing")]
    [Range(0f, 1f)]
    private float headingSmooth = 0.3f;

    private float headingSmoothVelocity = 0.0f;

    private float heading = 0;
    private float filteredHeading = -1f;

    #region Debug Variables

    [Header("Debugging")]
    [SerializeField]
    private bool useFakeData = true;

    [SerializeField]
    private float fakeHeading = 0f;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the compass updates.
    /// </summary>
    public event EventHandler<pLab_HeadingUpdatedEventArgs> OnHeadingUpdated;

    #endregion

    #endregion

    #region Properties

    public bool UseFakeData { get { return useFakeData; } set { Input.compass.enabled = !value; useFakeData = value; } }
    
    public float Heading { get { return heading; } }
    public double LastHeadingTimestamp { get { return lastHeadingTimestamp; } }
    public float FilteredHeading { get { return filteredHeading; } }
    public float HeadingSmoothVelocity { get { return headingSmoothVelocity; } }
    
    
    #endregion

    #region Inherited Methods

    private void Start() {
        if (!useFakeData) {
            Input.compass.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
       if (!useFakeData) {
           PollHeading();
       } else {
           PollFakeHeading();
       }
    }

    #endregion


    #region Private Methods

    /// <summary>
    /// Use magic to make compass heading's variations smoother
    /// </summary>
    /// <param name="rawHeading"></param>
    /// <returns></returns>
    private float CalculateFilteredHeading(float rawHeading) {
        if (filteredHeading < 0) {
            filteredHeading = rawHeading;
        }
        else {
            //Filter out rapid small changes
            filteredHeading = Mathf.SmoothDampAngle(filteredHeading, rawHeading, ref headingSmoothVelocity, headingSmooth);

            filteredHeading = pLab_MathConversions.DegAngle0To360(filteredHeading);
        }

        return filteredHeading;
    }

    private void PollHeading() {
        double timestamp = Input.compass.timestamp;

        if (Input.compass.enabled && timestamp > lastHeadingTimestamp)
        {
            heading = Input.compass.trueHeading;
            lastHeadingTimestamp = timestamp;

            CalculateFilteredHeading(heading);
            SendUpdatedHeading();
        }
    }

    private void PollFakeHeading() {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        lastHeadingTimestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;

        heading = fakeHeading;
        filteredHeading = fakeHeading;
        headingSmoothVelocity = 0.5f;

        SendUpdatedHeading();
    }

    private void SendUpdatedHeading()
    {
        if (OnHeadingUpdated != null)
        {
            pLab_HeadingUpdatedEventArgs eventArgs = new pLab_HeadingUpdatedEventArgs()
            {
                heading = heading,
                filteredHeading = filteredHeading,
                timestamp = lastHeadingTimestamp,
                filteredHeadingVelocity = headingSmoothVelocity
            };

            OnHeadingUpdated(this, eventArgs);
        }
    }

    #endregion

    #region Debug Methods

    public void MoveFakeCompassEast() {
        fakeHeading = fakeHeading + 5f;
        fakeHeading = pLab_MathConversions.DegAngle0To360(fakeHeading);
    }

    public void MoveFakeCompassWest() {
        fakeHeading = fakeHeading - 5f;
        fakeHeading = pLab_MathConversions.DegAngleToPositive(fakeHeading);
    }

    #endregion
    
}

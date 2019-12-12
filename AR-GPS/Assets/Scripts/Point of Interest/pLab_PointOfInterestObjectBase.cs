/******************************************************************************
* File         : pLab_PointOfInterestObjectBase.cs
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

public abstract class pLab_PointOfInterestObjectBase : MonoBehaviour
{

    protected pLab_PointOfInterest pointOfInterest;
    
    #region Properties

    public pLab_PointOfInterest PointOfInterest { get { return pointOfInterest; } set { pointOfInterest = value; } }
    
    #endregion

    #region Methods

    /// <summary>
    /// Setup-function that is called after object is instantiated
    /// </summary>
    /// <param name="poi"></param>
    /// <param name="arCamera"></param>
    public abstract void Setup(pLab_PointOfInterest poi, Camera arCamera);

    /// <summary>
    /// Update position including all axises.
    /// </summary>
    /// <param name="pos">New position (Y is ignored)</param>
    /// <param name="groundLevelY">Ground level (real world) Y-position</param>
    /// <param name="devicePosY">Device's (camera's) Y-position</param>
    public abstract void UpdatePosition(Vector3 pos, float groundLevelY, float devicePosY);


    /// <summary>
    /// Update position including ONLY the Y-axis (height)
    /// </summary>
    /// <param name="groundLevelY"></param>
    /// <param name="devicePosY"></param>
    public abstract void UpdatePositionY(float groundLevelY, float devicePosY);


    /// <summary>
    /// Update position including only the X- and Z-axis. Height was not changed.
    /// </summary>
    /// <param name="newPos"></param>
    public abstract void UpdatePositionXZ(Vector3 newPos);

    /// <summary>
    /// Activate or deactivate object based on isVisible
    /// </summary>
    /// <param name="isVisible"></param>
    public virtual void SetVisibility(bool isVisible) {
        gameObject.SetActive(isVisible);
    }

    /// <summary>
    /// Update object's rotation based on true north heading and POI facing direction
    /// </summary>
    /// <param name="trueNorthHeading"></param>
    public virtual void UpdateRotation(float trueNorthHeading) {
        Vector3 rotEuler = transform.rotation.eulerAngles;

        rotEuler.y = trueNorthHeading + pointOfInterest.FacingDirectionHeading;

        transform.rotation = Quaternion.Euler(rotEuler);
    }

    
    #endregion

}

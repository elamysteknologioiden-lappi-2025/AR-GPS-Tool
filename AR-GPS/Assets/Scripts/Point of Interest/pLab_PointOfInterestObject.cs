/******************************************************************************
* File         : pLab_PointOfInterestObject.cs
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

public class pLab_PointOfInterestObject : pLab_PointOfInterestObjectBase
{

    private Transform objectTransform;

    #region Inherited Methods

    private void Awake() {
        objectTransform = transform;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Calculate new height for 
    /// </summary>
    /// <param name="groundLevelY"></param>
    /// <param name="devicePosY"></param>
    /// <returns></returns>
    private float CalculateHeight(float groundLevelY, float devicePosY) {
        float posY = 0;

        switch(pointOfInterest.PositionMode) {
            case POIPositionMode.AlignWithGround:
                posY = groundLevelY - 0.05f;
                break;
            case POIPositionMode.RelativeToDevice:
                posY = devicePosY + pointOfInterest.RelativeHeight;
                break;
            case POIPositionMode.RelativeToGround:
                posY = groundLevelY + pointOfInterest.RelativeHeight;
                break;
        }

        return posY;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initial setup for object.
    /// </summary>
    /// <param name="poi"></param>
    /// <param name="arCamera"></param>
    public override void Setup(pLab_PointOfInterest poi, Camera arCamera) {
        this.pointOfInterest = poi;
    }

    /// <summary>
    /// Set position. Height is determined by the POI positionMode setting which used groundLevel and devicePosY to calculate height
    /// </summary>
    /// <param name="pos">New position (Y is ignored)</param>
    /// <param name="groundLevelY">Ground level (real world) Y-position</param>
    /// <param name="devicePosY">Device's (camera's) Y-position</param>
    public override void UpdatePosition(Vector3 pos, float groundLevelY, float devicePosY) {

        pos.y = CalculateHeight(groundLevelY, devicePosY);
        
        objectTransform.localPosition = pos;
    }

    public override void UpdatePositionY(float groundLevelY, float devicePosY) {
        Vector3 localPos = objectTransform.localPosition;
        localPos.y = CalculateHeight(groundLevelY, devicePosY);
        objectTransform.localPosition = localPos;
    }


    /// <summary>
    /// Change object position but Y-value (height) is not changed
    /// </summary>
    /// <param name="newPos"></param>
    public override void UpdatePositionXZ(Vector3 newPos) {
        newPos.y = objectTransform.localPosition.y;
        objectTransform.position = newPos;
    }


    #endregion
}

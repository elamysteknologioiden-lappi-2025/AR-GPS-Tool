/******************************************************************************
* File         : pLab_PointOfInterestItemDebug.cs
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
using UnityEngine.UI;

public class pLab_PointOfInterestItemDebug : MonoBehaviour
{
    [SerializeField]
    private Text poiNameText;

    [SerializeField]
    private Text distanceToPoiText;

    [SerializeField]
    private Text bearingToPoiText;

    [SerializeField]
    private GameObject isTrackingIcon;

    [SerializeField]
    private GameObject isCloseTrackingIcon;

    [SerializeField]
    private Text positionText;

    private pLab_PointOfInterest pointOfInterest;

    public void Setup(pLab_PointOfInterest poi) {
        pointOfInterest = poi;
        UpdateIsTrackingIcon();
        UpdateName();
        ToggleBearingToPoiTextVisiblity(false);
        TogglePositionTextVisibility(false);
    }

    public void UpdateInfo(double distance) {
        SetDistanceToPoiText(distance);
        ToggleBearingToPoiTextVisiblity(false);
        UpdateIsTrackingIcon();
        TogglePositionTextVisibility(false);
    }

    public void UpdateInfo(double distance, float bearing) {
        UpdateIsTrackingIcon();
        SetDistanceToPoiText(distance);
        SetBearingToPoiText(bearing);
    }

    public void UpdateInfo(double distance, float bearing, Vector3 pos) {
        this.UpdateInfo(distance, bearing);
        SetPositionText(pos);
        TogglePositionTextVisibility(true);
    }

    public void UpdateIsTrackingIcon() {
        if (isTrackingIcon != null) {
            isTrackingIcon.SetActive(pointOfInterest.Tracking);
        }

        if (isCloseTrackingIcon != null) {
            isCloseTrackingIcon.SetActive(pointOfInterest.CloseTracking);
        }
    }

    public void UpdateName() {
        if (poiNameText != null) {
            poiNameText.text = pointOfInterest.PoiName;
        }
    }

    public void SetDistanceToPoiText(double distance) {
        if (distanceToPoiText != null) {
            distanceToPoiText.text = string.Format("{0} m", distance.ToString("F2"));
        }
    }

    public void SetBearingToPoiText(float bearingInRad) {

        if (bearingToPoiText != null) {
            float bearing = pLab_MathConversions.RadToDeg(bearingInRad);

            bearing = pLab_MathConversions.DegAngleToPositive(bearing);

            bearingToPoiText.text = string.Format("{0} deg", bearing.ToString("F1"));
            ToggleBearingToPoiTextVisiblity(true);
        }
    }

    public void SetPositionText(Vector3 pos) {
        if (positionText != null) {
            positionText.text = string.Format("X {0}  Y {1}  Z {2}", pos.x.ToString("F2"), pos.y.ToString("F1"), pos.z.ToString("F2"));
        }
    }

    public void ToggleBearingToPoiTextVisiblity(bool isVisible) {
        if (bearingToPoiText != null) {
            bearingToPoiText.gameObject.SetActive(isVisible);
        }
    }

    public void TogglePositionTextVisibility(bool isVisible) {
        if (positionText != null) {
            positionText.gameObject.SetActive(isVisible);
        }
    }

}

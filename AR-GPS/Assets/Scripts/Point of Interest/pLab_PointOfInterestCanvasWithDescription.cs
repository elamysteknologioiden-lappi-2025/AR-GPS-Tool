/******************************************************************************
* File         : pLab_PointOfInterestCanvasWithDescription.cs
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
using UnityEngine.EventSystems;

public class pLab_PointOfInterestCanvasWithDescription : pLab_PointOfInterestCanvasBase, IPointerClickHandler
{
    #region Variables

    [SerializeField]
    private GameObject descriptionObject;

    [SerializeField]
    private Text descriptionText;

    private Camera arCamera;

    private Transform cameraTransform;

    private Vector3 worldPosition;

    [SerializeField]
    private Text poiNameText;

    [SerializeField]
    private Image poiIcon;

    [SerializeField]
    private GameObject poiIconBackground;

    [SerializeField]
    private Text poiDistanceText;

    [SerializeField]
    private RectTransform panelTransform;

    #endregion

    #region Properties
    
    #endregion

    #region Inherit Methods

    private void Start() {
        if (descriptionObject != null) {
            descriptionObject.SetActive(false);
        }
    }

    private void Update() {

        if (panelTransform != null) {
            Vector3 screenPosition = arCamera.WorldToScreenPoint(worldPosition);

            panelTransform.gameObject.SetActive(screenPosition.z > 0);
            
            panelTransform.position = screenPosition;
        }

        SetDistanceText((cameraTransform.position - worldPosition).magnitude);
    }

    #endregion

    #region Private Methods
    private void SetDistanceText(float distance) {
        if (poiDistanceText != null) {
            poiDistanceText.text = $"{(int) distance} m";
        }
    }
    #endregion

    #region Public Methods
    
    public override void Setup(pLab_PointOfInterest poi, Camera arCamera) {
        this.pointOfInterest = poi;
        this.arCamera = arCamera;
        this.cameraTransform = arCamera.transform;
        UpdateInfo();
    }

    public void UpdateInfo() {
        if (pointOfInterest != null) {
            if (poiNameText != null) {
                poiNameText.text = pointOfInterest.PoiName;
            }

            if (descriptionText != null) {
                descriptionText.text = pointOfInterest.Description;
            }
            
            if(poiIcon != null) {
                bool hasIcon = pointOfInterest.Icon != null;

                if (hasIcon) {
                    poiIcon.sprite = pointOfInterest.Icon;
                }

                if (poiIconBackground != null) {
                    poiIconBackground.SetActive(hasIcon);
                }
            }
        }
    }

    public override void UpdatePosition(Vector3 worldPosition, float groundLevelY, float devicePosY) {
        this.worldPosition = worldPosition;
        UpdatePositionY(groundLevelY, devicePosY);
    }

    public override void UpdatePositionXZ(Vector3 newPos) {
        newPos.y = worldPosition.y;
        worldPosition = newPos;
    }

    public override void UpdatePositionY(float groundLevelY, float devicePosY) {
        float posY = 0;

        switch(pointOfInterest.PositionMode) {
            case POIPositionMode.AlignWithGround:
                posY = devicePosY;
                break;
            case POIPositionMode.RelativeToDevice:
                posY = devicePosY + pointOfInterest.RelativeHeight + 1f;
                break;
            case POIPositionMode.RelativeToGround:
                posY = groundLevelY + pointOfInterest.RelativeHeight + 1f;
                break;
        }

        this.worldPosition.y = posY;
    }

    public override void UpdateDistance(float distance)
    {
        SetDistanceText(distance);
    }

    public void ToggleDescriptionVisibility(bool isVisible) {
        if (descriptionObject != null && descriptionText != null && !descriptionText.text.Equals("")) {
            descriptionObject.SetActive(isVisible);
        }
    }

    public void ToggleDescriptionVisibility() {
        if (descriptionObject != null) {
            ToggleDescriptionVisibility(!descriptionObject.activeSelf);
        }
    }

    #endregion

    #region Event Handlers

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleDescriptionVisibility();
    }

    #endregion
}

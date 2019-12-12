/******************************************************************************
* File         : pLab_DebugSettingsPanel.cs
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

public class pLab_DebugSettingsPanel : MonoBehaviour
{

    [SerializeField]
    private pLab_ARPointOfInterestManager arPointOfInterestManager;

    [SerializeField]
    private pLab_ARTrueNorthFinder arSessionToTrueNorth;

    [SerializeField]
    private pLab_LocationProvider locationProvider;

    [SerializeField]
    private pLab_HeadingProvider headingProvider;

    [SerializeField]
    private Toggle automaticTrueNorthToggle;

    [SerializeField]
    private Toggle useFakeGPSDataToggle;

    [SerializeField]
    private Dropdown trueNorthDropdown;

    [SerializeField]
    private Toggle useFakeCompassDataToggle;

    [SerializeField]
    private Toggle useGPSARForTrueNorthToggle;

    [SerializeField]
    private Toggle useUTMToggle;

    public void Start() {
        if (automaticTrueNorthToggle != null && arSessionToTrueNorth != null) {
            automaticTrueNorthToggle.isOn = arSessionToTrueNorth.IsEnabled;
        }

        if (useFakeGPSDataToggle != null && locationProvider != null) {
            useFakeGPSDataToggle.isOn = locationProvider.UseFakeData;
        }

        if (useFakeCompassDataToggle != null && headingProvider != null) {
            useFakeCompassDataToggle.isOn = headingProvider.UseFakeData;
        }

        if (useGPSARForTrueNorthToggle != null && arSessionToTrueNorth != null) {
            useGPSARForTrueNorthToggle.isOn = arSessionToTrueNorth.CalculationModeProp == pLab_ARTrueNorthFinder.CalculationMode.GPSAndARMovement;
        }

        if (trueNorthDropdown != null && arSessionToTrueNorth != null) {
            if (!arSessionToTrueNorth.IsEnabled) {
                trueNorthDropdown.value = 0;
            } else {
                trueNorthDropdown.value = (int) arSessionToTrueNorth.CalculationModeProp + 1;
            }
        }

        if (useUTMToggle != null) {
            if (arPointOfInterestManager != null) {
                useUTMToggle.interactable = true;
                useUTMToggle.isOn = arPointOfInterestManager.PositioningModeProperty == pLab_ARPointOfInterestManager.PositioningMode.UTM;
            } else {
                useUTMToggle.interactable = false;
            }
        }
    }

    public void SetAutomaticTrueNorth(bool isOn) {
        if (arSessionToTrueNorth != null) {
            arSessionToTrueNorth.IsEnabled = isOn;
        }
    }

    public void SetUseFakeGPSData(bool isOn) {
        if (locationProvider != null) {
            locationProvider.UseFakeData = isOn;
        }
    }

    public void SetUseFakeCompassData(bool isOn) {
        if (headingProvider != null) {
            headingProvider.UseFakeData = isOn;
        }
    }

    public void SetUseGPSARForTrueNorth(bool isOn) {
        if (arSessionToTrueNorth != null) {
            arSessionToTrueNorth.CalculationModeProp = isOn ? pLab_ARTrueNorthFinder.CalculationMode.GPSAndARMovement : pLab_ARTrueNorthFinder.CalculationMode.Compass;
        }
    }

    public void SetTrueNorthMode(int selectedIndex) {
        if (arSessionToTrueNorth != null) {
            if (selectedIndex == 0) {
                arSessionToTrueNorth.IsEnabled = false;
            } else {
                arSessionToTrueNorth.IsEnabled = true;
                
                switch(selectedIndex) {
                    case 1:
                        arSessionToTrueNorth.CalculationModeProp = pLab_ARTrueNorthFinder.CalculationMode.Compass;
                        break;
                    case 2:
                        arSessionToTrueNorth.CalculationModeProp = pLab_ARTrueNorthFinder.CalculationMode.GPSAndARMovement;
                        break;
                    case 3:
                        arSessionToTrueNorth.CalculationModeProp = pLab_ARTrueNorthFinder.CalculationMode.Both;
                        break;
                    case 4: arSessionToTrueNorth.CalculationModeProp = pLab_ARTrueNorthFinder.CalculationMode.Manual;
                        break;
                }
                
            }
        }
    }

    public void SetUTM(bool isOn) {
        if (arPointOfInterestManager != null) {
            arPointOfInterestManager.PositioningModeProperty = isOn ? pLab_ARPointOfInterestManager.PositioningMode.UTM : pLab_ARPointOfInterestManager.PositioningMode.DistanceAndBearing;
        }
    }
}

/******************************************************************************
* File         : pLab_TrueNorthCalculationUI.cs
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

public class pLab_TrueNorthCalculationUI : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private pLab_ARTrueNorthFinder trueNorthFinder;

    [SerializeField]
    private GameObject findingNorthIndicator;

    #endregion

    #region Inherited Methods

    #if UNITY_EDITOR

    private void Reset() {
        if (trueNorthFinder == null) {
            trueNorthFinder = GameObject.FindObjectOfType<pLab_ARTrueNorthFinder>();
        }
    }

    #endif


    private void OnEnable() {
        SetFindingWithCompassIndicatorVisibility(trueNorthFinder != null && trueNorthFinder.IsEnabled && trueNorthFinder.CalculationPhaseProp == pLab_ARTrueNorthFinder.CalculationPhase.FastInterval);

        if (trueNorthFinder != null) {
            trueNorthFinder.OnCalculationPhaseChanged += OnCalculationPhaseChanged;
        }
    }

    private void OnDisable() {
        if (trueNorthFinder != null) {
            trueNorthFinder.OnCalculationPhaseChanged -= OnCalculationPhaseChanged;
        }
    }

    private void Start() {
        SetFindingWithCompassIndicatorVisibility(trueNorthFinder != null && trueNorthFinder.IsEnabled && trueNorthFinder.CalculationPhaseProp == pLab_ARTrueNorthFinder.CalculationPhase.FastInterval);
    }

    #endregion

    #region Event Handlers

    private void OnCalculationPhaseChanged(object sender, pLab_CalculationPhaseChangedEventArgs e)
    {
        SetFindingWithCompassIndicatorVisibility(e.calculationPhase == pLab_ARTrueNorthFinder.CalculationPhase.FastInterval);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Set the visibility of the findingNorthIndicator.
    /// </summary>
    /// <param name="isVisible"></param>
    public void SetFindingWithCompassIndicatorVisibility(bool isVisible) {
        if (findingNorthIndicator != null) {
            findingNorthIndicator.SetActive(isVisible);
        }
    }
    
    #endregion
}

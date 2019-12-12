/******************************************************************************
* File         : pLab_DebugPositionRotation.cs
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

public class pLab_DebugPositionRotation : MonoBehaviour
{
    [SerializeField]
    private Transform transformToDebug;

    [SerializeField]
    private bool debugLocalPosition = false;

    [SerializeField]
    private bool debugLocalRotation = false;

    [SerializeField]
    private Text positionText;

    [SerializeField]
    private Text rotationText;

    [SerializeField]
    private float updateInterval = 2f;

    private float timer = 0;

    private void Update() {
        if (transformToDebug == null) return;

        timer += Time.deltaTime;

        if (timer >= updateInterval) {
            UpdateTexts();
            timer = 0;
        }
    }

    private void UpdateTexts() {
        
        if (positionText != null) {
            Vector3 pos = debugLocalPosition ? transformToDebug.localPosition : transformToDebug.position;
            positionText.text = string.Format("POS X {0} Y {1} Z {2}", pos.x.ToString("F2"), pos.y.ToString("F2"), pos.z.ToString("F2"));
        }

        if (rotationText != null) {
            Vector3 rot = debugLocalRotation ? transformToDebug.localRotation.eulerAngles : transformToDebug.rotation.eulerAngles;
            rotationText.text = string.Format("ROT X {0} Y {1} Z {2}", rot.x.ToString("F2"), rot.y.ToString("F2"), rot.z.ToString("F2"));
        }
    }

}

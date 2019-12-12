/******************************************************************************
* File         : pLab_PointOfInterestDebug.cs
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

public class pLab_PointOfInterestDebug : MonoBehaviour
{

    [SerializeField]
    private Transform poiListParent;

    [SerializeField]
    private GameObject poiItemTemplate;

    private Dictionary<pLab_PointOfInterest, pLab_PointOfInterestItemDebug> poiItems = new Dictionary<pLab_PointOfInterest, pLab_PointOfInterestItemDebug>();

    public void SetupPOIList(List<pLab_PointOfInterest> pointOfInterests) {
        poiItemTemplate.SetActive(false);

        foreach(pLab_PointOfInterest poi in pointOfInterests) {
            GameObject go = Instantiate(poiItemTemplate);
            go.transform.SetParent(poiListParent, false);
            pLab_PointOfInterestItemDebug poiItem = go.GetComponent<pLab_PointOfInterestItemDebug>();
            poiItems.Add(poi, poiItem);
            poiItem.Setup(poi);
            poiItem.gameObject.SetActive(true);
        }

    }

    public void UpdateItem(pLab_PointOfInterest poi, double distance) {
        pLab_PointOfInterestItemDebug itemDebug = null;
        if (poiItems.TryGetValue(poi, out itemDebug)) {
            itemDebug.UpdateInfo(distance);
        }
    }

    public void UpdateItem(pLab_PointOfInterest poi, double distance, float bearing) {
        pLab_PointOfInterestItemDebug itemDebug = null;
        if (poiItems.TryGetValue(poi, out itemDebug)) {
            itemDebug.UpdateInfo(distance, bearing);
        }
    }

    public void UpdateItem(pLab_PointOfInterest poi, double distance, float bearing, Vector3 pos) {
        pLab_PointOfInterestItemDebug itemDebug = null;
        if (poiItems.TryGetValue(poi, out itemDebug)) {
            itemDebug.UpdateInfo(distance, bearing, pos);
        }
    }

}

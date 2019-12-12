/******************************************************************************
* File         : pLab_PointOfInterest.cs
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

using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// POI Tracking state.
/// </summary>
public enum POITrackingState {
    /// <summary>
    /// POI is not being tracked (it isn't in the tracking radius)
    /// </summary>
    NotTracking = 0,
    /// <summary>
    /// POI is being tracked but the POI is still "far" away
    /// </summary>
    FarTracking = 1,
    /// <summary>
    /// POI is being tracked, and it is inside the close tracking radius
    /// </summary>
    CloseTracking = 2
}

/// <summary>
/// What mode is being used to position POI 3D-model (and canvas) on the Y-axis.
/// </summary>
public enum POIPositionMode {
    /// <summary>
    /// Y-position is at the ground level
    /// </summary>
    AlignWithGround = 0,
    /// <summary>
    /// Y-position can be changed relative to the ground
    /// </summary>
    RelativeToGround = 1,
    /// <summary>
    /// Y-position is relative to device
    /// </summary>
    RelativeToDevice = 2
}


[CreateAssetMenu(fileName = "New Point of Interest", menuName = "Point of Interest", order = 1)]
public class pLab_PointOfInterest : ScriptableObject
{
    #region Variables

    [Header("Information")]
    [SerializeField]
    private string poiName;

    [SerializeField]
    [TextArea(2, 20)]
    private string description;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private GameObject objectPrefab;

    [SerializeField]
    [FormerlySerializedAs("model")]
    private GameObject modelPrefab;

    [SerializeField]
    private GameObject canvasPrefab;

    [Header("Location")]
    [SerializeField]
    private pLab_LatLon coordinates;

    [Header("Far Tracking Radiuses (meters)")]
    [SerializeField]
    private int trackingRadius = 100;

    [SerializeField]
    private int trackingExitMargin = 20;

    [Header("Close Tracking Radiuses (meters)")]
    [SerializeField]
    private int closeTrackingRadius = 20;

    [SerializeField]
    private int closeTrackingExitMargin = 10;


    [Header("Positional Settings")]
    [SerializeField]
    private POIPositionMode positionMode = POIPositionMode.AlignWithGround;

    [SerializeField]
    [Tooltip("In what compass heading direction should the model face. 45 = 45 deg = North-East, 180 South")]
    [Range(0, 360)]
    private float facingDirectionHeading = 0f;

    [SerializeField]
    [Tooltip("This is only used when PositionMode is either RelativeToGround or RelativeToDevice")]
    private float relativeHeight = 0f;

    private POITrackingState trackingState;

    #endregion

    #region Properties

    public string PoiName { get { return poiName; } set { poiName = value; } }
    public string Description { get { return description; } set { description = value; } }
    public Sprite Icon { get { return icon; } set { icon = value; } }
    
    public GameObject ObjectPrefab { get { return objectPrefab; } set { objectPrefab = value; } }
    public GameObject ModelPrefab { get { return modelPrefab; } set { modelPrefab = value; } }
    public GameObject CanvasPrefab { get { return canvasPrefab; } set { canvasPrefab = value; } }
    
    public pLab_LatLon Coordinates { get { return coordinates; } set { coordinates = value; } }

    public int TrackingRadius { get { return trackingRadius; } set { trackingRadius = value; } }
    public int TrackingExitMargin { get { return trackingExitMargin; } set { trackingExitMargin = value; } }
    public int TrackingExitRadius { get { return trackingRadius + trackingExitMargin; } }

    public int CloseTrackingRadius { get { return closeTrackingRadius; } set { closeTrackingRadius = value; } }
    public int CloseTrackingExitMargin { get { return closeTrackingExitMargin; } set { closeTrackingExitMargin = value; } }
    public int CloseTrackingExitRadius { get { return closeTrackingRadius + closeTrackingExitMargin; } }


    public POITrackingState TrackingState { get { return trackingState; } set { trackingState = value; } }
    public bool Tracking { get { return trackingState == POITrackingState.FarTracking || trackingState == POITrackingState.CloseTracking; } }
    public bool CloseTracking { get { return trackingState == POITrackingState.CloseTracking; } }
    public bool FarTracking { get { return trackingState == POITrackingState.FarTracking; } }

    public POIPositionMode PositionMode { get { return positionMode; } set { positionMode = value; } }
    public float FacingDirectionHeading { get { return facingDirectionHeading; } set { facingDirectionHeading = value; } }
    public float RelativeHeight { get { return relativeHeight; } set { relativeHeight = value; } }
    
    #endregion


    #region Public Methods


    #endregion

}

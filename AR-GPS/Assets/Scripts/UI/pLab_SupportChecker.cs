/******************************************************************************
* File         : pLab_SupportChecker.cs
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


public class SupportCheckDoneEventArgs : System.EventArgs {
    public bool supported;
}

/// <summary>
/// This example shows how to check for AR support before the ARSession is enabled.
/// For ARCore in particular, it is possible for a device to support ARCore but not
/// have it installed. This example will detect this case and prompt the user to install ARCore.
/// To test this feature yourself, use a supported device and uninstall ARCore.
/// (Settings > Search for "ARCore" and uninstall or disable it.)
/// </summary>
public class pLab_SupportChecker : MonoBehaviour
{

    [SerializeField]
    private Text statusText;

    [SerializeField]
    private Button installButton;

    [SerializeField]
    private Button exitButton;

    [Header("AR Session")]

    [SerializeField]
    [Tooltip("Will this script manually enable and disable AR Session.")]
    private bool controlARSessionEnabledState = true;

    [SerializeField]
    private ARSession arSession;

    [SerializeField]
    private string checkingForARSupportText = "Checking for AR support...";

    [SerializeField]
    private string needsInstallingText = "Your device supports AR, but requires a software update. Attempting install...";

    [SerializeField]
    private string deviceNotSupportedText = "Your device does not support AR.";

    [SerializeField]
    private string updateFailedOrCancelledText = "The software update failed, or you declined the update.";

    [SerializeField]
    private string attemptingInstallText = "Attempting install...";




    public static event EventHandler<SupportCheckDoneEventArgs> OnSupportCheckDone;

    private void Awake() {
        if (installButton != null) {
            installButton.onClick.AddListener(OnInstallButtonPressed);
        }

        if (exitButton != null) {
            exitButton.onClick.AddListener(OnExitButtonPressed);
        }

        //If the ARSession was not set, try to find it from the scene
        if (arSession == null) {
            arSession = (ARSession) GameObject.FindObjectOfType(typeof(ARSession));
        }

        SetARSessionEnabled(false);
    }

    private void OnEnable()
    {
       StartCoroutine(CheckSupport());
    }

    /// <summary>
    /// Change status text to message
    /// </summary>
    /// <param name="message">Message to show</param>
    private void ChangeStatusText(string message)
    {
        statusText.text = $"{message}";
    }

    /// <summary>
    /// Check for support, and if some software needs installing
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckSupport()
    {
        #if UNITY_EDITOR

        yield return new WaitForSeconds(2f);
        SendSupportCheckDoneEvent(true);
        
        #endif

        SetExitButtonActive(false);
        SetInstallButtonActive(false);

        ChangeStatusText(checkingForARSupportText);

        yield return ARSession.CheckAvailability();

        ChangeStatusText(ARSession.state.ToString());
        

        if (ARSession.state == ARSessionState.NeedsInstall)
        {
            ChangeStatusText(needsInstallingText);
            yield return ARSession.Install();
        }

        if (ARSession.state == ARSessionState.Ready)
        {
            // To start the ARSession, we just need to enable it.
            SetARSessionEnabled(true);
            SendSupportCheckDoneEvent(true);
        }
        else
        {
            switch (ARSession.state)
            {
                case ARSessionState.Unsupported:
                    ChangeStatusText(deviceNotSupportedText);
                    break;
                case ARSessionState.NeedsInstall:
                    ChangeStatusText(updateFailedOrCancelledText);

                    // In this case, we enable a button which allows the user
                    // to try again in the event they decline the update the first time.
                    SetInstallButtonActive(true);
                    break;
                default:
                    SendSupportCheckDoneEvent(true);
                    break;
            }

            //Show user the exit button
            SetExitButtonActive(true);

        }
    }

    /// <summary>
    /// Send event when support check is done, and disabled the object
    /// </summary>
    /// <param name="supported">Was AR supported</param>
    private void SendSupportCheckDoneEvent(bool supported) {

        if (OnSupportCheckDone != null) {
            SupportCheckDoneEventArgs args = new SupportCheckDoneEventArgs() {
                supported = supported
            };

            OnSupportCheckDone(this, args);
        }


        this.gameObject.SetActive(false);


    }

    /// <summary>
    /// Set install button active based on active-parameter
    /// </summary>
    /// <param name="active">Should the button be active</param>
    private void SetInstallButtonActive(bool active)
    {
        if (installButton != null)
            installButton.gameObject.SetActive(active);
    }

    /// <summary>
    /// Set exit button active based on active-parameter
    /// </summary>
    /// <param name="active"></param>
    private void SetExitButtonActive(bool active) {
        if (exitButton != null) {
            exitButton.gameObject.SetActive(active);
        }
    }

    /// <summary>
    /// Try to install the necessary software for AR to work
    /// </summary>
    /// <returns></returns>
    private IEnumerator Install()
    {
        SetInstallButtonActive(false);

        if (ARSession.state == ARSessionState.NeedsInstall)
        {
            ChangeStatusText(attemptingInstallText);
            yield return ARSession.Install();

            if (ARSession.state == ARSessionState.NeedsInstall)
            {
                ChangeStatusText(updateFailedOrCancelledText);
                SetInstallButtonActive(true);
            }
            else if (ARSession.state == ARSessionState.Ready)
            {
                SendSupportCheckDoneEvent(true);
                SetARSessionEnabled(true);
            }
        }
    }

    private void OnInstallButtonPressed()
    {
        StartCoroutine(Install());
    }

    private void OnExitButtonPressed()
    {
        SendSupportCheckDoneEvent(false);
    }

    /// <summary>
    /// Toggle enabled-state of ARSession
    /// </summary>
    /// <param name="isEnabled"></param>
    private void SetARSessionEnabled(bool isEnabled) {
        if (controlARSessionEnabledState && arSession != null) {
            arSession.enabled = isEnabled;
        }
    }

}
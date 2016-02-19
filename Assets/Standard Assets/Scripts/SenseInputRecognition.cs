/********************************************************************************

INTEL CORPORATION PROPRIETARY INFORMATION This software is supplied under the 
terms of a license agreement or nondisclosure agreement with Intel Corporation 
and may not be copied or disclosed except in accordance with the terms of that 
agreement.
Copyright(c) 2011-2015 Intel Corporation. All Rights Reserved.

*********************************************************************************/
using UnityEngine;
using System.Collections;

public class SenseInputRecognition : MonoBehaviour
{
    #region Inspector Config Vars
    public enum RecognitionType
    {
        Dictation,
        CommandControl
    }
	;
    public RecognitionType mode = RecognitionType.Dictation;
    public string[] commands;
    private string cur_deviceName = "VF0800"; //Optional: F200 Device Name
    #endregion

    #region SenseInput Vars
    private PXCMSession session;
    private PXCMAudioSource source;
    private PXCMSpeechRecognition sr;
    private pxcmStatus sts;
    private PXCMSpeechRecognition.Handler handler;
    #endregion

    #region Public Events To Subscribe
    public delegate void OnRecDataDelegate(string sentence);

    public event OnRecDataDelegate OnRecData;

    public delegate void OnAlertDelegate(string alertlabel);

    public event OnAlertDelegate OnAlertData;

    public delegate void OnShutDownDelegate();

    public event OnShutDownDelegate OnShutdown;

    #endregion

    private void OnRecognition(PXCMSpeechRecognition.RecognitionData data)
    {
        if (mode == RecognitionType.CommandControl)
        {
            if (data.scores[0].confidence > 50)
                if (OnRecData != null)
                    OnRecData(data.scores[0].sentence);
        }
        else
        {
            if (OnRecData != null)
                OnRecData(data.scores[0].sentence);
        }
    }

    private void OnAlert(PXCMSpeechRecognition.AlertData data)
    {
        if (OnAlertData != null)
            OnAlertData(data.label.ToString());
    }

    // Use this for initialization
    void Start()
    {
        /* Create a session */
        session = PXCMSession.CreateInstance();
        if (session == null)
            Debug.LogError("Failed to create a session");

        /* Retrieve an Audio Source from current session */
        source = session.CreateAudioSource();

        /* Optional: Select a specific device */
        source.ScanDevices();
        bool isDeviceSet = false;
        PXCMAudioSource.DeviceInfo dinfo = null;
        Debug.Log("Found " + source.QueryDeviceNum() + " input devices.");

        for (int d = 0; d < source.QueryDeviceNum(); d++)
        {
            sts = source.QueryDeviceInfo(d, out dinfo);
            if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR)
                continue;

            if (dinfo.name.Contains(cur_deviceName))
            {
                sts = source.SetDevice(dinfo);
                if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR)
                {
                    Debug.LogError("Failed to Set Device: " + sts);
                }
                else
                {
                    isDeviceSet = true;
                    Debug.Log("Selected F200 Input: " + dinfo.name);
                }
                break;
            }
        }

        /* If F200 not found select last found Input device */
        if (!isDeviceSet)
        {
            sts = source.SetDevice(dinfo);
            if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR) Debug.LogError("Failed to Set Device: " + sts);
            else Debug.Log("Selected: " + dinfo.name);
        }
        /* Create a Speech Recognition Instance */
        sts = session.CreateImpl<PXCMSpeechRecognition>(out sr);
        if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR)
            Debug.LogError("Failed to create a Speech Recognition Instance: " + sts);

        /* Configure the Speech Recognition Module */
        PXCMSpeechRecognition.ProfileInfo pinfo;
        sts = sr.QueryProfile(out pinfo);
        if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR)
            Debug.LogError("Failed to retrieve a Speech Recognition Profile: " + sts);
        pinfo.language = PXCMSpeechRecognition.LanguageType.LANGUAGE_US_ENGLISH;
        sr.SetProfile(pinfo);
        if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR)
            Debug.LogError("Failed to set language Profile: " + sts);

        /* Set handler: OnRecognition & OnAlert */
        handler = new PXCMSpeechRecognition.Handler();
        handler.onRecognition = OnRecognition;
        handler.onAlert = OnAlert;

        /* Set Recognition Mode Type */
        switch (mode)
        {
            case RecognitionType.Dictation:
                sts = sr.SetDictation(); // Set Dictation Mode
                if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR)
                    Debug.LogError("Failed to set Dictation Mode: " + sts);
                break;
            case RecognitionType.CommandControl:
                if (commands.Length == 0)
                {
                    Debug.LogError("Grammar list is Empty");
                    return;
                }

                sts = sr.BuildGrammarFromStringList(1, commands, null); // Build the grammar
                if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR)
                    Debug.LogError("Failed to Build Grammar from list: " + sts);

                sts = sr.SetGrammar(1); // Set active grammar
                if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR)
                    Debug.LogError("Failed to set Grammar: " + sts);
                break;
        }

        /* Start recognition */
        sts = sr.StartRec(source, handler); // for default device: source = null
        if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR)
            Debug.LogError("Failed to Start Recognition: " + sts);
    }

    // Use this for Cleap Up
    void OnDisable()
    {
        /* Inform Subscribers */
        if (OnShutdown != null)
            OnShutdown();

        /* Stop the session */
        if (sr != null)
        {
            // Stop recognition
            sr.StopRec();

            // Destroy the Speech Recognition Instance
            sr.Dispose();
        }

        /* Destroy the session */
        if (session != null)
            session.Dispose();
    }

    #region Singleton Code
    private static SenseInputRecognition _instance;

    public static SenseInputRecognition Instance
    {
        get
        {
            if (!_instance)
                _instance = (SenseInputRecognition)GameObject.FindObjectOfType(typeof(SenseInputRecognition));
            return _instance;
        }
    }

    void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }
    #endregion
}

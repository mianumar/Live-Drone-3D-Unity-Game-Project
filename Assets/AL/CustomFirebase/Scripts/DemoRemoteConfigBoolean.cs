using System.Collections;
using System.Collections.Generic;
using AL.CustomFirebase.Scripts;
using UnityEngine;

public class DemoRemoteConfigBoolean : MonoBehaviour
{
    public UnityEngine.UI.Text outputText;
    public UnityEngine.UI.InputField input;


    public void GetBoolean()
    {
        if(FirebaseRemoteController.Instance)
            outputText.text="Value: "+FirebaseRemoteController.Instance.GetBoolean(input.text).ToString();
    }
}
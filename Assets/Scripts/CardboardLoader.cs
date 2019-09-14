using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class CardboardLoader : MonoBehaviour
{
    public static bool vr = false;

    void Start()
    {
        if (!vr)
        {
            StartCoroutine(LoadDevice("cardboard"));
            vr = true;
        }
    }

    IEnumerator LoadDevice(string newDevice)
    {
        XRSettings.LoadDeviceByName(XRSettings.supportedDevices[1]);
        yield return null;
        XRSettings.enabled = true;
    }
}
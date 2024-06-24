using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
public class LocationManager : MonoBehaviour
{
    // Default Bandung's Lat-Long in degree
    public float latitude = -6.914744f;
    public float longitude = 107.609810f;
    [SerializeField] private TMP_InputField latitudeUI;
    [SerializeField] private TMP_InputField longitudeUI;
    public UnityEvent<float, float> onLocationChanged;
    private void Start()
    {
        latitudeUI.text = $"{latitude}";
        longitudeUI.text = $"{longitude}";
    }

    public void ResetLatLongToDefault()
    {
        SetLat("-6.914744");
        SetLong("107.609810");
    }

    public void SetLat(string latString)
    {
        latitude = float.Parse(latString);
        latitudeUI.text = $"{latitude}";
        onLocationChanged.Invoke(latitude, longitude);
    }

    public void SetLong(string longString)
    {
        longitude = float.Parse(longString);
        longitudeUI.text = $"{longitude}";
        onLocationChanged.Invoke(latitude, longitude);
    }
 
    [ContextMenu("Text Change Location")]
    public void TestChangeLocation()
    {
        SetLong("50");
    }
}

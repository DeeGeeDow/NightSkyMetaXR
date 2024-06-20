using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LocationManager : MonoBehaviour
{
    // Default Bandung's Lat-Long in degree
    public float latitude = -6.914744f;
    public float longitude = 107.609810f;
    [SerializeField] private TMP_InputField latitudeUI;
    [SerializeField] private TMP_InputField longitudeUI;

    private void Start()
    {
        ResetLatLongToDefault();
    }

    public void ResetLatLongToDefault()
    {
        SetLat("-6.914744");
        SetLong("107.609810");
    }

    public void SetLat(string latString)
    {
        latitude = float.Parse(latString);
        latitudeUI.text = latitude.ToString();
    }

    public void SetLong(string longString)
    {
        longitude = float.Parse(longString);
        longitudeUI.text = longitude.ToString();
    }
}

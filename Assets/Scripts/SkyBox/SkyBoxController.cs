using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxController : MonoBehaviour
{
    public LocationManager LocationManager;
    public TimeManager TimeManager;

    private void Start()
    {
        //RotateSkyBased((float)TimeManager.Lst, LocationManager.latitude);
        
    }
    public void Update()
    {
        RotateEveryTime((float)TimeManager.Lst, LocationManager.latitude);
    }

    void RotateSkyBased(float lst, float lat)
    {
        transform.Rotate(0, 0, lst);
        transform.Rotate(-(90+lat), 0, 0);
    }

    void RotateEveryTime(float lst, float lat)
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        RotateSkyBased(lst, lat);
    }
}

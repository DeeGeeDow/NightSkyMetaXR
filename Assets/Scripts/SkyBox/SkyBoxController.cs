using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxController : MonoBehaviour
{
    public LocationManager LocationManager;
    public TimeManager TimeManager;
    public bool isTextureFlipped = false;
    private Material material;

    // Get Milkyway texture https://svs.gsfc.nasa.gov/4851/, and then set material shader to Skybox/Cubemap
    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }
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
        if (!isTextureFlipped) transform.Rotate(0, 0, 180);
        transform.Rotate(0, -lst, 0, Space.World);
        transform.Rotate(-(90 + lat), 0, 0, Space.World);
    }

    void RotateEveryTime(float lst, float lat)
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        RotateSkyBased(lst, lat);
    }

    public void SetIntensity(float maxMagnitude)
    {
        float exposure = Mathf.Pow(2.5f, (maxMagnitude - 8)/2);
        material.SetFloat("_Exposure", exposure);
    }
}

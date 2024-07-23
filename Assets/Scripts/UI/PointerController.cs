using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerController : MonoBehaviour
{
    public Star star;
    private bool _isShowing = false;
    public StarField StarField;
    public TimeManager TimeManager;
    public LocationManager LocationManager;
    public int starIndex;

    [Header("Children Control")]
    public GameObject pointer;
    public GameObject direction;
    public bool IsShowing
    {
        get => _isShowing;
        set
        {
            _isShowing = value;
            if (value)
            {
                if (StarField.cullResults[starIndex] == Util.CullFlags.CULLED)
                {
                    direction.SetActive(true);
                    pointer.SetActive(false);
                }
                else
                {
                    direction.SetActive(false);
                    pointer.SetActive(true);
                }
            }
            else
            {
                direction.SetActive(false);
                pointer.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (IsShowing)
        {
            if (star.mag < StarField.maxMagnitudeVisible) star.CalculatePosition((float)TimeManager.Lst, LocationManager.latitude);
            Vector3 starViewportPos = Camera.main.WorldToViewportPoint(star.FieldPosition*StarField.starFieldScale);
            if (starViewportPos.x <= 0 || starViewportPos.x >= 1 || starViewportPos.y <= 0 || starViewportPos.y >= 1 || starViewportPos.z < 0)
            {
                direction.SetActive(true);
                pointer.SetActive(false);
                float k = 0.95f;
                Vector3 origin = Camera.main.transform.forward * StarField.starFieldScale;
                Vector3 targetPointing = star.FieldPosition * StarField.starFieldScale;
                Vector3 pointingDirection = (targetPointing - origin).normalized;
                transform.position = DirectionOnViewport(origin, targetPointing, k);
                transform.LookAt(Vector3.zero, pointingDirection);
            }
            else
            {
                direction.SetActive(false);
                pointer.SetActive(true);
                transform.position = star.FieldPosition * StarField.starFieldScale;
                transform.LookAt(Vector3.zero);
                transform.Rotate(Vector3.up, 90);
            }

        }
    }

    Vector3 DirectionOnViewport(Vector3 origin, Vector3 target, float k)
    {
        Vector3 originViewport = Camera.main.WorldToViewportPoint(origin);
        Vector3 targetViewport = Camera.main.WorldToViewportPoint(target);
        Vector3 directionPointViewport = Vector3.MoveTowards(originViewport, targetViewport, k); 
        directionPointViewport.z = StarField.starFieldScale/1.01f;
        Vector3 directionWorld = Camera.main.ViewportToWorldPoint(directionPointViewport);
        return directionWorld;
    }
}

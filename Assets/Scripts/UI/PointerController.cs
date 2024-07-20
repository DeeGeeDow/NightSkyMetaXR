using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerController : MonoBehaviour
{
    public Star star;
    private bool _isShowing = false;
    public StarField StarField;
    public bool IsShowing
    {
        get => _isShowing;
        set
        {
            _isShowing = value;
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(value);
            }
        }
    }

    private void Update()
    {
        if (IsShowing)
        {
            transform.position = star.FieldPosition * StarField.starFieldScale;
            transform.LookAt(Vector3.zero);
            transform.Rotate(Vector3.up, 90);
        }
    }
}

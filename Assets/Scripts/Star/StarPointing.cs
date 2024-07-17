using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class StarPointing : MonoBehaviour
{
    private LineRenderer pointingLine;
    public Transform pointTransform;
    public bool isPointing = true;
    public OVRSkeleton skeleton;
    public OVRHand hand;
    public Ray ray;
    private void Start()
    {
        pointingLine = GetComponent<LineRenderer>();
        pointingLine.startWidth = 0.005f;
        pointingLine.endWidth = 1f;
        ray = new Ray();
    }

    private void Update()
    {
        if (isPointing)
        {
            Vector3 startPosition = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.position;
            Vector3 direction = hand.GetPointerRayTransform().forward.normalized;
            //Vector3 direction = (skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.position - skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index1].Transform.position).normalized;
            pointingLine.SetPosition(0, startPosition);
            pointingLine.SetPosition(1, startPosition + direction * 400);
            ray.origin = startPosition;
            ray.direction = direction;
        }
    }
    public void Activate()
    {
        isPointing = true;
        pointingLine.enabled = true;
    }

    public void Deactivate()
    {
        isPointing = false;
        pointingLine.enabled = false;
    }

    
}

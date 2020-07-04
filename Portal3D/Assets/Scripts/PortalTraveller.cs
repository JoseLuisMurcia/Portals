﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour
{

    public Vector3 previousOffsetFromPortal;

    public virtual void Teleport(Transform originPortal, Transform destPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    public virtual void EnterPortalThreshold()
    {

    }

    public virtual void ExitPortalThreshold()
    {

    }
}

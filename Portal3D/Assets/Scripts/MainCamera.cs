using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    Portal[] portals;

    void Start()
    {
        portals = FindObjectsOfType<Portal>();
    }

    private void OnPreCull()
    {
        /*
        foreach (Portal portal in portals)
            portal.PrePortalRender(); */

        foreach (Portal portal in portals)
        {
            portal.Render();
        } 

        /*
        foreach (Portal portal in portals)
            portal.PostPortalRender(); */
    }
}

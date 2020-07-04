using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    [SerializeField] Portal otherPortal;
    [SerializeField] MeshRenderer screen;
    public Camera playerCam;
    public Camera portalCam;
    public RenderTexture viewTexture;

    void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
    }

    void CreateViewTexture()
    {
        if(viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            if (viewTexture != null) viewTexture.Release();

            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            /* Render the view from the portal camera to the viewTexture */
            portalCam.targetTexture = viewTexture;
            /* Display the view texture on the screen of the other portal */
            otherPortal.screen.material.mainTexture = viewTexture;
        }
    }

    /* Called before the player camera is rendered */
    public void Render()
    {
        screen.enabled = false;
        CreateViewTexture();

        /* Equal the relative position and rotation of this portal and the other */
        Matrix4x4 m = transform.localToWorldMatrix * otherPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        /* Render the camera */
        portalCam.Render();
        screen.enabled = true;

    }

}

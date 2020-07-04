using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public List<PortalTraveller> trackedTravellers = new List<PortalTraveller>();
    [SerializeField] Portal otherPortal;
    [SerializeField] MeshRenderer screen;
    Camera playerCam;
    Camera portalCam;
    RenderTexture viewTexture;

    void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
    }

    private void LateUpdate()
    {
        for(int i=0; i<trackedTravellers.Count; i++)
        {
            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerTransform = traveller.transform;
            Vector3 offsetFromPortal = travellerTransform.position - transform.position;
            
            int portalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
            int portalSideOld = System.Math.Sign(Vector3.Dot(traveller.previousOffsetFromPortal, transform.forward));

            if(portalSide != portalSideOld)
            {
                Debug.Log("PLAYER VA A VIAJAR");
                Matrix4x4 m = otherPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerTransform.localToWorldMatrix;
                traveller.Teleport(transform, otherPortal.transform, m.GetColumn(3), m.rotation);

                otherPortal.OnTravellerEnterPortal(traveller);
                trackedTravellers.RemoveAt(i);
                i--;
            }
            else
            {
                traveller.previousOffsetFromPortal = offsetFromPortal;
            }
        }
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
        if(!VisibleFromCamera(otherPortal.screen, playerCam))
        {
            Texture2D testTexture = new Texture2D(1, 1);
            testTexture.SetPixel(0, 0, Color.red);
            testTexture.Apply();
            otherPortal.screen.material.mainTexture = testTexture;
            return;
        }
        otherPortal.screen.material.mainTexture = viewTexture;
        screen.enabled = false;
        CreateViewTexture();
        
        /* Equal the relative position and rotation of this portal and the other */
        Matrix4x4 m = transform.localToWorldMatrix * otherPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        /* Render the camera */
        portalCam.Render();
        screen.enabled = true;

    }

    static bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    void OnTravellerEnterPortal(PortalTraveller traveller)
    {
        if (!trackedTravellers.Contains(traveller))
        {
            
            traveller.EnterPortalThreshold();
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            trackedTravellers.Add(traveller);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PortalTraveller traveller = other.GetComponent<PortalTraveller>();
        if (traveller) OnTravellerEnterPortal(traveller);
    }

    private void OnTriggerExit(Collider other)
    {
        PortalTraveller traveller = other.GetComponent<PortalTraveller>();
        if(traveller && trackedTravellers.Contains(traveller))
        {
            traveller.ExitPortalThreshold();
            trackedTravellers.Remove(traveller);
        }
    }

}

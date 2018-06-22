using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Player : MonoBehaviour {

    [SerializeField]
    private Camera cam;

    public IEnumerator ScreenZoom(float intensity, int dir)
    {
        for(float i = 0f; i < 2.0; i+=.1f)
        {
            cam.fieldOfView -= .1f * intensity * dir;
            yield return new WaitForSeconds(.005f);
        }
        for(float i = 0f; i < 2.0; i += .1f)
        {
            cam.fieldOfView += .1f * intensity * dir;
            yield return new WaitForSeconds(.005f);
        }
        ResetCamPosition();
    }

    void ResetCamPosition()
    {
        cam.transform.localPosition = new Vector3(0, 0, 0);
    }
}

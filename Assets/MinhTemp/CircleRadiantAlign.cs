using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Utility;

public class CircleRadiantAlign : MonoBehaviour
{
    public Camera Cam;
    public GameObject Quad;

    private void Start()
    {
        float maxSize = ((int)BoardSize.Large + 1) * Constants.DELTA;
        float orthoSize = maxSize * 0.5f;
        Cam.orthographicSize = orthoSize;

        Quad.transform.localScale = new Vector3(orthoSize * 2, orthoSize * 2, orthoSize *2);
    }
}

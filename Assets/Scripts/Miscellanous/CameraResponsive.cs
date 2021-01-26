using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Connexion.Utility;

namespace Connexion.Miscellaneous
{
    public class CameraResponsive : MonoBehaviour
    {
        private void Start()
        {
            float maxSize = ((int)BoardSize.Large + 1) * Constants.DELTA;
            float orthoSize = maxSize * Screen.height / Screen.width * 0.5f;
            Camera.main.orthographicSize = orthoSize;

            Debug.Log(Camera.main.scaledPixelWidth + ":" + Camera.main.scaledPixelHeight);
        }
    }
}
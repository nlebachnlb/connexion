using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchColor : MonoBehaviour
{
    public LineRenderer Target;
    public Material MatTarget;

    int colorId;
    Color lastColor;

    private void Awake()
    {
        colorId = Shader.PropertyToID("_Color");
        MatTarget.SetColor(colorId, lastColor);
    }   

    private void Update()
    {
        if(lastColor != Target.startColor)
        {
            lastColor = Target.startColor;
            MatTarget.SetColor(colorId, lastColor);
        }
        
    }
}

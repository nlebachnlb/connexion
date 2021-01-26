using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineButton : MonoBehaviour
{
    private Toggle _toggle;
    [SerializeField] private Image background;
    // [SerializeField] private Image checkMark;
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (_toggle.isOn)
            background.CrossFadeAlpha(0f, 0.25f, true);
    }
    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(Animate);
    }

    private void Animate(bool isChecked)
    {
        if (isChecked)
            background.CrossFadeAlpha(0f, 0.25f, true);
        else
            background.CrossFadeAlpha(1f, 0.25f, true);
    }
}

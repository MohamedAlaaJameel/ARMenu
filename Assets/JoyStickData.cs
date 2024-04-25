using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JoyStickData : MonoBehaviour
{
    [SerializeField]
    FixedJoystick joystick;
    [SerializeField]
    TMPro.TextMeshProUGUI xTMPpro;
    [SerializeField]
    TMPro.TextMeshProUGUI yTMPpro;
    [SerializeField]
    GameObject parentXY;
    [NonSerialized]
    public float horizontalInput;
    [NonSerialized]
    public float verticalInput;
    public float inputScale = .1f;

    public void HideXY()
    {
        parentXY.SetActive(false);
    }
    public void ShowXY()
    {
        parentXY.SetActive(true);
    }
    void Update()
    {
        // Accumulate joystick input over time
        horizontalInput += joystick.Horizontal * inputScale * Time.deltaTime;
        verticalInput += joystick.Vertical * inputScale * Time.deltaTime;

        bool isJoystickActive = Mathf.Abs(joystick.Horizontal) > 0.1f || Mathf.Abs(joystick.Vertical) > 0.1f;

        if (parentXY.activeSelf != isJoystickActive)
        {
            parentXY.SetActive(isJoystickActive);
        }
        if (isJoystickActive)
        {
            xTMPpro.text = horizontalInput.ToString("F2");
            yTMPpro.text = verticalInput.ToString("F2");
        }
    }
    public void ResetData()
    {
        horizontalInput = 0;
        verticalInput = 0;

    }
}

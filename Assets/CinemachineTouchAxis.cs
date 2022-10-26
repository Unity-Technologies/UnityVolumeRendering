using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineTouchAxis : MonoBehaviour
{
    public float TouchSensitivity_x = 10f;
    public float TouchSensitivity_y = 10f;
 
    // Use this for initialization
    void Start () {
        CinemachineCore.GetInputAxis = HandleAxisInputDelegate;
    }
     
    float HandleAxisInputDelegate(string axisName)
    {
        switch(axisName)
        {
 
            case "Mouse X":
 
                if (Input.touchCount>0)
                {
                    return Input.touches[0].deltaPosition.x / TouchSensitivity_x;
                }else{
                    return Input.GetAxis(axisName);
                }
 
            case "Mouse Y":
                if (Input.touchCount > 0)
                {
                    return Input.touches[0].deltaPosition.y / TouchSensitivity_y;
                }
                else
                {
                    return Input.GetAxis(axisName);
                }
 
            default:
                Debug.LogError("Input <"+axisName+"> not recognyzed.",this);
                break;
        }
 
        return 0f;
    }
}

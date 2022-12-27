using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamParallax : MonoBehaviour
{
    public float camSpeed;
    private void FixedUpdate()
    {
        Vector2 screenRes = new Vector2(Screen.width, Screen.height);
        Vector2 change = ((Vector2)Input.mousePosition - screenRes / 2) / screenRes * 10f;
        transform.localPosition = Vector2.Lerp(transform.localPosition, change * -2f, camSpeed);
        transform.localEulerAngles = new Vector2(Mathf.LerpAngle(transform.localEulerAngles.x, -change.y, camSpeed), Mathf.LerpAngle(transform.localEulerAngles.y, change.x, camSpeed));
    }
}

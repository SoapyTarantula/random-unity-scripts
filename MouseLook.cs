using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mSens = 100f;
    public Transform pBody;

    float _xRot;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mSens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mSens * Time.deltaTime;

        //print(mouseX + ", " + mouseY); // Just seeing if this is reading correctly.

        pBody.Rotate(Vector3.up * mouseX);
        _xRot -= mouseY;
        _xRot = Mathf.Clamp(_xRot, -70f, 90f);
        transform.localRotation = Quaternion.Euler(_xRot,0f,0f);
    }
}

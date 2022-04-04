using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// A really basic and bad camera controller that is used for toggling between first and third person perspectives with Cinemachine.
// Set the cameras to some empty object or something and handle all the dumb Cinemachine follows & look-ats or something, I dunno man.
// Probably should figure out a way to assign the cameras to their respective fields from within the script instead of doing it in the inspector.

// Poorly made by SoapyTarantula | https://github.com/SoapyTarantula | https://twitter.com/soapytarantula

// REQUIRES NAMESPACES:
// using UnityEngine;
// using Cinemachine;

public class CameraSwitching : MonoBehaviour
{
    private Controls controls;
    private bool _3pp = true;

    [SerializeField] CinemachineVirtualCamera _firstPersonCamera;
    [SerializeField] CinemachineVirtualCamera _thirdPersonCamera;

    private void Awake()
    {
        controls = new Controls();
    }

    // OnEnable() and OnDisable are required by the new input system.
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        bool _switch = controls.Movement.SwitchCamera.triggered;
        if(_switch) { SwitchCamera(); }
    }

    // Added the null checks because it would throw errors but still work otherwise.
    private void SwitchCamera()
    {
        if (_thirdPersonCamera == null || _firstPersonCamera == null)
        {
            return;
        }
        else if (_3pp)
        {
            _thirdPersonCamera.Priority = 0;
            _firstPersonCamera.Priority = 1;
        }
        else
        {
            _thirdPersonCamera.Priority = 1;
            _firstPersonCamera.Priority = 0;
        }
        _3pp = !_3pp;
      }
}

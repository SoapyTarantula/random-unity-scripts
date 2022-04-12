using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Only really works if you're using a Directional Light, but I hope that would be obvious.
// Attach the script directly to the light. Or don't, I'm not your dad.

// Poorly made by SoapyTarantula | https://github.com/SoapyTarantula | https://twitter.com/soapytarantula

public class TimeOfDay : MonoBehaviour
{
    [SerializeField] GameObject _sun; // The directional light this script is probably attached to.
    [SerializeField] float _lowerLimit = -30f, _upperLimit = 30f; // The lower and upper limit of the sun position, negative values mean its darker more often. Defaults to -30f and 30f.

    void Start()
    {
        _sun = this.gameObject; // Comment this line out if you are not attaching this script to the light source.
        RandomizeTimeOfDay();
    }

    // Uses Random.Range to change the sun position at start.
    void RandomizeTimeOfDay()
    {
        _sun.transform.Rotate(new Vector3(Random.Range(_lowerLimit, _upperLimit), 0, 0));
    }
}

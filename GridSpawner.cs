using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A very barebones grid spawner.
// It can spawn in 2D or 3D depending on a bool.
// Also allows you to destroy & remake the grid after a delay, useful for vanishing platforms.
// By default this spawns the grid around the world origin (0,0,0) vector. Could be modified to spawn elsewhere, but that's effort and I have covid.

// Poorly made by SoapyTarantula | https://github.com/SoapyTarantula | https://twitter.com/soapytarantula

public class GridSpawner : MonoBehaviour
{
     // BE VERY CAUTIOUS IF YOU SET THIS TO TRUE WITHOUT ALSO ENABLING _destroyAfterTime!! If done you will repeatedly create objects that won't be destroyed automatically!
    [SerializeField] bool _callRepeatedly; // If enabled it causes the script to run multiple times, delayed by the float below.
    [SerializeField] bool _destroyAfterTime; // If enabled the objects will destroy themselves after the time set below.

    // Really make sure you understand the above warning.

    [SerializeField] bool _is3D; // Changes the vector direction of the instantiated objects depending on if you are doing a 2D (x & y) or a 3D (x & z) grid.
    [SerializeField] GameObject _object; // The gameobject you want to instantiate.
    [SerializeField] Transform _parentObject; // The parent object of the instantiated objects, important to have otherwise you are going to have a ton of clutter.
    [SerializeField] int _xAxisCount, _yOrZAxisCount; // The amount of objects to instantiate in the x & y/z axes.
    [SerializeField] float _timeBetweenCalls = 15f; // Time to delay the script call in seconds. Defaults to 15f because we cannot use % with 0;
    [SerializeField] float _timeBeforeDestroy = 10f; // see line above, defaults to 10f.
    [SerializeField] float _timeBetweenSpawns = 1f; // Time in seconds between each cycle of instantiate.

    private GameObject _instantiatedObject; // A reference to the object we're instantiating so we can do stuff with it after.

    private void Start()
    {
        if (_callRepeatedly) // If we're calling this repeatedly we do not need to handle this in Start(), so we skip it.
        {
            return;
        }
        else // Otherwise we have Start() call the coroutine for the only time it should fire.
        {
            StartCoroutine(nameof(Spawn));
        }
    }

    // I run this in FixedUpdate for timing consistency, but you could just slap the call anywhere.
    private void FixedUpdate()
    {
        if (_callRepeatedly)
        {
            if ((Time.time % _timeBetweenCalls) == 0)
            {
                StartCoroutine(nameof(Spawn));
            }
        }
    }

    // Spawns things in a grid pattern
    IEnumerator Spawn()
    {
        for (int i = 0; i < _xAxisCount; i++)
        {
            for (int v = 0; v < _yOrZAxisCount; v++)
            {
                if (_is3D) // If we're set to 3D then we change the y  vector axis to the z axis, so we spawn flatly.
                {
                    _instantiatedObject = Instantiate(_object, Vector3.zero + new Vector3(i, 0, v), Quaternion.identity) as GameObject;
                }
                else // Otherwise we spawn on the x & y.
                {
                    _instantiatedObject = Instantiate(_object, Vector3.zero + new Vector3(i, v, 0), Quaternion.identity) as GameObject;
                }

                if (_parentObject != null)
                {
                    _instantiatedObject.transform.SetParent(_parentObject); // Parents the spawned objects to something you've hopefully set in the inspector, if not the spawned objects will not be parented to anything. This is used to avoid clutter in your hierarchy.
                } 
                yield return new WaitForSeconds(_timeBetweenSpawns); // Waits an amount of time before rerunning the script. Can be used to create a nice effect.

                if (_destroyAfterTime) // Destroys the objects after a set time if we have the boolean enabled.
                {
                    Destroy(_instantiatedObject, _timeBeforeDestroy);
                }
            }
        }
    }
}

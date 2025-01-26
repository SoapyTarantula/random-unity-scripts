// Attach this script to a gameobject, it'll slap a light component onto it automatically and cause it to flicker when an enemy is near.

using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light _light;
    private Transform _entity;
    [SerializeField] float defaultPower, lowestPower;
    void Start()
    {
        _entity = GameObject.FindGameObjectWithTag("Enemy").transform; // probably performance intensive if there are a lot of enemies, but fine for a handful or less.
        if (_light == null) { _light = GetComponent<Light>(); }
    }
    void LateUpdate()
    {
        if (_entity != null && _light != null)
        {
            if (_light.enabled)
            {
                if (Vector3.Distance(transform.position, _entity.position) < 20f)
                {
                    _light.intensity += Random.Range(-25f, 10f);
                }
            }
            _light.intensity = Mathf.Clamp(_light.intensity, lowestPower, defaultPower);
        }
        else { _light.intensity = defaultPower; }
    }
}

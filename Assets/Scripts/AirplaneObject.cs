using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AirplaneObject : ScriptableObject
{
    public string aircraftType;
    public string company;
    public GameObject airplaneObject;
    public Vector3 hangarPosition;
    public AirplaneController pilot;
    public Vector2 airportBounds;


    private void Start () 
    {
        pilot = airplaneObject.GetComponent<AirplaneController>();
    }

    public void SwitchRoam()
    {
        /// pass roam-switch from gameController to pilot
        pilot = airplaneObject.GetComponent<AirplaneController>();
        pilot.SwitchRoam(airportBounds);
    }

    public void MoveToHangar()
    {
        // get pilot
        pilot = airplaneObject.GetComponent<AirplaneController>();
        pilot.MoveToHangar(hangarPosition);
    }


    public void LightSwitch()
    {
        /// pass lightswitch from scenecontroller to plane
        foreach (var planeLight in airplaneObject.GetComponentsInChildren<Light>())
        {
            planeLight.enabled = !planeLight.enabled;
        }
    }

    private void OnDestroy() 
    {
        /// make sure gameobject is destroyed when AirplaneObject is destroyed
        DestroyImmediate(airplaneObject);
    }

}

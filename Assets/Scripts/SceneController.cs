using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using UnityEngine.AI;
using TMPro;
using UnityEngine.Events;


public class SceneController : MonoBehaviour
{
    [Header("Airport GameObjects")]
    public NavMeshSurface navMeshSurface;
    public GameObject Fence;
    public GameObject Hangar;
    public GameObject PlanePrefab;
    public GameObject FloorSurface;
    public TextMeshProUGUI LandedText;
    
    [Header("Airport Parameters")]
    [Range(30,80)]
    public int fenceSizeX = 30;
    [Range(30, 80)]
    public int fenceSizeZ = 30;
    [Range(3, 20)]
    public int numPlanes = 10;

    private int numPlanesParked = 0;

    private List<GameObject> fenceList = new List<GameObject>();
    private List<AirplaneObject> planeList = new List<AirplaneObject>();
    private List<GameObject> hangarList = new List<GameObject>();
    private List<string> typeList = new List<string>{"Fokker F.VII", "Fokker F-10", "Fokker D.XVII", "Fokker D.XXI"};

    public void Lights() 
    {
        /// switch lights of the plane
        for (int i = 0; i < planeList.Count; i++)
        {
            planeList[i].LightSwitch();
        }
    }

    public void PlaneParkedEvent()
    {
        numPlanesParked += 1;

        if (numPlanesParked == numPlanes)
        {
            StartCoroutine(ShowParkedMessage());
            numPlanesParked = 0;
        }
    }

    IEnumerator ShowParkedMessage () 
    {
        LandedText.text = "All planes parked";
        yield return new WaitForSeconds(3);
        LandedText.text = "";
    }

    public void Roam() 
    {
        /// set plane pilots to roaming mode
        for (int i = 0; i < planeList.Count; i++)
        {
            planeList[i].SwitchRoam();
        }
    }

    public void MoveToHangar() 
    {
        /// set plane pilots to roaming mode
        for (int i = 0; i < planeList.Count; i++)
        {
            planeList[i].MoveToHangar();
        }
    }

    private void GenerateAirport()
    {
        GenerateFloorSurface();
        GenerateFence();
        GenerateHangarsPlanes();
    }


    [Button]
    private void Reset() 
    {
        // destroy all objects first
        if (fenceList.Count > 0)
        {
            DestroyObjects();
        }

        GenerateAirport();

        // update (bake) navmesh at runtime
        navMeshSurface.BuildNavMesh();
    }

    private void GenerateFloorSurface()
    {
        // set surface to center of cube
        FloorSurface.transform.position = new Vector3(Mathf.Ceil(fenceSizeX / 2), 0, Mathf.Ceil(fenceSizeZ / 2));
        FloorSurface.transform.localScale = new Vector3(fenceSizeX + 2, 0.1f, fenceSizeZ + 2);
    }

    
    private void GenerateFence()
    {
        /// generates a rectangular fence from fenceSizeZ and fenceSizeX
        for (int x = 0; x < fenceSizeX; x++)
        {
            fenceList.Add(Instantiate(Fence, new Vector3 (x, 0, 0), Quaternion.identity, gameObject.transform));
            fenceList.Add(Instantiate(Fence, new Vector3 (x, 0, fenceSizeZ), Quaternion.identity, gameObject.transform));
        }
        for (int z = 0; z < fenceSizeZ; z++)
        {
            fenceList.Add(Instantiate(Fence, new Vector3 (-1, 0, z), Quaternion.Euler(0, 90, 0),gameObject.transform));
            fenceList.Add(Instantiate(Fence, new Vector3 (fenceSizeX - 1, 0, z), Quaternion.Euler(0, 90, 0), gameObject.transform));
        }
    }


    private void GenerateHangarsPlanes()
    {
        ///  spawn hangars with planes on random locations within perimeter
        for (int i = 0; i < numPlanes; i++)
        {
            // choose random location
            Vector3 position = new Vector3(Random.Range(5, fenceSizeX - 5), 0, Random.Range(5, fenceSizeZ - 5));
            // spawn hangar
            hangarList.Add(Instantiate(Hangar, position, Quaternion.identity, gameObject.transform));
            hangarList[i].GetComponent<TextMeshPro>().text = "Hangar " + i;

            // create plane and instantiate with characteristics
            AirplaneObject plane = ScriptableObject.CreateInstance<AirplaneObject>();
            plane.company = "KLM";
            plane.aircraftType = typeList[Random.Range(0,typeList.Count)];
            plane.hangarPosition = position + new Vector3(0, 0, 0);
            plane.airportBounds = new Vector2(fenceSizeX, fenceSizeZ);
            plane.airplaneObject = Instantiate(PlanePrefab, position + new Vector3(0,0,-1), Quaternion.Euler(0, 180, 0), gameObject.transform);
            plane.airplaneObject.GetComponent<TextMeshPro>().text = "Airplane " + i;

            planeList.Add(plane);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        // always destroy object before starting
        if (planeList.Count > 0)
            DestroyObjects();
  
        Reset();
    }

    private void OnDestroy() 
    {
        // At game-end, destroy objects
        Debug.Log("OnDestroy");
        DestroyObjects(); 
    }

    private void DestroyObjects() 
    {
        /// destroys all objects within the airplane object
        for (int i = 0; i < fenceList.Count; i++)
        {
            Destroy(fenceList[i]);
        }

        for (int i = 0; i < planeList.Count; i++)
        {
            Destroy(planeList[i]);
            // destroyimmediate because the navmesh leaves a residue
            DestroyImmediate(hangarList[i]);
        }

        // clear the lists as well
        fenceList.Clear();
        planeList.Clear();
        hangarList.Clear();
    }
}

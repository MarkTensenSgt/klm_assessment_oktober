using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AirplaneController : MonoBehaviour
{

    public NavMeshAgent agent;
    public Vector3 targetPosition;
    public bool roam = false;
    public bool movingToHangar = false;
    public Vector2 airportBounds;

    private void MoveToNewTarget()
    {
        // set new destination  
        targetPosition = new Vector3(Random.Range(0, airportBounds.x), 0, Random.Range(0, airportBounds.y));
        // and go there
        agent.SetDestination(targetPosition);
    }

    private void StopMoving()
    {
        // set new destination  
        targetPosition = gameObject.transform.position;
        // and go there
        agent.SetDestination(targetPosition);
    }

    public void MoveToHangar(Vector3 hangerPosition)
    {
        /// moves plane to hangar
        roam = false;
        agent.SetDestination(hangerPosition);
        movingToHangar = true;
    }

    public void SwitchRoam(Vector2 airportLimit)
    {
        /// switched agent to roaming mode, or to stationary
        airportBounds = airportLimit;

        if (roam == false){
            MoveToNewTarget();
            roam = true;
        }
        else{
            StopMoving();
            roam = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (roam == true)
        {
            // change course when destination reached or after 1000 frames
            if (Vector3.Distance(agent.destination, gameObject.transform.position) < 0.5 || Time.frameCount % 1000 == 0){
                MoveToNewTarget();
            }
        }  
        if (movingToHangar == true)
        {
            if(Vector3.Distance(agent.destination, gameObject.transform.position) < 0.5)
            {
                StopMoving();
                movingToHangar = false;

                // send message to sceneController that plane has landed
                SendMessageUpwards("PlaneParkedEvent");
            }
        }
    }
}

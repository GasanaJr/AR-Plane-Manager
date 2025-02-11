using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARRaycastManager))]
public class InteractionManager : MonoBehaviour
{
    ///<summary>
    /// This is going to be the prefab to place on the plane on touch
    ///</summary>
    [SerializeField] GameObject objPrefab;
    ///<summary>
    /// This is going to be the instatied object from the prefab
    ///</summary>
    GameObject spawnedObj;
    ARRaycastManager aRRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        // Check for existing touch
        if (Input.touchCount == 0) return;

        Debug.Log("Touch detected!"); 

        // Check if raycast hit any objects
        if (aRRaycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
        {
            Debug.Log("Raycast hit a plane!"); 

            var hitPose = hits[0].pose;

            if (spawnedObj == null)
            {
                spawnedObj = Instantiate(objPrefab, hitPose.position, hitPose.rotation);
                Debug.Log("Spawned object at: " + hitPose.position);
            }
            else
            {
                spawnedObj.transform.position = hitPose.position;
                spawnedObj.transform.rotation = hitPose.rotation;
            }

            Vector3 lookPos = Camera.main.transform.position - spawnedObj.transform.position;
            lookPos.y = 0;
            spawnedObj.transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }

}

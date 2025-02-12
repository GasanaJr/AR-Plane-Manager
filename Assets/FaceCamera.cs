using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform arCamera; 

    void Start()
    {

        arCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (arCamera != null)
        {

            transform.LookAt(arCamera);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }
}

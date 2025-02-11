using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class CustomSpawner : MonoBehaviour
{
    [Header("AR References")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject modelPrefab;

    [Header("UI")]
    [SerializeField] private Button[] colorButtons;

    GameObject spawnedObject;
    bool isPlaced;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    const float MinScaleDistance = 0.1f;
    const float Rotationspeed = 1f;
    private Vector2 touchStartPosition;
    private float initialTouchDistance;
    private Vector3 initialObjectScale;

    private void Start()
    {
        setupColorButtons();
    }

    private void Update()
    {
        if (Input.touchCount == 0) return;

        if (!isPlaced)
        {
            HandleObjectPlacement();
            return;
        }

        switch(Input.touchCount)
        {
            case 1:
                HandleRotation();
                break;
            case 2:
                HandleScaling();
                break;
        }
    }

    private void HandleObjectPlacement()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;
        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            spawnedObject = Instantiate(modelPrefab, hitPose.position, hitPose.rotation);
            isPlaced = true;
        }
    }

    private void HandleRotation()
    {
        Touch touch = Input.GetTouch(0);

        switch(touch.phase)
        {
            case TouchPhase.Began:
                touchStartPosition = touch.position;
                break;
            case TouchPhase.Moved:
                float changeX = touch.position.x - touchStartPosition.x;
                spawnedObject.transform.Rotate(Vector3.up, -changeX * Rotationspeed);
                touchStartPosition = touch.position;
                break;
        }
    }
    private void HandleScaling()
    {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
        {
            initialTouchDistance = Vector2.Distance(touch0.position, touch1.position);
            initialObjectScale = spawnedObject.transform.localScale;
            return;
        }

        if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
        {
            float currentTouchDistance = Vector2.Distance(touch0.position, touch1.position);

            if (initialTouchDistance < MinScaleDistance) return;

            float scaleFactor = currentTouchDistance / initialTouchDistance;
            spawnedObject.transform.localScale = initialObjectScale * scaleFactor;
        }
    }

    private void setupColorButtons()
    {
        foreach (var button in colorButtons)
        {
            Color buttonColor = button.GetComponent<Image>().color;
            button.onClick.AddListener(() => ChangeObjectColor(buttonColor));
        }
    }

    private void ChangeObjectColor(Color newColor)
    {
        if (spawnedObject == null) return;

        var renderers = spawnedObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                material.color = newColor;
            }
        }
    }

}

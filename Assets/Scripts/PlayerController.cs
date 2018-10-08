﻿using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class for player movement.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour {

    /// <summary>
    /// The NavMeshAgent associated with this gameobject.
    /// </summary>
    private NavMeshAgent _agent;

    /// <summary>
    /// Whether a controller is plugged in or not.
    /// </summary>
    private bool _joystick;

    // Use this for initialization
    private void Start() {
        _agent = GetComponent<NavMeshAgent>();
        gameObject.GetComponent<Renderer>().material.color = Color.blue; // For testing purposes

        // Check if a controller is plugged in
        if (Input.GetJoystickNames().Length > 0) {
            _joystick = true;
        }

        // Make sure that this gameobject has an enabled NavMeshAgent
        if (_agent == null || _agent.enabled == false) {
            Debug.Log("Attempted to run PlayerController script without a NavMeshAgent.");
            Destroy(this);
        }
    }

    // Update is called once per frame
    private void Update() {
        // If the right mouse button is held down
        if (Input.GetMouseButton(1)) {

            // Ray from camera to the clicked position in world space
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray collided with anything
            if (Physics.Raycast(ray, out hit, 100)) {
                // Attempt to move to the collision point
                _agent.destination = hit.point;
            }
        }

        // If there is a controller plugged in
        if (_joystick) {

            // Horizontal and vertical input values of the joystick
            float horizontal = Input.GetAxis("HorizontalAnalog");
            float vertical = Input.GetAxis("VerticalAnalog");

            // Move in the direction of the joystick
            Vector3 goal = gameObject.transform.position + new Vector3(horizontal, gameObject.transform.position.y, vertical).normalized;
            _agent.destination = goal;
            
        }

        // Position to look towards
        Vector3 lookPos = _agent.destination;
        lookPos.y = transform.position.y; // Prevents gameobject from looking up or down

        // Face towards the destination
        transform.LookAt(lookPos);
    }
}

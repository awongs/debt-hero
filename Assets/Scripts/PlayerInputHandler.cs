﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class for handling input and keybindings.
/// </summary>
public class PlayerInputHandler : MonoBehaviour {

    /// <summary>
    /// Map for keybinds.
    /// </summary>
    private Keybinds _keybinds;

    /// <summary>
    /// The NavMeshAgent associated with this gameobject.
    /// </summary>
    private NavMeshAgent _agent;

    /// <summary>
    /// Reference to the gameobject's animator.
    /// </summary>
    private SkillCaster _skillCaster;

    private bool _ableToMove;

    // Use this for initialization
    private void Start() {
        _keybinds = new Keybinds();
        _agent = transform.parent.GetComponent<NavMeshAgent>();
        _skillCaster = GetComponent<SkillCaster>();
        _ableToMove = true;
    }

    // Update is called once per frame
    private void Update() {
        // Don't accept input if the character is casting something
        if (!_skillCaster.isCasting && _ableToMove) {
            // Used for inputs that involve the mouse position
            Vector3 clickedPoint;

            // Check if the player pressed or is holding the move key
            if (Input.GetKey(_keybinds["MoveKeyboard"])) {
                if (GetClickedPoint(out clickedPoint)) {
                    transform.LookAt(new Vector3(clickedPoint.x, transform.position.y, clickedPoint.z));
                    _agent.destination = clickedPoint;
                }
            }

            // Check if the player pressed the attack key
            if (Input.GetKeyDown(_keybinds["AttackKeyboard"])) {
                if (GetClickedPoint(out clickedPoint)) {
                    transform.LookAt(new Vector3(clickedPoint.x, transform.position.y, clickedPoint.z));
                    StartCoroutine(StopMovement(0.5f)); // Stop movement
                    _skillCaster.Cast(0, 0);
                    return;
                }
            }

            // Check if the player pressed or is holding the move key
            if (Input.GetKey(_keybinds["Skill1"])) {
                _skillCaster.Cast(1, 1);
            }

            // If a controller is plugged in
            if (Input.GetJoystickNames().Length > 0) {
                // Check if the player pressed or is holding the controller attack key
                if (Input.GetKeyDown(_keybinds["AttackController"])) {
                    _agent.isStopped = true;//Stop movement
                    _agent.velocity = Vector3.zero;
                    _skillCaster.Cast(0, 0);
                }

                // Horizontal and vertical input values of the joystick
                float horizontal = Input.GetAxis("HorizontalAnalog");
                float vertical = Input.GetAxis("VerticalAnalog");

                // Check if the player is moving the joystick
                if (horizontal != 0f || vertical != 0f) {
                    // Move in the direction of the joystick
                    Vector3 goal = gameObject.transform.position + new Vector3(horizontal, gameObject.transform.position.y, vertical).normalized;
                    _agent.destination = goal;
                }
            }
        }
    }

    /// <summary>
    /// Checks if the mouse position is colliding with any gameobject's colliders.
    /// </summary>
    /// <param name="clickedPoint">A Vector3 to output to</param>
    /// <returns>Whether a collider was found or not</returns>
    private bool GetClickedPoint(out Vector3 clickedPoint) {
        // Ray from camera to the clicked position in world space
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray collided with anything
        if (Physics.Raycast(ray, out hit, 100)) {
            // Return the collision point
            clickedPoint = hit.point;
            return true;
        }

        // No collision point
        clickedPoint = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Stops movement of the player.
    /// </summary>
    /// <param name="seconds">How many seconds to stop movement for.</param>
    private IEnumerator StopMovement(float seconds) {
        _ableToMove = false;
        _agent.ResetPath();
        yield return new WaitForSeconds(seconds);
        _ableToMove = true;
    }
}

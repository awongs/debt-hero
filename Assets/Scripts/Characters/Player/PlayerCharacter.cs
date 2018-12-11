﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class for handling input and keybindings.
/// </summary>
public class PlayerCharacter : BaseCharacter {

    /// <summary>
    /// Flag for if playing on a PS4.
    /// </summary>
    public static bool OnPS4;

    /// <summary>
    /// Used for inputs that involve the mouse position
    /// </summary>
    private Vector3 _mousePosition;

    /// <summary>
    /// The current path that the agent is taking.
    /// </summary>
    private NavMeshPath _path;

    // Use this for initialization
    private void Start() {
        _path = new NavMeshPath();
        characterStats = PlayerManager.instance.GetComponent<CharacterStats>();
        characterStats.OnDeath += Die;

        characterEquipment = PlayerManager.instance.GetComponent<CharacterEquipment>();

        // Check if on PS4
        if (Application.platform == RuntimePlatform.PS4 || Input.GetJoystickNames().Length > 0) {
            OnPS4 = true;
        }
    }

    /// <summary>
    /// When the gameobject gets destoryed
    /// </summary>
    private void OnDestroy() {
        characterStats.OnDeath -= Die;
    }

    // Update is called once per frame
    private void Update() {
        animator.SetFloat("Speed", agent.velocity.magnitude); // Run animation

        // Don't accept input if the character is casting something or if in a menu screen
        if (!animatorStatus.isCasting && GameState.currentState == GameState.PLAYING) {
            // Handle mouse and keyboard input
            if (!OnPS4) {
                HandleMouseKeyboardInput();
            }
            
            // Handle controller input
            else {
                HandleControllerInput();
            }
        }

        // Force model position and rotation to stay the same
        transform.GetChild(0).position = transform.position;
        transform.GetChild(0).rotation = transform.rotation;
    }

    /// <summary>
    /// Handles all mouse and keybard input from the player.
    /// </summary>
    private void HandleMouseKeyboardInput() {
        
        // Check if the player pressed or is holding the move key
        if (Input.GetMouseButton(0)) {
            if (GetMousePosition(out _mousePosition)) {
                transform.LookAt(new Vector3(_mousePosition.x, transform.position.y, _mousePosition.z));

                // Get the closest nav mesh position
                NavMeshHit navHitPosition;
                NavMesh.SamplePosition(_mousePosition, out navHitPosition, 20f, NavMesh.AllAreas);

                // Calculate path to that location and move to it
                agent.CalculatePath(navHitPosition.position, _path);
                agent.path = _path;
            }
        }

        // Check for basic attack
        if (Input.GetButtonDown("Basic Attack") && GetMousePosition(out _mousePosition)) {
            if (skillCaster.Cast(0)) { // Attempt to attack
                return;
            }
        }

       // Check for skill casts
        if (Input.GetButtonDown("Dash") && GetMousePosition(out _mousePosition)) {
            if (skillCaster.Cast(1)) {
                return;
            }
        } else if (Input.GetButtonDown("First Skill") && GetMousePosition(out _mousePosition)) {
            if (skillCaster.Cast(2)) {
                return;
            }
        } else if (Input.GetButtonDown("Second Skill") && GetMousePosition(out _mousePosition)) {
            if (skillCaster.Cast(3)) {
                return;
            }
        } else if (Input.GetButtonDown("Third Skill") && GetMousePosition(out _mousePosition)) {
            if (skillCaster.Cast(4)) {
                return;
            }
        }
    }

    /// <summary>
    /// Handles controller input from the player.
    /// </summary>
    private void HandleControllerInput() {
        bool castedSomething = false;

        HandleControllerMovement(); // Movement with left joystick
        HandleControllerDashing();

        // Check if the player pressed or is holding the controller attack key
        if (Input.GetButtonDown("Basic Attack")) {
            if (skillCaster.Cast(0)) {
                castedSomething = true;
            }
        }

        if (Input.GetButtonDown("First Skill")) {
            if (skillCaster.Cast(2)) {
                castedSomething = true;
            }
        } else if (Input.GetButtonDown("Second Skill")) {
            if (skillCaster.Cast(3)) {
                castedSomething = true;
            }
        } else if (Input.GetButtonDown("Third Skill")) {
            if (skillCaster.Cast(4)) {
                castedSomething = true;
            }
        }

        // Additional procedures after casting a skill
        if (castedSomething) {
            agent.ResetPath();
        }
    }

    /// <summary>
    /// Makes the player face the mouse position.
    /// Also resets the navmash agent's path.
    /// </summary>
    public void FaceMousePosition() {
        transform.LookAt(new Vector3(_mousePosition.x, transform.position.y, _mousePosition.z));
        agent.ResetPath();
    }

    /// <summary>
    /// Handles regular movement on controllers.
    /// </summary>
    private void HandleControllerMovement() {
        // Horizontal and vertical input values of the left joystick
        float leftHorizontal = Input.GetAxis("Horizontal");
        float leftVertical = Input.GetAxis("Vertical");

        // Check if the player is moving the left joystick
        if (leftHorizontal != 0f || leftVertical != 0f) {
            // Move in the direction of the left joystick
            Vector3 goal = gameObject.transform.position + new Vector3(leftHorizontal, 0f, leftVertical).normalized;
            agent.transform.LookAt(goal);
            agent.destination = goal;
        }
    }

    private void HandleControllerDashing() {
        // Horizontal and vertical input values of the right joystick
        float rightHorizontal = Input.GetAxis("Dash Horizontal");
        float rightVertical = Input.GetAxis("Dash Vertical");

        // Check for skill casts
        if ((rightHorizontal != 0f || rightVertical != 0f)) {
            if (skillCaster.Cast(1)) {
                transform.LookAt(transform.position + new Vector3(rightHorizontal, 0f, rightVertical).normalized);
                agent.ResetPath();
            }
        }
    }

    /// <summary>
    /// Checks if the mouse position is colliding with any gameobject's colliders.
    /// </summary>
    /// <param name="mousePosition">A Vector3 to output to</param>
    /// <returns>Whether a collider was found or not</returns>
    private bool GetMousePosition(out Vector3 mousePosition) {
        // Ray from camera to the clicked position in world space
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray collided with anything
        if (Physics.Raycast(ray, out hit, 100)) {
            // Return the collision point
            mousePosition = hit.point;
            return true;
        }

        // No collision point
        mousePosition = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Additional cleanup for when this gameobject dies.
    /// </summary>
    private void Die() {
        skillCaster.ClearSkillObjects(); // Remove all active skill objects
        Destroy(gameObject);

        // trigger gameover screen
        Transform gameOver = GameObject.Find("GameOver").transform;
        Transform gameOverMsg = gameOver.GetChild(0);
        gameOverMsg.gameObject.SetActive(true);

        // close the menu
        GameObject.Find("Menu").SetActive(false);

    }
}

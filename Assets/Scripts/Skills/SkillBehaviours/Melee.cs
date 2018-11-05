﻿using UnityEngine;

/// <summary>
/// This is a skill behaviour that deals damage in an arc infront of the player.
/// </summary>
public class Melee : MonoBehaviour {

    /// <summary>
    /// The physical attack damage of the user.
    /// </summary>
    private float _physAtkdamage;

    /// <summary>
    /// The magical attack damage of the user.
    /// </summary>
    private float _magicAtkdamage;

    /// <summary>
    /// Reference to the animator status.
    /// </summary>
    private AnimatorStatus _animatorStatus;

    /// <summary>
    /// Reference to this gameobject's Collider component.
    /// </summary>
    private Collider _collider;

    /// <summary>
    /// Reference to the skill that called this attack.
    /// </summary>
    private Skill _skill;

    /// <summary>
    /// Flag for if this behaviour is ready to begin.
    /// </summary>
    private bool ready;

    // Use this for initialization
    private void Start () {
        _animatorStatus = transform.parent.GetComponent<BaseCharacter>().animatorStatus;
        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    private void Update() {
        if (ready) {
            // Only deal damage within the damage window of the animation
            if (_animatorStatus.canDealDamage) {
                _collider.enabled = true;
            } else {
                // Check if the damage window is over
                if (_collider.enabled && !_animatorStatus.canDealDamage) {
                    Destroy(gameObject);
                }
            }
        }
    }

    /// <summary>
    /// For detecting collisions.
    /// </summary>
    /// <param name="other">The collision object collided with</param>
    private void OnTriggerEnter(Collider other) {
        // Only deal damage to Player or AI tags, and no friendly fire
        if ((other.CompareTag("AI") || other.CompareTag("Player")) && !other.CompareTag(transform.parent.tag)) {

            // Apply damage to the other character
            _skill.DealDamage(transform.parent.GetComponent<BaseCharacter>(), 
                other.GetComponent<BaseCharacter>(), _physAtkdamage, _magicAtkdamage);
        }
    }

    /// <summary>
    /// Starts the melee attack.
    /// </summary>
    public void Activate(Transform caster, Skill skill) {
        // Setup melee properties
        _physAtkdamage = caster.GetComponent<BaseCharacter>().characterStats.physAtk * skill.physicalMultiplier;
        _magicAtkdamage = caster.GetComponent<BaseCharacter>().characterStats.magicAtk * skill.magicMultiplier;
        _skill = skill;
        transform.localScale *= skill.rangeMultiplier;
        transform.localPosition = transform.localScale * 1.5f;
        
        // Start the melee attack
        caster.GetComponent<BaseCharacter>().animator.SetTrigger("Attack"); // Play attack animation
        ready = true;
    }
}
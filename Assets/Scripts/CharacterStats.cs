﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour {

    /// <summary>
    /// Damage event template.
    /// </summary>
    /// <param name="physAtkdamge">The amount of physical damage to take.</param>
    /// <param name="magicAtkdamage">The amount of magical damage to take.</param>
    public delegate void DamageEvent(float physAtkdamge, float magicAtkdamage);

    /// <summary>
    /// Death event template.
    /// </summary>
    public delegate void DeathEvent();

    /// <summary>
    /// This damage event is called after the character has taken damage.
    /// </summary>
    public event DamageEvent OnDamageTaken;

    public enum StatType {
        NONE,
        CURRENT_HP,
        MAX_HP,
        CURRENT_MP,
        MAX_MP,
        PHY_ATK,
        MAG_ATK,
        PHY_DEF,
        MAG_DEF,
        EXP
    }

    /// <summary>
    /// This death event is called when the character dies.
    /// </summary>
    public event DeathEvent OnDeath;

    /// <summary>
    /// Reference to the health bar prefab.
    /// </summary>
    public GameObject healthBar;

    /// <summary>
    /// The character's current health point.
    /// </summary>
    public float currentHp;

    /// <summary>
    /// The character's maximum health point. 
    /// </summary>
    public float maxHp;

    /// <summary>
    /// The character's current mana point.
    /// </summary>
    public float currentMp;

    /// <summary>
    /// The character's maximum mana point.
    /// </summary>
    public float maxMp;

    /// <summary>
    /// The character's physical attack stat.
    /// </summary>
    public float physAtk;

    /// <summary>
    /// The character's magic attack stat.
    /// </summary>
    public float magicAtk;

    /// <summary>
    /// The character's physical defense stat. 
    /// </summary>
    public float physDef;

    /// <summary>
    /// The character's magic defense stat.
    /// </summary>
    public float magicDef;

    /// <summary>
    /// The character's level.
    /// </summary>
    public int level;

    /// <summary>
    /// The character's experience point.
    /// </summary>
    public float exp;

    /// <summary>
    /// Boolean to represent if a character is alive.
    /// </summary>
    public bool isAlive;

    private void Start() {
        OnDeath += Die;
    }

    /// <summary>
    /// Called when this character takes healing.
    /// </summary>
    /// <param name="healingAmount"></param>
    public void TakeHealing(float healingAmount) {
        float netHealingAmount = 0f;
        if (currentHp + healingAmount > maxHp) {
            netHealingAmount = maxHp - currentHp;
            currentHp = maxHp;
        } else {
            currentHp += healingAmount;
            netHealingAmount = healingAmount;
        }

        // Show the healing number
        FloatingTextController.instance.CreateCombatNumber(netHealingAmount, false, gameObject);
    }

    /// <summary>
    /// Basic damage calculation function.
    /// </summary>
    /// <param name="physAtkDamage">The amount of physical damage to take.</param>
    /// <param name="magicAtkDamage">The amount of magical damage to take.</param>
    public void TakeDamage(float physAtkDamage, float magicAtkDamage) {

        if (physDef < physAtkDamage) {
            currentHp = currentHp - (physAtkDamage - physDef);
        }

        if (magicDef < magicAtkDamage) {
            currentHp = currentHp - (magicAtkDamage - magicDef);
        }

        if (currentHp > 0) {
            isAlive = true;
        } else {
            isAlive = false;
            OnDeath();
        }

        // Start the take damage animation
        //GetComponent<Animator>().SetTrigger("TakeDamage");

        // if damage was taken, trigger OnDamageTaken event
        if (physDef < physAtkDamage || magicDef < magicAtkDamage) {
            if (OnDamageTaken != null) {
                OnDamageTaken(physAtkDamage, magicAtkDamage);
            }
        }
        ShowDamageText(physAtkDamage, magicAtkDamage); // Show damage numbers
    }

    /// <summary>
    /// Looks up a stat and adds to it
    /// </summary>
    /// <param name="targetStat">Stat to look up</param>
    /// <param name="amt"></param>
    public void AddToStat(StatType targetStat, float amt) {
        switch (targetStat) {
            case StatType.CURRENT_HP:
                currentHp += amt;
                break;

            case StatType.MAX_HP:
                maxHp += amt;
                break;
                
            case StatType.CURRENT_MP:
                currentMp += amt;
                break;
                
            case StatType.MAX_MP:
                maxMp += amt;
                break;
                
            case StatType.PHY_ATK:
                physAtk += amt;
                break;
                
            case StatType.MAG_ATK:
                magicAtk += amt;
                break;
                
            case StatType.PHY_DEF:
                physDef += amt;
                break;
                
            case StatType.MAG_DEF:
                magicDef += amt;
                break;
                
            case StatType.EXP:
                exp += amt;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Shows the damage taken as a floating text object on the canvas.
    /// </summary>
    /// <param name="physAtkDamage">The raw amount of physical damage taken</param>
    /// <param name="magicAtkDamage">The raw amount of magical damage taken</param>
    private void ShowDamageText(float physAtkDamage, float magicAtkDamage) {
        float netDamageTaken = 0f;

        // Calculate net damage taken
        if (physDef < physAtkDamage) {
            netDamageTaken += physAtkDamage - physDef;
        }
        if (magicDef < magicAtkDamage) {
            netDamageTaken += magicAtkDamage - magicDef;
        }
      
        // Show the damage text
        FloatingTextController.instance.CreateCombatNumber(netDamageTaken, true, gameObject);
    }

    private void ShowHealingText() {
        //FloatingTextController.instance.CreateCombatNumber(netDamageTaken, true, gameObject);
    }

    /// <summary>
    /// Called when the gameobject dies.
    /// </summary>
    public void Die() {
        Destroy(gameObject); // Get rid of the gameobject
    }

}

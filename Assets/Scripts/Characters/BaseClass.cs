﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseClass : MonoBehaviour {
    /// <summary>
    /// The ID of the class, for other scripts
    /// </summary>
    public int classID;
    /// <summary>
    /// Initial stats for selecting the class
    /// </summary>
    public float bonusHP;
    public float bonusMP;
    public float bonusPhysAtt;
    public float bonusMagAtt;
    public float bonusPhysDef;
    public float bonusMagDef;
    public float bonusCrit;
    public float bonusCritDmg;

    /// <summary>
    /// Stat increases per level for the class
    /// </summary>
    public float perLevelHP;
    public float perLevelMP;
    public float perLevelPhysAtt;
    public float perLevelMagAtt;
    public float perLevelPhysDef;
    public float perLevelMagDef;
    public float perLevelCrit;
    public float perLevelCritDmg;

    /// <summary>
    /// Skills
    /// </summary>
    public int skill1;
    public int skill2;
    public int skill3;
    public int skill4;

    /// <summary>
    /// Used for leveling
    /// </summary>
    public void GainStats()
    {
        CharacterStats player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>();
        player.maxHp += perLevelHP;
        player.maxMp += perLevelMP;
        player.physAtk += perLevelPhysAtt;
        player.physDef += perLevelPhysDef;
    }

    /// <summary>
    /// Used for leveling
    /// </summary>
    public void InitiateClass()
    {
        CharacterStats player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>();
        player.maxHp += bonusHP;
        player.maxMp += bonusMP;
        player.physAtk += bonusPhysAtt;
        player.magicAtk += bonusMagAtt;
        player.physDef += bonusPhysDef;
        player.magicDef += bonusMagDef;
    }
}
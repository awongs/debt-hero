﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogManager : MonoBehaviour {
    /// <summary>
    /// Background image of the dialog prefab.
    /// </summary>
    public Image dialogBackground;

    /// <summary>
    /// Text component of the dialog prefab.
    /// </summary>
    public Text dialogText;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Read Lines of dialog.
    /// </summary>
    /// <param name="line"></param>
    public void ReadDialog(string line) {
        ToggleDialog(true);
        dialogText.text = line;
    }

    /// <summary>
    /// Disable the dialog box.
    /// </summary>
    public void CloseDialog() {
        ToggleDialog(false);
    }

    /// <summary>
    /// Toggle Dialog
    /// </summary>
    /// <param name="flag"></param>
    public void ToggleDialog(bool flag) {
        dialogBackground.enabled = flag;
        dialogText.text = "";
        dialogText.enabled = flag;
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopSlideZone : MonoBehaviour, IPointerEnterHandler {

	public InventoryPanel itemPanel;

	public Animator animator;

	public bool sellerSide;

	/// <summary>
	/// Pointer enter event
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerEnter(PointerEventData eventData) {
		animator.SetBool("Seller", sellerSide);
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update() {
		float lrswitch = Input.GetAxis("Switch Game Menu");

		if (sellerSide) {
			if (lrswitch > 0) {
				animator.SetBool("Seller", sellerSide);
			} else if (lrswitch < 0) {
				itemPanel.enabled = false;
			}
		} else {
			if (lrswitch < 0) {
				animator.SetBool("Seller", sellerSide);
			} else if (lrswitch > 0) {
				itemPanel.enabled = false;
			}
		}
	}
}
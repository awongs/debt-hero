﻿using System;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class InventoryPanel : MonoBehaviour {

	[Header("Text Displays")]

	/// <summary>
	/// Text showing the gold amount
	/// </summary>
	public TextMeshProUGUI goldText;

	[Header("Item Detail Text")]

	/// <summary>
	/// Text showing the item name
	/// </summary>
	public TextMeshProUGUI itemNameText;

	/// <summary>
	/// Text showing the item desc
	/// </summary>
	public TextMeshProUGUI itemDescText;

	/// <summary>
	/// Text showing the item quantity
	/// </summary>
	public TextMeshProUGUI itemQtyText;

	/// <summary>
	/// Area for big item icon
	/// </summary>
	public Transform itemIconArea;

	[Header("Items")]

	/// <summary>
	/// Rows of item ui slots
	/// </summary>
	public ItemRowUI[] itemRows;

	[Header("Inventory Panel Settings")]

	/// <summary>
	/// Delay between slots
	/// </summary>
	public float slotDelay = 0.1f;

	/// <summary>
	/// array of item uis
	/// </summary>
	private ItemUI[,] _items;

	/// <summary>
	/// target inventory to show
	/// </summary>
	private CharacterInventory _inventory;

	/// <summary>
	/// Currente selected item slot for moving items
	/// </summary>
	private ItemSlot _currentMoveSlot;

	/// <summary>
	/// mouse item icon
	/// </summary>
	private GameObject _mouseItemIcon;

	/// <summary>
	/// Item details icon
	/// </summary>
	private GameObject _itemDetailIcon;

	/// <summary>
	/// current hovered or selected slot 
	/// </summary>
	private ItemSlot _currentSelectSlot;

	/// <summary>
	/// Time for cool down
	/// </summary>
	private float _cooldownTime;

	/// <summary>
	/// cool down flag
	/// </summary>
	private bool _onCooldown;

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start() {
		_inventory = PlayerManager.instance.GetComponent<CharacterInventory>();

		_items = new ItemUI[itemRows.Length, itemRows[0].items.Length];

		_inventory.OnItemAdded += UpdateItemSlot;

		ItemGridItemUI.OnLeftClick += SelectSlot;

		ItemGridItemUI.OnRightClick += UseSlot;

		ItemGridItemUI.OnHoverOver += ShowItemDetails;

		ItemGridItemUI.OnHoverOff += ResetItemDetails;

		// assign slot references

		for (int row = 0; row < itemRows.Length; row++) {
			for(int col = 0; col < itemRows[0].items.Length; col++) {
				ItemSlot slot = new ItemSlot(row, col);
				GetGridSlot(slot).slot = slot;
			}
		}

		UpdateAllItemSlots();

		ResetItemDetails(null);

		_currentSelectSlot = new ItemSlot(0, 0);
	}
	
	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update() {
		if (_mouseItemIcon != null) {
			_mouseItemIcon.transform.position = Input.mousePosition;
		}

		InventoryControls();
	}

	/// <summary>
	/// Key controls for inventory
	/// </summary>
	private void InventoryControls() {
		if (_onCooldown) {
			_cooldownTime += Time.deltaTime;

			if (_cooldownTime >= slotDelay) {
				_onCooldown = false;
				_cooldownTime = 0;
			}

			return;
		}

		// input
		float vert = Input.GetAxis("Menu Vertical");
		float horz = Input.GetAxis("Menu Horizontal");

		// row col after
		int rowAfter = horz == 0 ? 0 : (horz > 0 ? 1 : -1);
		rowAfter += _currentSelectSlot.row;

		// col inverted because positive is down
		int colAfter = vert == 0 ? 0 : (vert > 0 ? -1 : 1);
		colAfter += _currentSelectSlot.col;

		if (rowAfter >= 0 && rowAfter < itemRows.Length) {
			_currentSelectSlot.row = rowAfter;
			_onCooldown = true;
		}

		if (colAfter >= 0 && colAfter < itemRows[0].items.Length) {
			_currentSelectSlot.col = colAfter;
			_onCooldown = true;
		}
	}

	/// <summary>
	/// This function is called when the object becomes enabled and active.
	/// </summary>
	void OnEnable() {	
		if (_inventory == null) {
			return;
		}

		goldText.text = _inventory.gold + "g";
		UpdateAllItemSlots();

		_currentSelectSlot = new ItemSlot(0, 0);
	}

	/// <summary>
	/// This function is called when the behaviour becomes disabled or inactive.
	/// </summary>
	void OnDisable() {
		_currentMoveSlot = null;

		Destroy(_mouseItemIcon);

		_mouseItemIcon = null;

		ResetItemDetails(null);
	}

	/// <summary>
	/// Updates all item slots
	/// </summary>
	public void UpdateAllItemSlots() {
		for (int row = 0; row < itemRows.Length; row++) {
			for (int col = 0; col < itemRows[0].items.Length; col++) {

				ItemSlot slot = new ItemSlot(row, col);

				UpdateItemSlot(slot);
			}
		}
	}

	/// <summary>
	/// Uses the item in the slot
	/// </summary>
	/// <param name="slot"></param>
	private void UseSlot(ItemSlot slot) {
		ItemBase item = _inventory.GetItem(slot);

		if (item != null) {
			item.Use();
		}

		UpdateItemSlot(slot);
	}

	/// <summary>
	/// Shows the item details in the slot if any
	/// </summary>
	/// <param name="slot"></param>
	private void ShowItemDetails(ItemSlot slot) {
		ItemBase item = _inventory.GetItem(slot);

		// display details
		if (item != null) {
			itemNameText.text = item.properties.name;
			itemDescText.text = item.properties.description;
			itemQtyText.text = "x " + item.properties.quantity;

			ItemUI iUI = Instantiate(item.itemUIPrefab).GetComponent<ItemUI>();

			_itemDetailIcon = iUI.icon.gameObject;

			_itemDetailIcon.transform.SetParent(itemIconArea, false);

			Destroy(iUI.gameObject);
		}
	}

	/// <summary>
	/// Resets the item details area to blank
	/// </summary>
	/// <param name="slot">Unused. Satifies delegate template</param>
	private void ResetItemDetails(ItemSlot slot) {
		itemNameText.text = "";
		itemDescText.text = "";
		itemQtyText.text = "";

		if (_itemDetailIcon != null) {
			Destroy(_itemDetailIcon);
			_itemDetailIcon = null;
		}
	}

	/// <summary>
	/// Selects the item slot for action
	/// </summary>
	/// <param name="slot"></param>
	private void SelectSlot(ItemSlot slot) {
		// begin item movement
		if (_currentMoveSlot == null) {
			ItemBase item = _inventory.GetItem(slot);

			// Drag icon and stuff if there is actually an item
			if (item != null) {
				_currentMoveSlot = slot;

				ItemUI iUI = Instantiate(item.itemUIPrefab).GetComponent<ItemUI>();

				_mouseItemIcon = iUI.icon.gameObject;

				_mouseItemIcon.transform.SetParent(GameObject.Find("Canvas").transform, false);

				RectTransform rt = _mouseItemIcon.GetComponent<RectTransform>();

				rt.sizeDelta = new Vector2(100, 100);

				rt.anchorMin = new Vector2(0.5f, 0.5f);
				rt.anchorMax = new Vector2(0.5f, 0.5f);

				Destroy(iUI.gameObject);
			}
		}

		// end item movement
		else if (_currentMoveSlot != null) {
			_inventory.SwapItems(_currentMoveSlot, slot);

			UpdateItemSlot(_currentMoveSlot);
			UpdateItemSlot(slot);

			_currentMoveSlot = null;

			Destroy(_mouseItemIcon);

			_mouseItemIcon = null;
		}
	}

	/// <summary>
	/// Adds an item from inventory in the same slot to the display grid
	/// </summary>
	/// <param name="slot"></param>
	private void UpdateItemSlot(ItemSlot slot) {
		ItemBase item = _inventory.GetItem(slot);

		ItemUI itemUI = _items[slot.row, slot.col];

		// no item in slot but ui exist... remove ui
		if (item == null && itemUI != null) {
			Destroy(itemUI.gameObject);
		}

		// item exist in slot but no ui... create ui
		else if (item != null && itemUI == null) {
			itemUI = Instantiate(item.itemUIPrefab).GetComponent<ItemUI>();

			itemUI.transform.SetParent(GetGridSlot(slot).transform, false);

			_items[slot.row, slot.col] = itemUI;

			itemUI.stackText.text = item.properties.quantity + "";
		}

		// item and ui exist... update ui
		else if (item != null && itemUI != null) {
			itemUI.stackText.text = item.properties.quantity + "";
		}
	}

	private ItemGridItemUI GetGridSlot(ItemSlot slot) {
		return itemRows[slot.row].items[slot.col];
	}

}

/// <summary>
/// Class used to get 2d array of ItemBases to appear in the unity inspector
/// </summary>
[Serializable]
public class ItemRowUI {
	public ItemGridItemUI[] items;
}

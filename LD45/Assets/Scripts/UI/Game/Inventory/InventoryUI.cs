﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : BaseUI {
	public Inventory Inventory;
	public GameObject ParentForDraggedSlot;

	[NonSerialized] public bool isDrag;

	protected List<ItemSlot> itemSlots;

	override protected void Awake() {
		base.Awake();
		
		Inventory.OnItemsChanged.AddListener(UpdateUI);

		itemSlots = new List <ItemSlot>(Inventory.Items.Length);
	}

	protected virtual void Start() {
		ItemSlot[] items = GetComponentsInChildren<ItemSlot>(true);
		for (byte i = 0; i < items.Length; ++i) {
			itemSlots.Add(items[i]);
			items[i].invId = i;
		}

		HideAfterStart();
	}

	virtual protected void OnDestroy() {
		Inventory.OnItemsChanged.RemoveListener(UpdateUI);
	}

	public override bool CanChangeShowHide() {
		return !isDrag;
	}

	protected override void BeforeShow() {
		UpdateUIForce();
	}

	public void UpdateUI() {
		if (!IsShowed)
			return;
		UpdateUIForce();
	}

	public void UpdateUIForce() {
		for (byte i = 0; i < Inventory.Items.Length; ++i)
			itemSlots[i].SetItem(Inventory.Items[i]);
	}
}

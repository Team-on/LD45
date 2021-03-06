﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour {
	public SpriteRenderer ItemInHandLeft;
	public SpriteRenderer ItemInHandRight;
	public SpriteRenderer ItemInHandDual;

	[NonSerialized] public GameObject GOLinkedAnim;
	public Action OnUseHandAnimEndEvent;

	[NonSerialized] public ItemSO handLeft;
	[NonSerialized] public ItemSO handRight;
	[NonSerialized] public ItemSO head;
	[NonSerialized] public ItemSO armor;

	Animator animator;


	private void Start() {
		animator = GameManager.Instance.Player.Animator;
	}

    public void PlayUseHandItemAnim(bool isLeftHand) {
		switch ((isLeftHand ? handLeft : handRight).Type) {
			case ItemSO.ItemType.Axe:
				OnStartAnim();
				animator.Play("AxePunch");
				break;
			case ItemSO.ItemType.Pickaxe:
				OnStartAnim();
				animator.Play("PickaxePunch");
				break;
			case ItemSO.ItemType.Spear:
				OnStartAnim();
				animator.Play("SpearPunch");
				break;                              
			default:
				//TODO: show that it cant be used
				break;
		}
	}

	//Callback for animations
	public void OnUseHandAnimEnd() {
		if(OnUseHandAnimEndEvent != null) {
			OnUseHandAnimEndEvent();
			OnUseHandAnimEndEvent = null;
			ItemInHandLeft.enabled = true;
			ItemInHandRight.enabled = true;
			ItemInHandDual.enabled = true;
		}
	}

	public void EquipItem(ItemSO item, bool forceHand = false, bool isLeftHand = false) {
		GameManager.Instance.Player.InterruptAction();
		if (item == null)
			return;

		if (forceHand) {
			EquipInHand(item, isLeftHand);
		}
		else {
			switch (item.Slot) {
				case ItemSO.ItemSlot.HandLeft:
					EquipInHand(item, true);
					break;
				case ItemSO.ItemSlot.HandRight:
					EquipInHand(item, false);
					break;
				case ItemSO.ItemSlot.Head:
					head = item;
					break;
				case ItemSO.ItemSlot.Armor:
					armor = item;
					break;
			}
		}
	}

	public void UnequipItem(ItemSO.ItemSlot slot) {
		switch (slot) {
			case ItemSO.ItemSlot.HandLeft:
				if (IsDualWield()) {
					ItemInHandDual.sprite = null;
					ItemInHandDual.enabled = false;
					SetRightHandSprite();
				}

				handLeft = null;
				ItemInHandLeft.sprite = null;
				ItemInHandLeft.enabled = false;
				break;
			case ItemSO.ItemSlot.HandRight:
				if (IsDualWield()) {
					ItemInHandDual.sprite = null;
					ItemInHandDual.enabled = false;
					SetLeftHandSprite();
				}

				handRight = null;
				ItemInHandRight.sprite = null;
				ItemInHandRight.enabled = false;
				break;
			case ItemSO.ItemSlot.Head:
				head = null;
				break;
			case ItemSO.ItemSlot.Armor:
				armor = null;
				break;
		}
	}

	public bool NeedInterrupt() {
		return OnUseHandAnimEndEvent != null;
	}

	void EquipInHand(ItemSO item, bool isLeftHand) {
		if (isLeftHand) {
			if (IsDualWield()) {
				ItemInHandDual.sprite = null;
				ItemInHandDual.enabled = false;
				SetRightHandSprite();
			}

			handLeft = item;
			SetLeftHandSprite();
		}
		else {
			if (IsDualWield()) {
				ItemInHandDual.sprite = null;
				ItemInHandDual.enabled = false;
				SetLeftHandSprite();
			}

			handRight = item;
			SetRightHandSprite();
		}

		if (IsDualWield()) {
			SetDualHandSprite();
		}

		EventData eventData = new EventData("OnItemSlotChange");
		eventData["ItemSlotType"] = item;
		GameManager.Instance.EventManager.CallOnEquipmentChange(eventData);
	}
	
	public void InterruptAction() {
		OnUseHandAnimEndEvent = null;
		ItemInHandLeft.enabled = true;
		ItemInHandRight.enabled = true;
		ItemInHandDual.enabled = true;
		GOLinkedAnim = null;
	}

	public bool IsDualWield() {
		return handLeft == handRight && handLeft != null;
	}

	void OnStartAnim() {
		ItemInHandLeft.enabled = false;
		ItemInHandRight.enabled = false;
		ItemInHandDual.enabled = false;
	}

	void SetLeftHandSprite() {
		ItemInHandLeft.transform.localScale = new Vector3(handLeft.ScaleInHand, handLeft.ScaleInHand, 1.0f);
		ItemInHandLeft.sprite = handLeft.Sprite;
		ItemInHandLeft.enabled = true;
	}

	void SetRightHandSprite() {
		ItemInHandRight.transform.localScale = new Vector3(handRight.ScaleInHand, handRight.ScaleInHand, 1.0f);
		ItemInHandRight.sprite = handRight.Sprite;
		ItemInHandRight.enabled = true;
	}

	void SetDualHandSprite() {
		ItemInHandLeft.sprite = ItemInHandRight.sprite = null;
		ItemInHandLeft.enabled = ItemInHandRight.enabled = false;

		ItemInHandDual.transform.localScale = new Vector3(handLeft.ScaleInHand, handLeft.ScaleInHand, 1.0f);
		ItemInHandDual.sprite = handLeft.Sprite;
		ItemInHandDual.enabled = true;
	}
}

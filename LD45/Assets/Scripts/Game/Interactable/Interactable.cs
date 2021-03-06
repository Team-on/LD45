﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// Всі класи, що наслідуються мають всередині виконувати   
// GameManager.Instance.Player.Equipment.GOLinkedAnim = null;
// Це треба для анимашок, щоб не викликати одне і те ж саме, якщо спамиш кликами
public class Interactable : MonoBehaviour {
	public float InteractDist;
	public Action OnMouseClick;

	[SerializeField] protected string tip;
	protected bool isInteractLMB;

	[SerializeField] bool interactPosOnCenter;
	[SerializeField] float outlineScale = 1;
	[SerializeField] float outlineSize = 1;

	SpriteRenderer spriteRenderer;

	SpriteOutline outline;
	Vector3 interactPos;
	float InteractDistSqr;

	protected virtual void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();

		InteractDistSqr = InteractDist * InteractDist;
    }

	virtual protected void Start() {
		RecalcInteractPos();
	}

	// OnMouseEnter та OnMouseExit викликаються завжди послідовно. Якщо курсор буде над 2 обєктами, то буде: OnMouseEnter -> OnMouseExit -> OnMouseEnter
	// Якщо буде дуже важно мати правильну обводку, то треба буде в OnMouseOver або в OnMouseEnter робити рейкасти, і чекати найвищий спрайт
	void OnMouseEnter() {
		if (GameManager.Instance.IsPaused/* || EventSystem.current.IsPointerOverGameObject()*/)
			return;

		GameManager.Instance.SelectedOutlineGO = this;
		ShowOutline();
	}

    private void OnMouseOver() {
		if (GameManager.Instance.IsPaused || GameManager.Instance.SelectedOutlineGO != this)
			return;

		if (Input.GetMouseButtonDown(0)) {
			isInteractLMB = true;
			ProcessMouseDown();
		}
		else if (Input.GetMouseButtonDown(1)){
			isInteractLMB = false;
			ProcessMouseDown();
		}

		EventData eventData = new EventData("OnPopUpShow");
        eventData["tipText"] = tip;
        GameManager.Instance.EventManager.CallOnMouseOverTip(eventData);
    }

	void OnMouseExit() {
		if (GameManager.Instance.IsPaused)
			return;

		HideOutline();
		GameManager.Instance.SelectedOutlineGO = null;
	}

	public void RecalcInteractPos() {
		interactPos = spriteRenderer.bounds.center;
		if (!interactPosOnCenter)
			interactPos += Vector3.down * spriteRenderer.bounds.size.y / 2;
	}

	public void SimulateMouseClick() {
		isInteractLMB = true;
		ProcessMouseDown();
	}

	public virtual bool CanInteract() => true;

	public bool IsInRange() {
		return GameManager.Instance.Player.CanInteract(transform.position, InteractDistSqr);
	}

	void ProcessMouseDown() {
		if (GameManager.Instance.IsPaused || GameManager.Instance.SelectedOutlineGO != this || GameManager.Instance.Player.Equipment.GOLinkedAnim == gameObject || !CanInteract())
			return;

		GameManager.Instance.Player.InterruptAction();
		GameManager.Instance.Player.Equipment.GOLinkedAnim = gameObject;
		if (IsInRange()) {
			OnMouseClick?.Invoke();
		}
		else {
			GameManager.Instance.Player.MoveTo(interactPos);
			GameManager.Instance.Player.OnMoveEndEvent += OnMouseClick;
		}
	}

	void ShowOutline() {
		if (outline == null)
			outline = CreateOutline(gameObject, true);
		outline.gameObject.SetActive(true);
	}

	void HideOutline() {
		outline?.gameObject?.SetActive(false);
	}

	SpriteOutline CreateOutline(GameObject parentGO, bool needScale) {
		var gameObject = new GameObject() {
			name = $"{parentGO.name}-outline",
		};
		gameObject.transform.parent = parentGO.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one * outlineScale;

		SpriteRenderer parentsr = parentGO.GetComponent<SpriteRenderer>();
		SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
		sr.sprite = parentsr.sprite;
		sr.sortingOrder = parentsr.sortingOrder - 1;

		SpriteOutline outline = gameObject.AddComponent<SpriteOutline>();
		outline._outlineSize = outlineSize;
		outline.color = Color.yellow;
		outline.UpdateOutline(outline._outlineSize);
		return outline;
	}

	public static void OnPause() {
		GameManager.Instance?.SelectedOutlineGO?.HideOutline();
		GameManager.Instance.SelectedOutlineGO = null;
	}
}

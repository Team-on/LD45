﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
	public Inventory Inventory;
	public Equipment Equipment;

	public float InteractDist;
	internal float InteractDistSqr;

	void Awake() {
		GameManager.Instance.Player = this;
		InteractDistSqr = InteractDist * InteractDist;
	}

	public bool CanInteract(Vector3 pos) {
		return (transform.position - pos).sqrMagnitude < InteractDistSqr;
	}

	public bool CanInteract(Vector3 pos, float InteractDistSqr) {
		Debug.Log((transform.position - pos).magnitude);
		return (transform.position - pos).sqrMagnitude < InteractDistSqr;
	}
}

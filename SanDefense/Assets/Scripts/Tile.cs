﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
	GameObject occupant;
	Renderer myRenderer;
	// Use this for initialization
	void Start () {
		myRenderer = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Gets a value indicating whether this <see cref="Tile"/> is occupied.
	/// </summary>
	/// <value><c>true</c> if occupied; otherwise, <c>false</c>.</value>
	public bool Occupied {
		get {
			return occupant != null;
		}
	}

	/// <summary>
	/// Gets the occupant.
	/// </summary>
	/// <value>The occupant.</value>
	public GameObject Occupant {
		get {
			return occupant;
		}

		set {
			occupant = value;
			occupant.transform.position = transform.position;
		}
	}

	/// <summary>
	/// Sets a value indicating whether this <see cref="Tile"/> is selected.
	/// </summary>
	/// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
	public bool Selected {
		set {
			myRenderer.material.color = value ? Color.cyan : Color.white;
		}
	}
}
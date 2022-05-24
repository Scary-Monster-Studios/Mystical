using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class medusaTypeControl : MonoBehaviour {

	public List<GameObject> gameObjects = new List<GameObject>();

	public void ToggleMedusa(){
		for (int i = 0; i < gameObjects.Count; i++){
			gameObjects [i].SetActive (!gameObjects [i].activeSelf);
		}
	}
}
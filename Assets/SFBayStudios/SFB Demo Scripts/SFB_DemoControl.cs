using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_DemoControl : MonoBehaviour {

	public GameObject cameraObject;							// The camera object

	/// <summary>
	/// Simply sets the .y value of the camera transform
	/// </summary>
	/// <param name="newValue">New value.</param>
	public void SetCameraHeight(float newValue){
		Vector3 position = cameraObject.transform.position;	// Get the position
		position.y = newValue;								// Set the value
		cameraObject.transform.position = position;			// Set the position
	}

	/// <summary>
	/// Simply sets the timescale
	/// </summary>
	/// <param name="newValue">New value.</param>
	public void SetTimescale(float newValue){
		Time.timeScale = newValue;							// Set the value
	}

}
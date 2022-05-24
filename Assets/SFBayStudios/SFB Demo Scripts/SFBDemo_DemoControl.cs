using UnityEngine;
using System.Collections;

public class SFBDemo_DemoControl : MonoBehaviour {

	/// <summary>
	/// This will set the Time.timeScale value
	/// </summary>
	/// <param name="newValue">New value.</param>
	public void SetTimescale(float newValue){
		Time.timeScale = newValue;										// Set timescale
	}
}
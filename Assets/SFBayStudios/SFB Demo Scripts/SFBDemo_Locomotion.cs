using UnityEngine;
using System.Collections;

public class SFBDemo_Locomotion : MonoBehaviour {

	public Animator animator;									// Animator Controller
	public string stringName = "locomotion";					// Name of locomotion variable string in Animator

	void Start(){
		if (!animator) {										// If animator has no value
			animator = GetComponent<Animator> ();				// Grab Animator
		}
	}

	/// <summary>
	/// Will set the value of locomotion speed in the Animator
	/// </summary>
	/// <param name="newValue">New value.</param>
	public void SetLocomotion(float newValue){
		if (animator) {											// If we have an animator
			animator.SetFloat (stringName, newValue);			// Set value
		}
	}
}

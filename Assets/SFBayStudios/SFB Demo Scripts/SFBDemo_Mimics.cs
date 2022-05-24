using UnityEngine;
using System.Collections;

public class SFBDemo_Mimics : MonoBehaviour {

	Animator animator;											// Reference to animator component

	public ParticleSystem[] castWarmup;							// Array of warmup particles
	public ParticleSystem[] castSpell;							// Array of spell particles

	public string locomotionName = "locomotion";				// Name of locomotion variable in Animator

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();					// Assign reference
		if (!animator) {										// If there is no animator
			// Show an error
			Debug.LogError ("Warning:  No Animator is found on " + gameObject.name + ", but the Demo Script requires it!");
		}
	}

	/// <summary>
	/// Updates the locomotion value in the Animator
	/// </summary>
	/// <param name="newValue">New value.</param>
	public void UpdateLocomotion(float newValue){
		if (animator) {											// If animator is assigned
			animator.SetFloat (locomotionName, newValue);		// Update the value
		}
	}

	/// <summary>
	/// Starts the cast particles
	/// </summary>
	public void StartWarmup(){
		for (int i = 0; i < castWarmup.Length; i++){			// For each castWarmup particle
			castWarmup [i].Play ();								// Play()
		}
	}

	/// <summary>
	/// Stops the Warmup and starts the Cast particles
	/// </summary>
	public void StartCast(){
		for (int i = 0; i < castWarmup.Length; i++){			// For each castWarmup particle
			castWarmup [i].Stop ();								// Stop()
		}
		for (int i = 0; i < castSpell.Length; i++){				// For each castSpell particle
			castSpell [i].Play ();								// Play()
		}
	}

	/// <summary>
	/// Stops the Cast portion
	/// </summary>
	public void StopCast(){
		for (int i = 0; i < castSpell.Length; i++){				// For each castSpell particle
			castSpell [i].Stop ();								// Stop()
		}
	}
}
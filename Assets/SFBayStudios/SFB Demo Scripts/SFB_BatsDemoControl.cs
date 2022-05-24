using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// NOTE:  The death 2 animation is a loop.  Depending on how fast you want your bat to fall, you should repeat that until the correct distance from the ground
/// at which time you transition to Death 3.  10 frames into death 3, the bat will "hit" the ground and bounce.  All of the vertical falling must be controlled
/// by code.  In some cases, the bat will be too close to the ground to make use of the death 2 animation.
/// </summary>


public class SFB_BatsDemoControl : MonoBehaviour {

	public float deathAnimationTime = 2.0f;									// How long the Death 1, 2, and 3 animations take (before the ground bounce) aka 60 frames = 2 seconds
	public GameObject[] bats;												// Reference to each bat
	public Vector3[] deathStartPosition;									// Start position of death motion
	public Vector3[] deathEndPosition;										// End position of death motion
	public float maxTurnTilt = 90.0f;										// Maximum tilt when turning
	public float maxTurnRotation = 360.0f;									// Maximum degrees of rotation per second

	private float t = 0.0f;													// Time counter for motion
	private bool isDying = false;											// Are we currently dying?
	private float turnTilt = 0.0f;											// Current tilt
	private float turnRotation = 0.0f;										// Current rotation

	/// <summary>
	/// Triggered by the "Locomotion" Slider
	/// </summary>
	/// <param name="newLocomotion">New locomotion.</param>
	public void UpdateLocomotion(float newLocomotion){
		for (int i = 0; i < bats.Length; i++) {													// For each bat
			bats[i].GetComponent<Animator>().SetFloat ("Locomotion", newLocomotion);			// Set the float value on the Animator component
		}
	}

	/// <summary>
	/// This starts the death, saving the start and end position.
	/// </summary>
	public void startDeath(){
		for (int i = 0; i < bats.Length; i++) {															// For each bat
			deathStartPosition[i] = bats[i].transform.position;											// Set start position
			deathEndPosition[i] = new Vector3(deathStartPosition[i].x, .05f, deathStartPosition[i].z);	// Compute end position (for the demo, just the gound at y = .05 [to keep the bat fully above ground])
			isDying = true;																				// Set isDying
			t = 0;																						// Set the time counter t to 0
		}
	}

	void Update(){
		if (isDying) {																								// If we are dying
			t += Time.deltaTime / deathAnimationTime;																// add computed deltaTime to the counter t
			if (t < 1.0) {																							// If we havne't reached the end yet
				for (int i = 0; i < bats.Length; i++) {																// For each bat
					bats [i].transform.position = Vector3.Lerp (deathStartPosition [i], deathEndPosition [i], t);	// Move closer to the end position over time
				}
			} else {																								// If we are at the end
				isDying = false;																					// Set isDying = false
			}
		} else if (t >= 1.0) {																						// If we just ended the death (Do this a frame AFTER the end)
			for (int i = 0; i < bats.Length; i++) {																	// For each bat
				bats [i].transform.position = deathEndPosition [i];													// Set to the end position
			}
		} else {																									// We are still alive!
			for (int i = 0; i < bats.Length; i++) {																	// For each bat
				// We will compute a newAngle.  Angle Y will be rotation -- add the turnRotation multiplied by deltaTime, to the current Y rotation,
				// and then for Angle Z, which is the tilt, simply ensure the bat is at that value.  
				// NOTE:  If we were also doing height, then we would add that for rotation X.  That's out of the scope of
				// the demo, though.
				Vector3 newAngle = new Vector3(0,bats[i].transform.eulerAngles.y + (turnRotation * Time.deltaTime),turnTilt);	
				bats[i].transform.eulerAngles = newAngle;															// Set the turn tilt
			}
		}
	}

	/// <summary>
	/// Triggered by the "Turning" slider, which will pass a value between -1 and 1.
	/// </summary>
	/// <param name="newValue">New value.</param>
	public void UpdateTurning(float newValue){
		turnTilt = maxTurnTilt * -newValue;						// Compute current tilt
		turnRotation = maxTurnRotation * newValue;				// Compute current rotation
	}
}

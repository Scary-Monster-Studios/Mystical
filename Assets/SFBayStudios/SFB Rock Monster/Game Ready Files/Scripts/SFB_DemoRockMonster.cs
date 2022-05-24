using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_DemoRockMonster : MonoBehaviour {

	public GameObject footstep;
	public GameObject clapDust;
	public GameObject[] laser;
	public Transform leftFoot;
	public Transform rightFoot;
	public Transform clap;
	public Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
	}
	
	public void SetLocomotion(float newValue){
		animator.SetFloat ("locomotion", newValue);
	}

	public void LeftFoot(){
		if (leftFoot) {
			GameObject newParticle = Instantiate (footstep, leftFoot.position, Quaternion.identity);
			Destroy (newParticle, 5.0f);
		}
	}

	public void RightFoot(){
		if (rightFoot) {
			GameObject newParticle = Instantiate (footstep, rightFoot.position, Quaternion.identity);
			Destroy (newParticle, 5.0f);
		}
	}

	public void Laser(){
		for (int i = 0; i < laser.Length; i++) {
			if (laser [i]) {
				if (laser [i].GetComponent<ParticleSystem> ()) {
					ParticleSystem.EmissionModule newSystem = laser [i].GetComponent<ParticleSystem> ().emission;
					newSystem.enabled = true;
				} else if (laser [i].GetComponent<Light> ()) {
					laser [i].SetActive (true);
				}
			}
		}
	}

	public void Clap(){
		if (clap) {
			GameObject newParticle = Instantiate (clapDust, clap.position, Quaternion.identity);
			Destroy (newParticle, 5.0f);
		}
	}

	public void LaserStop(){
		for (int i = 0; i < laser.Length; i++) {
			if (laser [i]) {
				if (laser [i].GetComponent<ParticleSystem> ()) {
					ParticleSystem.EmissionModule newSystem = laser [i].GetComponent<ParticleSystem> ().emission;
					newSystem.enabled = false;
				} else if (laser [i].GetComponent<Light> ()) {
					laser [i].SetActive (false);
				}
			}
		}
	}
}

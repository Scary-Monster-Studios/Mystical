using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_DemoGiantWorm : MonoBehaviour {

	public ParticleSystem[] magicSpell;
	public ParticleSystem[] groundParticles;

	public void StartMagicSpell(){
		for (int i = 0; i <magicSpell.Length; i++){
			magicSpell[i].Play();
		}
	}

	public void StopMagicSpell(){
		for (int i = 0; i <magicSpell.Length; i++){
			magicSpell[i].Stop();
		}
	}

	public void StartGround(){
		for (int i = 0; i <groundParticles.Length; i++){
			groundParticles[i].Play();
		}
	}

	public void StopGround(){
		for (int i = 0; i <groundParticles.Length; i++){
			groundParticles[i].Stop();
		}
	}
}

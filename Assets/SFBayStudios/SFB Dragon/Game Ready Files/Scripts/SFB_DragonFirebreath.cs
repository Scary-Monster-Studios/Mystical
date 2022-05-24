using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_DragonFirebreath : MonoBehaviour {

	public GameObject[] magicSpell;

	public void StartSpell(){
		for (int i = 0; i < magicSpell.Length; i++){
			if (magicSpell [i].GetComponent<ParticleSystem> ()) {
				ParticleSystem ms = magicSpell [i].GetComponent<ParticleSystem> ();
				var em = ms.emission;
				em.enabled = true;

			} else if (magicSpell [i].GetComponent<Light> ()) {
				magicSpell [i].GetComponent<Light> ().enabled = true;
			}
		}
	}

	public void EndSpell(){
		for (int i = 0; i < magicSpell.Length; i++){
			if (magicSpell [i].GetComponent<ParticleSystem> ()) {
				ParticleSystem ms = magicSpell [i].GetComponent<ParticleSystem> ();
				var em = ms.emission;
				em.enabled = false;
			} else if (magicSpell [i].GetComponent<Light> ()) {
				magicSpell [i].GetComponent<Light> ().enabled = false;
			}
		}
	}
}

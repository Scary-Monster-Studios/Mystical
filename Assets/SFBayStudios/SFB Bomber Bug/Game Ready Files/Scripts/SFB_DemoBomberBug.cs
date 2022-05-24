using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_DemoBomberBug : MonoBehaviour {

	public Animator charAnimator;
	public float desiredWeight = 0.0f;
	public bool checkTransitionDone = false;
	public Transform spitSpawnPos;
	public GameObject spitParticle;
	public GameObject explodeParticle;
	public Vector3 explodeRotation = new Vector3(-90,0,0);
	public GameObject bodyObj;
	public Material[] bodyMat1;
	public Material[] bodyMat2;

	// Use this for initialization
	void Start () {
		charAnimator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {

/*		if (charAnimator.GetLayerWeight(1) != desiredWeight){
			charAnimator.SetLayerWeight(1, Mathf.MoveTowards(charAnimator.GetLayerWeight(1), desiredWeight, Time.deltaTime * 3));
		}*/

/*		if (charAnimator.IsInTransition (1)) {
			checkTransitionDone = true;
		}*/
		if (checkTransitionDone && !charAnimator.IsInTransition (1)) {
			checkTransitionDone = false;
			if (charAnimator.GetCurrentAnimatorStateInfo (1).IsName ("fly idle")) {
				desiredWeight = 0;
			}
		}
	}

	public void UpdateBodyMaterial(int value){
		Material[] materials = bodyObj.GetComponent<Renderer> ().materials;
		materials [0] = bodyMat1 [value];
		materials [1] = bodyMat2 [value];
		bodyObj.GetComponent<Renderer> ().materials = materials;
	}

	public void Locomotion(float newSpeed){
		charAnimator.SetFloat ("locomotion", newSpeed);
	}

	public void SetBodyLayerWeight(float newWeight){
		desiredWeight = newWeight;
	}

	public void ExplodeBug(){
		GameObject newParticle = Instantiate(explodeParticle, transform.position, Quaternion.identity);
		newParticle.transform.eulerAngles = explodeRotation;
		Destroy(newParticle, 20.0f);
	}

	public void CastSpell(){
		GameObject newParticle = Instantiate(spitParticle, spitSpawnPos.position, Quaternion.identity);
		Destroy(newParticle, 20.0f);
	}
}


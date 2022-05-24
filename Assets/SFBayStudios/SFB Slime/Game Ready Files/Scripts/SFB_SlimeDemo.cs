using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_SlimeDemo : MonoBehaviour {

	public GameObject spitPoint;
	public GameObject spitParticle;
	public ParticleSystem walkParticle;
	public Animator animator;
	public bool isWalking = false;
	public SFB_AudioManager audioController;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		audioController = GetComponent<SFB_AudioManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (animator.GetFloat("locomotion") != 0 && !isWalking)  // If we havne't triggered walking
		{
			isWalking	= true;
			audioController.StartLoop("Walk");
		}
		else if (animator.GetFloat("locomotion") == 0 && isWalking) // if we haven't stopped walking
		{
			isWalking	= false;
			audioController.StopLoop();
		}
	}

	public void SpitAttack(){
		GameObject newSpit = Instantiate(spitParticle, spitPoint.transform.position, Quaternion.identity);
		newSpit.transform.parent = spitPoint.transform;
		newSpit.transform.localPosition = new Vector3(0,0,0);
		Destroy(newSpit, 3.0f);
	}

	public void Locomotion(float newValue){
		animator.SetFloat ("locomotion", newValue);
	}

	public void ParticleTurnOff(){
		walkParticle.Stop ();
	}

	public void ParticleTurnOn(){
		walkParticle.Play ();
	}
}

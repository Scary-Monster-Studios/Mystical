using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFB_DemoIceBall : MonoBehaviour {

	public GameObject explodeParticle;

	void OnCollisionEnter(Collision coll){
		GameObject weHit = coll.gameObject;
		if (weHit.name == "Terrain"){
			ContactPoint contact = coll.contacts[0];
			GameObject newParticle = Instantiate (explodeParticle, contact.point, Quaternion.identity);
			Destroy (newParticle, 5.0f);
			Destroy (gameObject);
		}
	}
}

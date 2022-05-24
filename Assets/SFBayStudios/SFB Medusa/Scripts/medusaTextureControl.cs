using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class medusaTextureControl : MonoBehaviour {

	public int sword1ID = 0;
	public int sword2ID = 0;
	public int bodyID = 0;
	public int armorID = 0;
	public int clothID = 0;

	public List<int> ids = new List<int> ();

	public List<Renderer> sword1 = new List<Renderer> ();
	public List<Renderer> sword2 = new List<Renderer> ();
	public List<Renderer> body = new List<Renderer> ();
	public List<Renderer> armor = new List<Renderer> ();
	public List<Renderer> cloth = new List<Renderer> ();
	public List<Renderer> head = new List<Renderer> ();
	public List<Renderer> tail = new List<Renderer> ();

	public List<Material> sword1Mat = new List<Material> ();
	public List<Material> sword2Mat = new List<Material> ();
	public List<Material> bodyMat = new List<Material> ();
	public List<Material> armorMat = new List<Material> ();
	public List<Material> clothMat = new List<Material> ();
	public List<Material> headMat = new List<Material> ();
	public List<Material> tailMat = new List<Material> ();

	public void NextSword(){
		NextMaterial (0, sword1, sword1Mat);
		NextMaterial (1, sword2, sword2Mat);
	}

	public void NextBody(){
		NextMaterial (2, body, bodyMat);
		NextMaterial (5, head, headMat);
		NextMaterial (6, tail, tailMat);
	}

	public void NextArmor(){
		NextMaterial (3, armor, armorMat);
	}

	public void NextCloth(){
		NextMaterial (4, cloth, clothMat);
	}

	public void NextMaterial(int id, List<Renderer> objects, List<Material> materials){
		ids [id]++;
		if (ids [id] >= materials.Count) {
			ids [id] = 0;
		}
		for (int i = 0; i < objects.Count; i++) {
			Material[] materialsArray = objects [i].materials;
			materialsArray[0] = materials[ids[id]];
			objects [i].materials = materialsArray;
		}
	}
}

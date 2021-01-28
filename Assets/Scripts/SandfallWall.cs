using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandfallWall : MonoBehaviour
{
	public LayerMask layer;
	private BoxCollider collisionBox;

	private void Start() {
		collisionBox = GetComponent<BoxCollider>();
	}

	void Update(){
		// Check box. If overlapping something we care about, disable collisions
		if(Physics.CheckBox(collisionBox.transform.position, collisionBox.transform.localScale/2, collisionBox.transform.rotation, layer)) {
			if(!collisionBox.isTrigger)
				collisionBox.isTrigger = true;
		} else {
			if(collisionBox.isTrigger)
				collisionBox.isTrigger = false;
		}
    }
}

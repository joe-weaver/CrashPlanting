using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnable : MonoBehaviour{

	public ParticleSystem fire;
	public ParticleSystem smoke;

	Burnable[] burnables;
	public float range = 1.5f;

	int heatLevel = 0;
	// The number of levels of heat before this object catches on fire
	public float heatTolerance = 100;
	public bool burning = false;
	public bool burnt = false;

	public bool heatedThisUpdate = false;

	public float burnTime = 10f;
	public float respawnTime = 10f;

	private new Collider collider;

    void Start(){
		// Adjust the scale of the mesh for the smoke
		// (This has to be done in code so the particles aren't scaled weirdly)
		var fireShape = fire.shape;
		fireShape.scale = transform.localScale;

		var smokeShape = smoke.shape;
		smokeShape.scale = transform.localScale;

		heatTolerance *= Random.Range(1f, 3f);

		collider = GetComponent<BoxCollider>();
    }

	public void AddHeatLevel() {
		heatLevel += 1;
		heatedThisUpdate = true;

		if(heatLevel > heatTolerance) {
			StartBurning();
		}
	}

	void StartBurning() {
		fire.Play();
		smoke.Play();
		burning = true;
		Invoke("BurnUp", burnTime);
	}

	void StopBurning() {
		fire.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		smoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	private void Respawn() {
		burnt = false;
		heatLevel = 0;
		gameObject.SetActive(true);
	}

	private void BurnUp() {
		burning = false;
		burnt = true;
		gameObject.SetActive(false);

		// Respawn after a time
		Invoke("Respawn", respawnTime);
	}

	public void SetBurnables(Burnable[] burnables) {
		this.burnables = burnables;
	}

	private void Update() {
		if(burning) {
			HeatBurnablesInRange();
		}
	}

	private void LateUpdate() {
		// Prepare for next update
		heatedThisUpdate = false;
	}

	void HeatBurnablesInRange() {
		if(burnables == null) return;

		foreach(Burnable burnable in burnables) {
			if(burnable != null && !burnable.burning && !burnable.heatedThisUpdate && !burnable.burnt) {
				// Check the shortest distance between the two objects
				Vector3 closestPosOther = burnable.collider.ClosestPoint(transform.position);
				Vector3 closestPosThis = collider.ClosestPoint(burnable.transform.position);

				float dist = Vector3.Distance(closestPosOther, closestPosThis);
				if(dist < range) {
					burnable.AddHeatLevel();
				}
			}
		}
	}

	public Collider getCollider() {
		return collider;
	}
}

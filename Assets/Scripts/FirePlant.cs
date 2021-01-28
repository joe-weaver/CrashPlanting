using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePlant : Plant
{
	Burnable[] burnables;
	public float range = 1.5f;

    public void SetBurnables(Burnable[] burnables) {
		this.burnables = burnables;
	}

	public override void handlePlantFunction()
    {
		HeatBurnablesInRange();
    }

	void HeatBurnablesInRange() {
		if(burnables == null) return;

		foreach(Burnable burnable in burnables) {
			if(burnable != null && !burnable.burning && !burnable.heatedThisUpdate && !burnable.burnt) {
				// Check the distance from this to the closest position on the burnable object
				Vector3 closestPoint = burnable.getCollider().ClosestPoint(transform.position);

				float dist = Vector3.Distance(transform.position, closestPoint);
				if(dist < range) {
					burnable.AddHeatLevel();
				}
			}
		}
	}
}

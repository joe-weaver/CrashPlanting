using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElephantEar : PickupPlant
{
	public override bool Drop(Transform holder, Vector3 movementDirection, Vector3 holderVelocity, bool isGrounded, float throwSpeed) {
		// Try to raycast to the ground infront of player
		Vector3 castPos = holder.position + holder.forward;
		RaycastHit ground;

		if(Physics.Raycast(castPos, Vector3.down, out ground, 2f)) {
			base.Drop(holder, movementDirection, holderVelocity, isGrounded, throwSpeed);
			body.constraints = RigidbodyConstraints.FreezeAll;			
			body.transform.position = ground.point;
			return true;
		} else {
			return false;
		}
	}
}

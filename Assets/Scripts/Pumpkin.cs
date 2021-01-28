using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pumpkin : PickupPlant
{

	public float collisionSpeedSoundCutoff = 2f;

	public override bool Drop(Transform holder, Vector3 movementDirection, Vector3 holderVelocity, bool isGrounded, float throwSpeed) {
		base.Drop(holder, movementDirection, holderVelocity, isGrounded, throwSpeed);

		body.velocity = holderVelocity;

		if(movementDirection != Vector3.zero || !isGrounded) {
			Vector3 forceDir = (movementDirection != Vector3.zero ? movementDirection.normalized : movementDirection.normalized / 4.0f) + (isGrounded ? Vector3.zero : Vector3.up);
			forceDir.x *= throwSpeed;
			forceDir.y *= 2.0f;
			forceDir.z *= throwSpeed;
			body.AddForce(forceDir, ForceMode.VelocityChange);
		}
		// We always throw the pumpkin
		return true;
	}

	private void OnCollisionEnter(Collision collision) {
		if(collision.relativeVelocity.magnitude > collisionSpeedSoundCutoff) {
			FindObjectOfType<AudioManager>().Play("Pumpkin");
		}
	}
}

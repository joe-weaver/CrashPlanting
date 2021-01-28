using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class PickupPlant : MonoBehaviour, Liftable {
	public Vector3 holdPosition;
	protected Rigidbody body;

	protected virtual void Start() {
		body = GetComponent<Rigidbody>();
	}

	public virtual void Lift(Transform holder) {
		// Parent the plant to the player
		body.transform.parent = holder;

		// Freeze physics movement and remove collisions
		body.constraints = RigidbodyConstraints.FreezeAll;
		body.detectCollisions = false;

		// Make it look like we're holding the plant
		body.transform.rotation = holder.rotation; //Quaternion.Euler(0, 0, 0);
		body.transform.localPosition = holdPosition;
	}

	public virtual bool Drop(Transform holder, Vector3 movementDirection, Vector3 holderVelocity, bool isGrounded, float throwSpeed) {
		// Place the plant back into the scene
		body.transform.parent = holder.parent;

		// Reset the constraints and collision detection
		body.constraints = RigidbodyConstraints.None;
		body.detectCollisions = true;

		// We threw the plant, so return true
		return true;
	}
}

public interface Liftable {
	void Lift(Transform holder);

	bool Drop(Transform holder, Vector3 movementDirection, Vector3 holderVelocity, bool isGrounded, float throwSpeed);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blowable : MonoBehaviour {

	protected Rigidbody body;

	private void Start() {
		body = GetComponent<Rigidbody>();
	}

	public virtual void HandleWind(float windPower, Transform windProducer) {
		print("Blowing");

		Vector3 forward = windProducer.rotation * Vector3.forward;

		Vector3 force = windPower * forward.normalized;

		body.AddForce(force, ForceMode.Acceleration);
	}
}

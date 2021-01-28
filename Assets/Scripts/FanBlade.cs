using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanBlade : Blowable {
	public override void HandleWind(float windPower, Transform windProducer) {
		print("Blowing");

		Vector3 forward = windProducer.rotation * Vector3.forward;

		// Get position on fan blade
		RaycastHit hit;

		Physics.Raycast(windProducer.position + new Vector3(0, 1, 0), forward, out hit);

		Debug.DrawRay(windProducer.position + new Vector3(0, 1, 0), forward.normalized * 5, Color.green);

		if(hit.collider != null) {
			// If it's this we hit
			if(hit.collider.gameObject.TryGetComponent<FanBlade>(out FanBlade thisObj)) {
				// Apply force at position on fan blade
				Vector3 force = windPower * forward.normalized;

				print("Hit");

				body.AddForceAtPosition(force, hit.point, ForceMode.Force);
			}
		}
	}
}

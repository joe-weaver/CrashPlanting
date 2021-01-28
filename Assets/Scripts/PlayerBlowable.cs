using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlowable : Blowable
{
	public PlayerControllerRB player;

	public override void HandleWind(float windPower, Transform windProducer) {
		print("Blowing");

		Vector3 forward = windProducer.rotation * Vector3.forward;

		Vector3 force = windPower * forward.normalized / 2f;

		if(player.HoldingPumpkin()) {
			force /= 4f;
		}

		body.AddForce(force, ForceMode.Acceleration);
	}
}

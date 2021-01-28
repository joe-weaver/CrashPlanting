using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounceable : MonoBehaviour
{
    public float bounceForce = 1f;
	private Rigidbody body;
	private float[] prevFallSpeeds = new float[5];

	private void Start() {
		body = GetComponent<Rigidbody>();

		for(int i = 0; i < prevFallSpeeds.Length; i++) {
			prevFallSpeeds[i] = 1f;
		}
	}

	public void Bounce()
    {
        //Only bounce if traveling downwards on mushroom. -.001 is used to prevent bouncing when the player has downward velocity due mesh collision
        if (body.velocity.y < -.001f)
        {
			float defaultMushroomVelocity = Mathf.Sqrt(bounceForce * 1 * -8f * Physics.gravity.y);

			if(GetComponent<PlayerControllerRB>() != null) {
				// The player doesn't bounce as high
				defaultMushroomVelocity = Mathf.Sqrt(bounceForce * 1 * -4f * Physics.gravity.y);
			}
			

            //Reduce jump height if it is the player and they are holding a pumpkin
            float multiplier = GetComponent<PlayerControllerRB>() != null && GetComponent<PlayerControllerRB>().HoldingPumpkin() ? .7f : 1;

			float count = 0;
			float sum = 0;

			for(int i = 0; i < prevFallSpeeds.Length; i++) {
				if(prevFallSpeeds[i] < -0.001f) {
					sum += prevFallSpeeds[i];
					count += 1;
				}
			}

			float avgFallSpeed = sum / count;

			// If the player jumps from a 'high' position, use that velocity instead.
			float realVelocity = Mathf.Max(defaultMushroomVelocity * multiplier, -avgFallSpeed);

            body.velocity = new Vector3(body.velocity.x, realVelocity, body.velocity.z);
        }
    }

	private void FixedUpdate() {
		// Update the list of velocities
		int index = prevFallSpeeds.Length - 2;

		while(index > -1) {
			prevFallSpeeds[index + 1] = prevFallSpeeds[index];
			index -= 1;
		}

		prevFallSpeeds[0] = body.velocity.y;
	}

	public float getFallSpeed() {
		float count = 0;
		float sum = 0;

		for(int i = 0; i < prevFallSpeeds.Length; i++) {
			if(prevFallSpeeds[i] < -0.001f) {
				sum += prevFallSpeeds[i];
				count += 1;
			}
		}

		float avgFallSpeed = sum / count;

		return avgFallSpeed;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Plant
{
	public float minBounceSoundTime = 0.3f;
	private float timeSinceLastBounce = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        Bounceable bounceable = collision.gameObject.GetComponent<Bounceable>();
        if (bounceable == null) return;

        bounceable.Bounce();

		if(timeSinceLastBounce > minBounceSoundTime) {
			FindObjectOfType<AudioManager>().Play("Bounce");
		}
    }

	protected void Update() {
		base.Update();
		timeSinceLastBounce += Time.deltaTime;
	}
}

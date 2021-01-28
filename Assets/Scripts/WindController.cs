using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    public float windPower = .5f;

    private void OnTriggerStay(Collider other)
    {   
        //All affected objects must have blowable component attached
        Blowable script = other.GetComponent<Blowable>();

        if (script != null){
			script.HandleWind(windPower, transform);
		}
    }
}

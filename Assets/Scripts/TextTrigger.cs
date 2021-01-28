using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTrigger : MonoBehaviour
{
    public int textEventNum;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerControllerRB>() == null) return;

        TextController.GetTextController().Activate(textEventNum);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedPacket : MonoBehaviour
{
	public int plantType;

    void OnCollisionEnter(Collision collision)
    {
        //Unlock the plant if a player 'collects' seed packet
        if (collision.gameObject.GetComponent<PlayerControllerRB>() == null) return;

        InventoryManager.inventory.unlockPlant(plantType);

        TextController.GetTextController().ActivatePlant(plantType);

        Destroy(this.gameObject);
    }
}

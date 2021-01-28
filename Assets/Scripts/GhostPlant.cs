using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlant : MonoBehaviour
{
    public Plant realPlant;
	public InventoryManager inventory;
	public static Material validMat;
    public static Material invalidMat;

	private Renderer rend;
    private Material[] invalidMatArray;
    private Material[] validMatArray;
    private bool isValidLocation = true;

	private bool notInitiallyOnPlantableGround = false;

    //List of all current colliding objects to check if a plant interferes with other objects
    //If a plant collides with another plant or with terrain, there are very buggy interactions with the player
    private bool isColliding = false;
    private HashSet<Collider> collisionList = new HashSet<Collider>();

    void Start() {
        rend = GetComponentInChildren<Renderer>();
        invalidMatArray = new Material[rend.sharedMaterials.Length];
        validMatArray = new Material[rend.sharedMaterials.Length];

        for(int i = 0; i < rend.sharedMaterials.Length; i++) {
            invalidMatArray[i] = invalidMat;
            validMatArray[i] = validMat;
        }

		// Check if this ghost plant has more than one item in the inventory
		if(!inventory.canPlantSelected() || notInitiallyOnPlantableGround) {
			print("Created ghost plant, but not initially plantable");
			SetInvalidMaterial(true);
		} else {
			print("Plantable");
		}

	}

	public void setNotInitiallyOnPlantableGround(bool value) {
		notInitiallyOnPlantableGround = value;
	}

	public void OnTriggerEnter(Collider other) {
        //If we were not previously colliding with anything, we are now
        if (collisionList.Count == 0) {
            isColliding = true;
            SetInvalidMaterial(true);
        }

        collisionList.Add(other);
    }

    public void OnTriggerExit(Collider other) {
        collisionList.Remove(other);

        //If we are no longer colliding with anything, attempt to set the valid material
        if (collisionList.Count == 0) {
            isColliding = false;
            SetValidMaterial(true);
        }
    }

    public bool Plant(Transform playerTransform, Vector3 plantGroundPoint, Burnable[] burnables) {
        if (isColliding) return false;

        Plant plant = Instantiate(realPlant, plantGroundPoint, playerTransform.rotation);
        plant.setIsPlantedByPlayer();

        if (plant is FirePlant) {
			(plant as FirePlant).SetBurnables(burnables);
		}

        return true;
    }

    //This may be called by the GhostPlant when there are no collisions
    public void SetInvalidMaterial(bool calledBySelf) {
        //If called by player, this means the plant is in an invalid location
        if (!calledBySelf) isValidLocation = false;

        rend.sharedMaterials = invalidMatArray;
    }

    public void SetValidMaterial(bool calledBySelf) {
        //If called by player, this means the plant is in a valid location and has enough plants left
        if (!calledBySelf) isValidLocation = true;

        //Only use the change the material if the plant is not colliding with anything AND is in a valid location
        if(isValidLocation && !isColliding)
            rend.sharedMaterials = validMatArray;
    }
}

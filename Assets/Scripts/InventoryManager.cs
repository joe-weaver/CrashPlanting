using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {
	public Image[] plantIcons;
	bool[] plantUnlocked;
	int[] plantMaxCount;
	int[] plantCount;
	public Image selector;
	int selectedPlant;
	bool hasUnlockedPlants = false;

	//Singleton InventoryManager to be referenced by plants
	public static InventoryManager inventory;

	public static InventoryManager GetInventoryManager() {
		return inventory;
    }

	void Start() {
		inventory = this;
		plantUnlocked = new bool[plantIcons.Length];
		plantMaxCount = new int[plantIcons.Length];
		plantCount = new int[plantIcons.Length];

		// Initialize all of our arrays and set the plants to be grayed out
		for(int i = 0; i < plantIcons.Length; i++) {
			plantIcons[i].color = new Color(0, 0, 0, 0.25f);
			plantUnlocked[i] = false;
			plantMaxCount[i] = 0;
			plantCount[i] = 0;
		}

		// Hide the selector
		selectedPlant = 0;
		selector.rectTransform.anchoredPosition = new Vector2(-100, selector.rectTransform.anchoredPosition.y);

		// FOR DEV - UNLOCK ALL PLANTS
		for(int i = 0; i < plantIcons.Length; i++) {
			unlockPlant(i);
		}
	}

	public void setSelection(int index) {
		// Fade out current selection
		plantIcons[selectedPlant].color = new Color(1, 1, 1, 0.5f);

		// Update gui for new selection
		selectedPlant = index;
		selector.rectTransform.anchoredPosition = new Vector2(index * 32, selector.rectTransform.anchoredPosition.y);
		plantIcons[index].color = new Color(1, 1, 1, 1);
	}

	public void unlockPlant(int index) {
		print("Unlocked Plant: " + index);

		// Update our arrays for this plant
		plantUnlocked[index] = true;
		plantMaxCount[index] = 1;
		plantCount[index] = 1;

		// Update the UI
		plantIcons[index].color = new Color(1, 1, 1, 0.5f);
		plantIcons[index].GetComponentInChildren<TMPro.TMP_Text>().text = 1.ToString();

		// Check if this is the first unlock
		if(!hasUnlockedPlants) {
			// First unlock, so select our only plant
			setSelection(index);
			hasUnlockedPlants = true;
		}
	}

	public void addMaxCount(int index) {
		plantMaxCount[index] += 1;
		plantCount[index] += 1;

		plantIcons[index].GetComponentInChildren<TMPro.TMP_Text>().text = plantCount[index].ToString();
	}

	public int getSelection() {
		return selectedPlant;
	}

	public void placeSelected() {
		print("Placed Plant: " + selectedPlant);

		plantCount[selectedPlant] -= 1;
		plantIcons[selectedPlant].GetComponentInChildren<TMPro.TMP_Text>().text = plantCount[selectedPlant].ToString();
	}

	public void removePlant(int index) {
		print("Removed Plant: " + index);

		plantCount[index] += 1;
		plantIcons[index].GetComponentInChildren<TMPro.TMP_Text>().text = plantCount[index].ToString();
	}

	public bool isUnlocked(int index) {
		return plantUnlocked[index];
	}

	public bool canPlantSelected() {
		return plantCount[selectedPlant] > 0;
	}

	public bool canPlant() {
		return hasUnlockedPlants;
	}

	public void scroll(float direction) {
		if(direction < 0) {
			// Negative scroll, go right
			int index = selectedPlant + 1;
			if(index >= plantIcons.Length) {
				index = 0;
			}

			// Scroll to next selected, loop around to other side if needed
			while(!isUnlocked(index)) {
				index += 1;
				if(index >= plantIcons.Length) {
					index = 0;
				}
			}

			setSelection(index);
		} else {
			// Positive scroll, go left
			int index = selectedPlant - 1;
			if(index < 0) {
				index = plantIcons.Length - 1;
			}

			// Scroll to next selected, loop around to other side if needed
			while(!isUnlocked(index)) {
				index -= 1;
				if(index < 0) {
					index = plantIcons.Length - 1;
				}
			}

			setSelection(index);
		}
	}
}

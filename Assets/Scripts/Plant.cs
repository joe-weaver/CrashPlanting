using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public float lifetime = 10f;
    
    public int index;

    public Jun_TweenRuntime growTween;
    public Jun_TweenRuntime lifetimeTween;
    public Jun_TweenRuntime dieTween;

    //0 = Growing
    //1 = Idle
    //2 = Dying
    private int plantState;
    private bool isPlantedByPlayer = false;
    private bool doesDecay;
    private float growAnimationTime;
    private float dieAnimationTime;
    private InventoryManager inventoryManager;

    // Start is called before the first frame update
    void Start()
    {
        print(gameObject.name + " planted");

        inventoryManager = InventoryManager.GetInventoryManager();

        //Disable any components problematic to the tweening
        DisableComponents();

        plantState = 0; 
        growAnimationTime = growTween.animationTime;
        dieAnimationTime = dieTween.animationTime;

        if (lifetime > 0)
            doesDecay = true;
        else
            doesDecay = false;
    }

    // Update is called once per frame
    protected void Update() {
        switch (plantState)
        {
            case 0:
                //Do grow sequence for growAnimationTime
                if (!growTween.isPlaying) growTween.Play();
                if(growAnimationTime > 0) {
                    growAnimationTime -= Time.deltaTime;
                }else {
                    EnableComponents();
                    plantState = 1;
                }

                break;

            case 1:
                //After lifetime, destory plant
                handlePlantFunction();
                if(doesDecay)
                    lifetime -= Time.deltaTime;

                bool isDestroyedByPlayer = isPlantedByPlayer && Input.GetKeyDown(KeyCode.R) && inventoryManager.getSelection() == index;
                

                if (lifetime < 0 || isDestroyedByPlayer) {
                    plantState = 2;
                    //DisableComponents();
                }

                break;

            case 2:
				//Do die sequence for dieAnimationTime
				if(!dieTween.isPlaying) {
					dieTween.Play();
					FindObjectOfType<AudioManager>().Play("LowPop");
				}

				if(dieAnimationTime > 0) {
                    dieAnimationTime -= Time.deltaTime;
                }else {
                    Destroy(gameObject);
                }
                
                break;
        }
    }

    void OnDestroy() {
		//Needs to add back to the player's inventory
		print("Destroyed Plant");
        InventoryManager.GetInventoryManager().removePlant(index);
    }

    public void setIsPlantedByPlayer() {
        isPlantedByPlayer = true;
    }

    public virtual void handlePlantFunction() {}

    //Disable any problematic compenents for tweening
    private void DisableComponents()
    {
        if(GetComponent<Rigidbody>() != null)
            GetComponent<Rigidbody>().isKinematic = true;
        //Do we want the trumpet to begin blowing immediately? May be better interaction if this is not disabeld on trumpet
        foreach (Collider c in GetComponents<Collider>())
            c.enabled = false;

    }

    private void EnableComponents()
    {
        if (GetComponent<Rigidbody>() != null)
            GetComponent<Rigidbody>().isKinematic = false;
        foreach (Collider c in GetComponents<Collider>())
            c.enabled = true;
    }

}

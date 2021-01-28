using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    //Should probably use a different color for each type of switch so the user has a visual cue of what to expect

    //Toggled switches flip between on / off indefinitely every time they are hit
    //   If the default and final positions are set to the same transform, this can be used as a 'reset' button that moves an object back to a certain position
    //Timed switches turn on for the specified time whenever they are hit
    //Pressure activated switches turn on while they are being stepped on
    public enum SwitchType {Toggled, Timed, PressureActivated}

    public SwitchType switchType;
    public GameObject affectedObject;
    public Transform objectDefaultTransform;
    public Transform objectFinalTransform;
    
    public float activationTime = 0f;

    private bool isActivated;

    //Toggled Switch Counters
    //Not sure yet

    //Timed Switch Counters
    private float timeRemaining;

    //Pressure Activated Counters
    private int numCollisions;

    // Start is called before the first frame update
    void Start() {
        isActivated = false;
        timeRemaining = 0f;
        numCollisions = 0;
    }

    public void OnCollisionEnter(Collision collision) {
        switch(switchType)
        {
            case SwitchType.Toggled:
                isActivated = !isActivated;

                if (isActivated)
                    setObjectFinalPosition();
                else
                    setObjectDefaultPosition();

                break;

            case SwitchType.Timed: 
                if (isActivated) return;

                isActivated = true;
                timeRemaining = activationTime;
                setObjectFinalPosition();

                break;

            case SwitchType.PressureActivated:
                //If the switch is going from off -> on, move the object to its new position
                if(numCollisions == 0) {
                    isActivated = true;
                    setObjectFinalPosition();
                }

                numCollisions++;

                break;
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        switch (switchType)
        {
            case SwitchType.PressureActivated:
                numCollisions--;

                //If nothing is no longer on the switch, move the object back to original position
                if(numCollisions == 0)
                {
                    isActivated = false;
                    setObjectDefaultPosition();
                }
                break;
        }
    }

    // Update is called once per frame
    void Update() {
        if (!isActivated) return;

        switch(switchType)
        {
            case SwitchType.Timed:
                //Maybe we can also play a timer ticking noise
                timeRemaining -= Time.deltaTime;

                //If timer has expired, reset back to start
                if(timeRemaining < 0)
                {
                    //Player a timer finished noise
                    isActivated = false;
                    setObjectDefaultPosition();
                }

                break;
        }
    }

    //Later we can interpolate the difference so that it slides / rotates between positions rather than snapping
    void setObjectDefaultPosition()
    {
        affectedObject.transform.position = objectDefaultTransform.position;
        affectedObject.transform.rotation = objectDefaultTransform.rotation;
        affectedObject.transform.localScale = objectDefaultTransform.localScale;
    }

    void setObjectFinalPosition()
    {
        affectedObject.transform.position = objectFinalTransform.position;
        affectedObject.transform.rotation = objectFinalTransform.rotation;
        affectedObject.transform.localScale = objectFinalTransform.localScale;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    //This is a very janky solution to fading in text


    // Start is called before the first frame update
    public static TextController singleton;
    public GameObject[] texts;
    public int[] plantCollectEventIndices;

    private int currEvent = 0;

    public static TextController GetTextController() {
        if (singleton == null) singleton = new TextController();
        return singleton;
    }
    void Start()
    {
        singleton = this;

        foreach(GameObject o in texts) {
            foreach (Text t in o.GetComponentsInChildren<Text>())
            {
                t.canvasRenderer.SetAlpha(0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(currEvent)
        {
            case 0:
                //How to enter planting mode
                break;
            case 1:
                //Display how to plant text when the player presses tab
                if (Input.GetKeyDown(KeyCode.Tab))
                    Activate();
                break;
        }
    }

    public void Activate()
    {
        foreach (Text t in texts[currEvent].GetComponentsInChildren<Text>())
        {
            t.CrossFadeAlpha(1, 1.25f, false);
        }
        currEvent++;
    }

    public void Activate(int index)
    {
        for(int i = currEvent; i <= index; i++){
             Activate();
        }
    }

    public void ActivatePlant(int plantNum)
    {
        //Display all the tutorial messages up until the plant collect event (Plant info is the most important to show in case of errors)
        Activate(plantCollectEventIndices[plantNum]);
    }

    public void Deactivate(int index)
    {
        foreach (Text t in texts[index].GetComponentsInChildren<Text>())
        {
            t.CrossFadeAlpha(0, 2, false);
        }
    }
}

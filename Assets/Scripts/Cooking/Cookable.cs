using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookable : MonoBehaviour
{
    /*
    PURPOSE: This script provides "cooking" functionality to the object it is attached to.
    How it works: There are three states:
        - raw
        - cooked
        - burned
    
    These three states are gone over when a timer increases. 
    This timer increases only if the object is in constant contact with a CookingStation tagged object.
    
    There are different colours associated with the 3 stages (currently hard-coded), while there are also
    bools associated with the 3 states. raw (isCooked = isBurned = false), cooked(isCooked = true),
    burned (isCooked = isBurned = true).
    */

    public float cookingTime; // time from raw to cook
    public float burnTime; // time from cook to burn
    private float cookTime; // time that it has been cooked for
    public Color[] colors; // list of colours that we will use
    public bool isCooked;
    public bool isBurned;
    private Renderer burgerRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // setting vars
        cookingTime = 2.0f;
        burnTime = cookingTime * 2.0f;
        cookTime = 0.0f;
        isCooked = false;
        isBurned = false;
        // private var burgerRenderer;
        colors = new Color[3];
        colors[0] = new Color(245.0f / 255.0f, 156.0f / 255.0f, 156.0f / 255.0f, 1.0f);
        colors[1] = new Color(200.0f / 255.0f, 139.0f / 255.0f, 58.0f / 255.0f, 1.0f);
        colors[2] = new Color(0.0f,0.0f,0.0f,0.0f);
        burgerRenderer = GetComponent<Renderer>(); 
        burgerRenderer.material.SetColor("_Color", colors[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (cookTime > cookingTime)
        {
            if (cookTime > burnTime)
            {
                burgerRenderer.material.SetColor("_Color", colors[2]);
                isBurned = true;
            }
            else
            {
                burgerRenderer.material.SetColor("_Color", colors[1]);
                isCooked = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<CookTop>(out CookTop cookTop))
        {
            cookTime += Time.deltaTime;
        }
    }
}

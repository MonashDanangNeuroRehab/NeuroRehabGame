using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class OrderDisplayScript : MonoBehaviour
{
    private int orderNumber;
    public GameObject[] orderDisplays;
    public float displayGenerationDelay = 10f;
    private float displayTimer = 0f;

    // Declare int for order number
    public bool[] orderDisplaysCompleted; // Pinged if order is completed
    
    // Declare bools for whether display should be active
    // public bool[] orderDisplaysActive;

    // Declare text objects NUMBER 1
    public TextMeshProUGUI[] orderDisplaysOrderNumber;
    public TextMeshProUGUI[] orderDisplaysTopIngredient;
    public TextMeshProUGUI[] orderDisplaysMidIngredient;
    public TextMeshProUGUI[] orderDisplaysBotIngredient;
    public string[,] burgerList = new string[6, 4]; // Max order is set at 6 for now

    // Declare list of potential fillings
    // Note - this may need to be touched up more later. At present, this is just a string. 
    // It will need to be compared to the actual outgoing order later for confirmation
    public string[] ingredients;
    public string[,] ingredientsList = new string[3,3];

    // Start is called before the first frame update
    void Start()
    {
        orderNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // go through each
        for (int i = 0; i < orderDisplays.Length; i++)
        {
            // Debug.Log("in loop, i = " + i);
            // if it's not active, and the timer if less than 0 then you should generate the display, and return all counts to 0
            // if it's not active, then increase timer just for this one.
            // if it is active, continue to next loop

            if (orderDisplays[i].activeSelf)
            {
                // Debug.Log("Display number " + i + " has self state of" + orderDisplays[i].activeSelf);
                continue; // Active so no need to check
            }
            else if (!orderDisplays[i].activeSelf && displayTimer < displayGenerationDelay)
            {
                // if it's not active and timer hasn't crosesd over, then you increment time and break
                // Debug.Log("Incrementing, timer is" + displayTimer);
                displayTimer += Time.deltaTime;
                break;
            }
            else
            {
                // Debug.Log("Activating Display Number" + i);
                // order has to be generated for this display
                orderNumber += 1;
                GenerateDisplay(i, orderNumber, ingredients, ingredientsList);
                displayTimer = 0f;
                break;
            }
        }

        for (int i = 0; i < orderDisplays.Length; i++)
        {
            if (orderDisplaysCompleted[i])
            {
                orderDisplays[i].gameObject.SetActive(false);
                ingredientsList[i, 0] = "";
                ingredientsList[i, 1] = "";
                ingredientsList[i, 2] = "";
                orderDisplaysCompleted[i] = false;
            }
        }

        // HARD CODE - TO DELETE LATER
        if (Input.GetKeyDown("a"))
        {
            orderDisplays[0].gameObject.SetActive(false);
        }
        if (Input.GetKeyDown("b"))
        {
            orderDisplays[1].gameObject.SetActive(false);
        }

    }



    public void GenerateDisplay(int displayNumber, int orderNumber, string[] ingredients, string[,] ingredientsList)
    {
        orderDisplays[displayNumber].gameObject.SetActive(true);
        orderDisplaysOrderNumber[displayNumber].text = "Order #: " + orderNumber;
        // Select the ingredients for the ingredients Top, Mid, Bot
        List<string> burgerIngredients = new List<string>(GenerateIngredients(ingredients));
        burgerList[displayNumber, 0] = "BottomBun";
        for (int j = 0; j < burgerIngredients.Count; j++)
        {
            burgerList[displayNumber, j+1] = burgerIngredients[j];
        }
        burgerList[displayNumber, 3] = "TopBun";
        // assign them to the display text
        orderDisplaysTopIngredient[displayNumber].text = "Top: " + burgerIngredients[0];
        orderDisplaysMidIngredient[displayNumber].text = "Mid: " + burgerIngredients[1];
        orderDisplaysBotIngredient[displayNumber].text = "Bot: " + burgerIngredients[2];

        // Current hard code - TO DELETE LATER
        orderDisplaysTopIngredient[0].text = "Top: Tomato";
        orderDisplaysMidIngredient[0].text = "Mid: Lettuce";
        orderDisplaysBotIngredient[0].text = "Bot: Burger";

        // return the display so they are assigned out of function
        ingredientsList[displayNumber, 0] = burgerIngredients[0];
        ingredientsList[displayNumber, 1] = burgerIngredients[1];
        ingredientsList[displayNumber, 2] = burgerIngredients[2];
    }


    private string[] GenerateIngredients(string[] ingredients)
    {
        string[] chosenIngredients = new string[3];
        List<string> availableIngredients = new List<string>(ingredients);

        // Choose a random index for sugar
        int burgerIndex = Random.Range(0, 3);

        // Place sugar at the randomly chosen index
        chosenIngredients[burgerIndex] = "Burger";
        availableIngredients.RemoveAt(availableIngredients.IndexOf("Burger"));

        // Fill the remaining two ingredients
        for (int i = 0; i < 3; i++)
        {
            if (i == burgerIndex)
            {
                continue;
            }
            int index = Random.Range(0, availableIngredients.Count);
            chosenIngredients[i] = availableIngredients[index];
            availableIngredients.RemoveAt(index);
        }

        return chosenIngredients;
    }
}

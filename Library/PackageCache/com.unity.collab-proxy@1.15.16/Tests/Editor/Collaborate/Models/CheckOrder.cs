using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckOrder : MonoBehaviour
{
    [SerializeField]
    OrderDisplayScript orderDisplayScript;
    [SerializeField]
    Food food;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OrderCompleted()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (orderDisplayScript.burgerList[i, j] == food.userArray[j])
                {
                    // Delete burger and gain points
                    // Send message "You have completed order "i"!"
                }
                else
                {
                    // Delete burger but no points gained
                    // Send message "Wrong Ingredients!"
                }
            }
        }
    }

}

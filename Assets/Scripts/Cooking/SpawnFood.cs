using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    public GameObject bottomBun;
    public GameObject tomato;
    public GameObject burger;
    public GameObject cheese;
    public GameObject lettuce;
    public GameObject topBun;
    private GameObject tempBottomBun;
    private GameObject tempTomato;
    private GameObject tempBurger;
    private GameObject tempCheese;
    private GameObject tempLettuce;
    private GameObject tempTopBun;


    public GameObject cutLettuce;
    private GameObject tempCutLettuce;

    // Start is called before the first frame update
    void Start()
    {
        tempBottomBun = Instantiate(bottomBun, new Vector3(-0.267f, 2.669545f, -2.4629f), Quaternion.identity);
        tempTomato = Instantiate(tomato, new Vector3(0.08089997f, 2.669545f, -2.4629f), Quaternion.identity);
        tempBurger = Instantiate(burger, new Vector3(-0.6165f, 2.669545f, -2.4629f), Quaternion.identity);
        tempCheese = Instantiate(cheese, new Vector3(-0.4417f, 2.669545f, -2.4629f), Quaternion.identity);
        tempLettuce = Instantiate(lettuce, new Vector3(-0.0849f, 2.669545f, -2.4629f), Quaternion.identity);
        tempTopBun = Instantiate(topBun, new Vector3(0.6174f, 2.669545f, -2.4629f), Quaternion.identity);
    }
    
    // Update is called once per frame
    void Update()
    {
        // HARD CODE
        if (Input.GetKeyDown("space"))
        {
            Destroy(tempBottomBun);
            Destroy(tempTopBun);
            Destroy(tempBurger);
            Destroy(tempCheese);
            Instantiate(tempTopBun, new Vector3(0.6174f, 2.6969f, -2.4629f), Quaternion.identity);
        }
    }
}

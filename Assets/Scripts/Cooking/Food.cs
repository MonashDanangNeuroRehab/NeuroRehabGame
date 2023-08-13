using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    /*    private void OnTriggerEnter(Collider other)
        {

            if (other.TryGetComponent<Food>(out Food food))
            {
                // Debug.Log("Setting as parent");
                // Whichever is higher is set as the child
                if (other.transform.position.y > gameObject.transform.position.y)
                {
                    // make the other object the child of this one
                    other.transform.SetParent(gameObject.transform);
                }
                else
                {
                    gameObject.transform.SetParent(other.transform);
                }

                //rigidBody.isKinematic = true;
            }
        }*/



    public string[] userArray = new string[4];
    private Vector3 validDirection = Vector3.up;
    private float contactThreshold = 20;               // Acceptable difference in degrees
    public bool canStick = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCollisionEnter(Collision collision)
    {
        var collisionMultiTag = collision.gameObject.GetComponent<CustomTag>();
        if (collisionMultiTag != null && (collision.gameObject.CompareTag("Food") || collisionMultiTag.HasTag("Food")))
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (Vector3.Angle(collision.GetContact(i).normal, validDirection) <= contactThreshold)
                {
                    // When collided with a surface facing mostly upwards, freeze position and rotation
                    canStick = true;
                    StackObject();
                }
            }
            /*var multiTag = gameObject.GetComponent<CustomTag>();
            if (multiTag != null)
            {
                if (multiTag.HasTag("BottomBun"))
                {
                    userArray[0] = "BottomBun";
                }
                else if (multiTag.HasTag("Cheese"))
                {
                    userArray[1] = "Cheese";
                }
                else if (multiTag.HasTag("Lettuce"))
                {
                    userArray[2] = "Lettuce";
                }
                else if (multiTag.HasTag("Burger"))
                {
                    userArray[3] = "Burger";
                }
                else if (multiTag.HasTag("Tomato"))
                {
                    // Empty for now - change all conditions later
                }
                else if (multiTag.HasTag("TopBun"))
                {
                    userArray[4] = "TopBun";
                }
                Debug.Log(string.Join(", ", userArray));
            }*/
        }
    }

    public void StackObject()
    {
        if (canStick == true)
        {
            // Freezes the position and rotation of the object
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }
    }


}


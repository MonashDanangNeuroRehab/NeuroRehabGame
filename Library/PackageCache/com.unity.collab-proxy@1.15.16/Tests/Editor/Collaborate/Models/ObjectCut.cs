using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Geometry;
using UnityEngine;

public class ObjectCut : MonoBehaviour
{
    public GameObject cutLettuce;
    private GameObject tempCutLettuce1;
    private GameObject tempCutLettuce2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Destroy(tempCutLettuce1);
            Destroy(tempCutLettuce2);
        }
    }

    private void OnCollisionEnter(UnityEngine.Collision col)
    {
        var multiTag = col.gameObject.GetComponent<CustomTag>();
        // Need to make prefabs for the gameobjects to put these scripts on them as well
        if (col.gameObject.CompareTag("Cuttable"))
        {
            Vector3 pos = col.gameObject.transform.position;
            ContactPoint contact = col.contacts[0];
            Vector3 contact_pos = contact.point;
            Vector3 scale = col.gameObject.transform.localScale;

            Vector3 leftPoint = pos - Vector3.right * scale.x / 1.5f;
            Vector3 rightPoint = pos + Vector3.right * scale.x / 1.5f;
            Material mat = col.gameObject.GetComponent<MeshRenderer>().material;

            Destroy(col.gameObject);
            /*GameObject rightSideObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightSideObj.transform.position = (rightPoint + pos) / 2;
            float rightWidth = Vector3.Distance(pos, rightPoint);
            rightSideObj.transform.localScale = new Vector3(0.75f*rightWidth, 2f * scale.y, 0.5f*scale.z);
            rightSideObj.AddComponent<Rigidbody>().mass = 100f;
            rightSideObj.GetComponent<MeshRenderer>().material = mat;

            GameObject leftSideObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftSideObj.transform.position = (leftPoint + pos) / 2;
            float leftWidth = Vector3.Distance(pos, leftPoint);
            leftSideObj.transform.localScale = new Vector3(0.75f * leftWidth, 2f * scale.y, 0.5f * scale.z);
            leftSideObj.AddComponent<Rigidbody>().mass = 100f;
            leftSideObj.GetComponent<MeshRenderer>().material = mat;

            Debug.Log(pos);*/

            tempCutLettuce1 = Instantiate(cutLettuce, new Vector3(0.6259f, 2.675f, -2.887f), Quaternion.identity);
            tempCutLettuce2 = Instantiate(cutLettuce, new Vector3(0.6259f, 2.675f, -3.052f), Quaternion.identity);

        }
    }


}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDetection : MonoBehaviour
{
    public GameObject DetectionCube;
    public GameObject myObject;
    public GameObject HoldStillText;
    public GameObject CommandText;
    public GameObject HandDataGatherer;
    public GameObject VectorGenerator;
    public GameObject MovementAnalyser;

    private float timeObjectEntered;
    private bool isObjectInside = false;

    private void Start()
    {
        // Activate the DetectionCube at the start
        DetectionCube.SetActive(true);
    }

    private void Update()
    {
        // Check if myObject is within the DetectionCube's bounds
        if (DetectionCube.activeSelf && DetectionCube.GetComponent<Collider>().bounds.Contains(myObject.transform.position))
        {
            if (!isObjectInside)
            {
                isObjectInside = true;
                timeObjectEntered = Time.time;
                HoldStillText.SetActive(true);
            }

            if (isObjectInside && Time.time - timeObjectEntered >= 1)
            {
                VectorGenerator.SetActive(true);
                MovementAnalyser.SetActive(true);
                HandDataGatherer.SetActive(true);
                StartCoroutine(ActivateAfterDelay());
            }
        }
        else
        {
            isObjectInside = false;
            HoldStillText.SetActive(false);
        }
    }

    IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(3);
        HoldStillText.SetActive(false);
        CommandText.SetActive(true);

        yield return new WaitForSeconds(30);
        HandDataGatherer.SetActive(false);

        yield return new WaitForSeconds(2);
        CommandText.SetActive(false);
        VectorGenerator.SetActive(false);
        MovementAnalyser.SetActive(false);
        DetectionCube.SetActive(false);
    }
}

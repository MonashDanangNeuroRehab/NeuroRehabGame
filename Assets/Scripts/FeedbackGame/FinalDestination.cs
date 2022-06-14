using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDestination: MonoBehaviour
{
    private GameObject _gameManager;
    private GameObject _player;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager");
        _player = GameObject.Find("BottomWall");
    }

    private void OnTriggerEnter(Collider other)
    {
        _gameManager.GetComponent<FeedbackGameManager>().gameFinished = true;
    }
    // Update is called once per frame
    void Update()
    {

    }
}

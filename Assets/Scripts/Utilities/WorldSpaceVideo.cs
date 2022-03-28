using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class WorldSpaceVideo : MonoBehaviour
{
    public VideoClip[] videoClips;
    public Text currentMinutes;
    public Text currentSeconds;
    public Text totalMinutes;
    public Text totalSeconds;

    private VideoPlayer videoPlayer;
    private int videoClipIndex;

    [SerializeField]
    private GameObject display;

    // Moving display
    private Camera mainCamera;
    private Vector3 desiredPosition = new Vector3(0, 0, 0);
    private Quaternion fixedRotation;
    private bool displayMoveFinish = true;
    private bool isDisplayUp = false;
    private Vector3 displayUp;
    private Vector3 displayAway;
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 1f;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    // Use this for initialization
    void Start()
    {
        // videoPlayer.targetTexture.Release();
        // videoPlayer.clip = videoClips[0];
        mainCamera = Camera.main;
        Ray ray = mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                desiredPosition = hit.point;
            }
        }
        display.transform.position = desiredPosition;
        displayUp = desiredPosition;
        display.transform.LookAt(mainCamera.transform, Vector3.up);
        fixedRotation = display.transform.rotation;
        ray = mainCamera.ScreenPointToRay(new Vector2( - Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                desiredPosition = hit.point;
            }
        }
        display.transform.position = desiredPosition;
        displayAway = desiredPosition;

        desiredPosition = displayUp;

        // Test move display 
        ToggleDisplay();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void LateUpdate()
    {
        if (displayMoveFinish is false)
        {
            display.transform.position = Vector3.SmoothDamp(display.transform.position, desiredPosition, ref velocity, smoothTime);
            if (display.transform.position.x <= desiredPosition.x + 0.01f && display.transform.position.x >= desiredPosition.x - 0.01f)
            {
                displayMoveFinish = true;
                isDisplayUp = !isDisplayUp;
                ToggleDisplay(); // Test toggle display, comment out after testing
            }
        }
    }

    public void SetNextClip()
    {
        videoClipIndex++;

        if (videoClipIndex >= videoClips.Length)
        {
            videoClipIndex = videoClipIndex % videoClips.Length;
        }

        videoPlayer.clip = videoClips[videoClipIndex];
        videoPlayer.Play();
    }

    public void PlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }
    }

    public void ToggleDisplay()
    {
        if (displayMoveFinish is true)
        {
            if (isDisplayUp is false)
            {
                desiredPosition = displayUp;
                displayMoveFinish = false;
            }
            else
            {
                desiredPosition = displayAway;
                displayMoveFinish = false;
            }
        }
    }
}

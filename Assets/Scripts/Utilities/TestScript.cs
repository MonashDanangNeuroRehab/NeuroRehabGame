using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Query;
public class TestScript : PostProcessProvider
{
    [Header("Projection")]
    public Transform headTransform;

    [SerializeField]
    private GameObject _testObjectRight;
    [SerializeField]
    private GameObject _testObjectLeft;
    [SerializeField]
    private GameObject _gameBall;

    public override void ProcessFrame(ref Frame inputFrame)
    {
        // Calculate the position of the head and the basis to calculate shoulder position.
        if (headTransform == null) { headTransform = MainCameraProvider.mainCamera.transform; }
        Vector3 headPos = headTransform.position;
        var shoulderBasis = Quaternion.LookRotation(
          Vector3.ProjectOnPlane(headTransform.forward, Vector3.up),
          Vector3.up);

        foreach (var hand in inputFrame.Hands)
        {
            // Approximate shoulder position with magic values.
            Vector3 shoulderPos = headPos
                                  + (shoulderBasis * (new Vector3(0f, -0.13f, -0.1f)
                                  + Vector3.left * 0.15f * (hand.IsLeft ? 1f : -1f)));

            // Calculate the projection of the hand if it extends beyond the
            // handMergeDistance.
            if (!hand.IsPinching())
            {
                _gameBall.transform.position = new Vector3(_gameBall.transform.position.x, 0.782f, _gameBall.transform.position.z);

                if (hand.IsRight)
                {
                    _testObjectRight.transform.localPosition = hand.GetIndex().TipPosition.ToVector3();
                    _testObjectRight.transform.localRotation = hand.Rotation.ToQuaternion();
                }
                else if (hand.IsLeft)
                {
                    _testObjectLeft.transform.localPosition = hand.GetIndex().TipPosition.ToVector3();
                    _testObjectLeft.transform.localRotation = hand.Rotation.ToQuaternion();
                }
            }
            else 
            {
                if (hand.IsRight)
                {
                    _testObjectLeft.transform.position = new Vector3(_testObjectLeft.transform.position.x, 0.782f, _testObjectLeft.transform.position.z);
                    _testObjectRight.transform.position = new Vector3(_testObjectRight.transform.position.x, 0.782f, _testObjectRight.transform.position.z);
                    _gameBall.transform.localPosition = hand.GetIndex().TipPosition.ToVector3();
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;

public class Player : MonoBehaviour
{
    private Hand _leftHand;
    private Hand _rightHand;
    private Vector3 _thumbTipPosition;
    private void Awake()
    {
        
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    public void SetLeftHand()
    {

    }
}
public class Test : HandModelBase
{
    public override Chirality Handedness { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public override ModelType HandModelType => throw new System.NotImplementedException();

    public override Hand GetLeapHand()
    {
        throw new System.NotImplementedException();
    }

    public override void SetLeapHand(Hand hand)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateHand()
    {
        throw new System.NotImplementedException();
    }
}

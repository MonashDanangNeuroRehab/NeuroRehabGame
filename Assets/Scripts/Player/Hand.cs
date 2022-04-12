namespace NeuroRehab
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Hand
    {
        public Arm Arm;
        public List<Finger> Fingers;
        public Hand()
        {
            Arm = new Arm();
            Fingers = new List<Finger>(5);
            Fingers.Add(new Finger());
            Fingers.Add(new Finger());
            Fingers.Add(new Finger());
            Fingers.Add(new Finger());
            Fingers.Add(new Finger());
        }
    }
}

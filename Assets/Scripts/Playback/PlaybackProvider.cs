/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEditor;
using System.Collections.Generic;

namespace Leap.Unity.Playback
{
    public class PlaybackProvider : LeapProvider
    {
        public override Frame CurrentFrame
        {
            get
            {
                return _transformedFrame;
            }
        }

        public override Frame CurrentFixedFrame
        {
            get
            {
                return CurrentFrame;
            }
        }
        [SerializeField]
        private InputAction buttonPressed;
        private Vector3 _offset = Vector3.zero;
        private Pose _pose;
        public LeapServiceProvider mainProvider;
        private bool isPlayback = false;
        private bool _isFirstHandDetected = false;
        private Vector3 _initialPalmarAxisLeftHand;
        private Vector3 _initialDistalAxisLeftHand;
        public List<float> palmarAxisAnglesLeftHand = new List<float>();
        public List<float> distalAxisAnglesLeftHand = new List<float>();
        public List<float> fistClenchLeftHand = new List<float>();
        private Vector3 _initialPalmarAxisRightHand;
        private Vector3 _initialDistalAxisRightHand;
        public List<float> palmarAxisAnglesRightHand = new List<float>();
        public List<float> distalAxisAnglesRightHand = new List<float>();
        public List<float> fistClenchRightHand = new List<float>();

        public bool isCalibrated = true;
        public int HandID = 0;
        public static int LEFT_HAND = 0;
        public static int RIGHT_HAND = 1;

        private void OnEnable()
        {
            buttonPressed.Enable();
            buttonPressed.performed += ButtonPressed;
        }
        private void OnDisable()
        {
            buttonPressed.performed -= ButtonPressed;
            buttonPressed.Disable();
        }
        private void ButtonPressed(InputAction.CallbackContext context)
        {
            if (isPlayback)
            {
                isPlayback = false;
                Stop();
                Debug.Log("Playback Off");
            }
            else
            {
                isPlayback = true;
                Play();
                Debug.Log("Playback On");
            }

        }
        [SerializeField]
        protected Recording _recording;

        [SerializeField]
        protected PlaybackTimeline _playbackTimeline = PlaybackTimeline.Graphics;

        [SerializeField]
        protected bool _autoPlay = false;

        protected bool _isPlaying = false;
        protected int _currentFrameIndex = 0;
        protected float _startTime = 0;

        protected Frame _transformedFrame = new Frame();

        public virtual bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
        }

        public virtual Recording recording
        {
            get
            {
                return _recording;
            }
            set
            {
                Stop();
                _recording = value;
            }
        }

        public void ChooseRecording(string title)
        {
            Stop();
            _recording = AssetDatabase.LoadAssetAtPath<Recording>("Assets/Resources/Recording/" + title + ".asset");
            palmarAxisAnglesLeftHand.Clear();
            palmarAxisAnglesRightHand.Clear();
            distalAxisAnglesLeftHand.Clear();
            distalAxisAnglesRightHand.Clear();
            fistClenchLeftHand.Clear();
            fistClenchRightHand.Clear();
        }
        public void SetOffset(Vector3 offset)
        {
            _offset = offset;
        }
        public virtual void Play()
        {
            if (_recording is null)
            {
                Debug.Log("No Recording chosen");
                return;
            }
            float delta = _recording.frameTimes[_currentFrameIndex] - _recording.frameTimes[0];

            switch (_playbackTimeline)
            {
                case PlaybackTimeline.Graphics:
                    _startTime = Time.time - delta;
                    break;
                case PlaybackTimeline.Physics:
                    _startTime = Time.fixedTime - delta;
                    break;
            }

            _isPlaying = true;
        }

        public virtual void Pause()
        {
            _isPlaying = false;
        }

        public virtual void Stop()
        {
            Pause();
            if (_recording != null)
            {
                Seek(0);
                _isFirstHandDetected = false;
            }
        }

        public virtual void Seek(int newFrameIndex)
        {
            newFrameIndex = Mathf.Clamp(newFrameIndex, 0, _recording.frames.Count - 1);
            if (newFrameIndex == _currentFrameIndex)
            {
                return;
            }
            
            _currentFrameIndex = newFrameIndex;
            _transformedFrame.CopyFrom(_recording.frames[_currentFrameIndex]);
            /*
                .Transform(new LeapTransform(mainProvider.transform.position.ToVector()
                                            , mainProvider.transform.rotation.ToLeapQuaternion()
                                            , mainProvider.transform.lossyScale.ToVector())));
            */
            // Remove the unused hand
            if (HandID == LEFT_HAND)
            {
                _transformedFrame.Hands.RemoveAll(hand => hand.IsRight == true);
            }
            else if (HandID == RIGHT_HAND)
            {
                _transformedFrame.Hands.RemoveAll(hand => hand.IsLeft == true);
            }
            foreach(Hand hand in _transformedFrame.Hands)
            {
                if (!_isFirstHandDetected)
                {
                    if (hand.IsLeft)
                    {
                        _initialPalmarAxisLeftHand = hand.PalmarAxis();
                        _initialDistalAxisLeftHand = hand.DistalAxis();
                    }
                    else if (hand.IsRight)
                    {
                        _initialPalmarAxisRightHand = hand.PalmarAxis();
                        _initialDistalAxisRightHand = hand.DistalAxis();
                    }
                    _isFirstHandDetected = true;
                }
                else
                {
                    if (hand.IsLeft)
                    {
                        palmarAxisAnglesLeftHand.Add(Vector3.Angle(_initialPalmarAxisLeftHand, hand.PalmarAxis()));
                        distalAxisAnglesLeftHand.Add(Vector3.Angle(_initialDistalAxisLeftHand, hand.DistalAxis()));
                        fistClenchLeftHand.Add(hand.GetFistStrength());

                    }
                    else if (hand.IsRight)
                    {
                        palmarAxisAnglesRightHand.Add(Vector3.Angle(_initialPalmarAxisRightHand, hand.PalmarAxis()));
                        distalAxisAnglesRightHand.Add(Vector3.Angle(_initialDistalAxisRightHand, hand.DistalAxis()));
                        fistClenchRightHand.Add(hand.GetFistStrength());
                    }
                }
            }

        }

        protected virtual void Start()
        {
            if (_autoPlay)
            {
                Play();
            }
        }

        protected virtual void Update()
        {
            if (_isPlaying)
            {
                if (_playbackTimeline == PlaybackTimeline.Graphics)
                {
                    stepRecording(Time.time - _startTime);
                }
                DispatchUpdateFrameEvent(_transformedFrame);
            }
        }

        protected virtual void FixedUpdate()
        {
            if (_isPlaying)
            {
                if (_playbackTimeline == PlaybackTimeline.Physics)
                {
                    stepRecording(Time.fixedTime - _startTime);
                }
                DispatchFixedFrameEvent(_transformedFrame);
            }
        }

        private void stepRecording(float time)
        {
            while (true)
            {
                if (_currentFrameIndex >= _recording.frames.Count - 1)
                {
                    Pause();
                    break;
                }

                float crossover = (_recording.frameTimes[_currentFrameIndex + 1] + _recording.frameTimes[_currentFrameIndex]) / 2.0f;
                if (time > crossover)
                {
                    Seek(_currentFrameIndex + 1);
                }
                else
                {
                    break;
                }
            }
        }

        public enum PlaybackTimeline
        {
            Graphics,
            Physics
        }
        
    }
}
/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using UnityEngine.InputSystem;

namespace Leap.Unity.Playback
{

    public class PlaybackRecorder : MonoBehaviour
    {
        public enum RecordTime
        {
            Update,
            FixedUpdate
        }

        public enum SaveType
        {
            None,
            UnityAsset
        }
        [SerializeField]
        private InputAction recordingButton;
        private bool isRecordingOn = false;

        [SerializeField]
        protected LeapProvider _provider;

        [SerializeField]
        protected RecordTime _recordTime = RecordTime.Update;

        [Header("Editor Settings")]

        [SerializeField]
        protected SaveType _saveType = SaveType.None;

        protected string _unityAssetSavePath = "Assets/Resources/Recording/Replacement";

        protected float _beginTime;
        protected Recording _currentRecording;

      

        private void OnEnable()
        {
            recordingButton.Enable();
            recordingButton.performed += ButtonPressed;
        }
        private void OnDisable()
        {
            recordingButton.performed -= ButtonPressed;
            recordingButton.Disable();
        }
        private void ButtonPressed(InputAction.CallbackContext context)
        {
            if (isRecordingOn)
            {
                EndRecording();
                isRecordingOn = false;
                Debug.Log("Recording Off");
            }
            else
            {
                isRecordingOn = true;
                StartRecording();
                Debug.Log("Recording On");
            }
        }

        public virtual void StartRecording()
        {
            switch (_recordTime)
            {
                case RecordTime.FixedUpdate:
                    _beginTime = Time.fixedTime;
                    break;
                case RecordTime.Update:
                    _beginTime = Time.time;
                    break;
            }
            _currentRecording = ScriptableObject.CreateInstance<Recording>();
        }

        public virtual Recording EndRecording()
        {
            Recording finishedRecording = _currentRecording;
            _currentRecording = null;

            switch (_saveType)
            {
                case SaveType.None:
                    break;
                case SaveType.UnityAsset:
#if UNITY_EDITOR
                    Directory.CreateDirectory(_unityAssetSavePath);
                    string path = AssetDatabase.GenerateUniqueAssetPath(_unityAssetSavePath + "/"+ "TestFileName" + ".asset");
                    AssetDatabase.CreateAsset(finishedRecording, path);
                    AssetDatabase.SaveAssets();
                    break;
#else
              throw new Exception("Cannot save unity assets outside of Unity Editor");
#endif
                default:
                    break;
            }

            return finishedRecording;
        }

        protected virtual void Update()
        {
            if (_currentRecording != null)
            {
                if (_recordTime == RecordTime.Update)
                {
                    Frame frame = _provider.CurrentFrame;
                    if (frame != null)
                    {
                        _currentRecording.frames.Add(new Frame().CopyFrom(frame));
                            /*
                            .Transform(new LeapTransform(transform.position.ToVector()
                            , transform.rotation.ToLeapQuaternion()
                            , transform.lossyScale.ToVector())));
                            */
                        _currentRecording.frameTimes.Add(Time.time - _beginTime);
                    }
                }

            }
        }

        protected virtual void FixedUpdate()
        {
            if (_currentRecording != null && _recordTime == RecordTime.FixedUpdate)
            {
                Frame frame = _provider.CurrentFixedFrame;
                if (frame != null)
                {
                    _currentRecording.frames.Add(new Frame().CopyFrom(frame));
                        /*
                        .Transform(new LeapTransform(transform.position.ToVector()
                        , transform.rotation.ToLeapQuaternion()
                        , transform.lossyScale.ToVector())));
                        */
                    _currentRecording.frameTimes.Add(Time.fixedTime - _beginTime);
                }
            }
        }
    }

}
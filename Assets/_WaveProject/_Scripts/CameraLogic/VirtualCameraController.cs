﻿using System;
using Cinemachine;
using UnityEngine;
using WaveProject.UserInput;
using WaveProject.UserInput.Legacy;

namespace WaveProject.CameraLogic
{
    public class VirtualCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _centerVc;
        [SerializeField] private CinemachineVirtualCamera _leftVc;
        [SerializeField] private CinemachineVirtualCamera _rightVc;

        [SerializeField] private CameraController _cameraController;

        private void Start()
        {
            _cameraController.OnSideChanged += OnSideChanged;
        }

        private void OnSideChanged(ScreenSide newSide)
        {
            switch (newSide)
            {
                case ScreenSide.Center:
                    _leftVc.gameObject.SetActive(false);
                    _centerVc.gameObject.SetActive(true);
                    _rightVc.gameObject.SetActive(false);
                    break;

                case ScreenSide.Left:
                    _leftVc.gameObject.SetActive(true);
                    _centerVc.gameObject.SetActive(false);
                    _rightVc.gameObject.SetActive(false);
                    break;

                case ScreenSide.Right:
                    _leftVc.gameObject.SetActive(false);
                    _centerVc.gameObject.SetActive(false);
                    _rightVc.gameObject.SetActive(true);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(newSide), newSide, null);
            }
        }
    }
}
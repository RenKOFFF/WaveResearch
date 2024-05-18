﻿using System;
using DG.Tweening;
using UnityEngine;
using WaveProject.Interaction;
using WaveProject.Utility;

namespace WaveProject.Station.Plates
{
    public class Plate : MonoBehaviour
    {
        [field: SerializeField] public PlateMovementInteractable MovementInteractable { get; private set; }
        private Vector3 _defaultPosition;

        private void Awake()
        {
            _defaultPosition = transform.position;
        }

        public void SetSize(float length, float thickness)
        {
            var lengthInMeters = Utils.MillimetersToMeters(length);
            var thicknessInMeters = Utils.MillimetersToMeters(thickness);
            
            transform.localScale = new Vector3(1, thicknessInMeters, lengthInMeters);
        }

        public void Init()
        {
            MovementInteractable.Init();
            MovementInteractable.SetDefaultValue();
        }

        public void SetStart()
        {
            transform.position = _defaultPosition;
        }

        public void MoveToAntenna(float moveDuration, TweenCallback callback)
        {
            transform
                .DOMoveZ(MovementInteractable.StartPoint.position.z, moveDuration)
                .SetEase(Ease.InOutExpo)
                .OnComplete(callback);
        }
    }
}
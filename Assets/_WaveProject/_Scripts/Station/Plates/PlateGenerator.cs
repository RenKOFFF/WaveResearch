﻿using Cinemachine;
using UnityEngine;
using WaveProject.Interaction;
using WaveProject.Services;
using WaveProject.UI;
using WaveProject.UserInput;

namespace WaveProject.Station.Plates
{
    public class PlateGenerator : MonoBehaviour
    {
        [SerializeField] private Receiver _receiver;
        [SerializeField] private PlateUiView _plateUiView;

        [Space] 
        [SerializeField] private InteractableButton _selectMetalButton;
        [SerializeField] private InteractableButton _selectDielectricButton;

        [Space] 
        [SerializeField] private int _lengthMinValue;
        [SerializeField] private int _lengthMaxValue;

        [SerializeField] private int _thicknessMinValue;
        [SerializeField] private int _thicknessMaxValue;

        [SerializeField] private float _resistanceMinValue;
        [SerializeField] private float _resistanceMaxValue;

        [Space] 
        [SerializeField] private Plate _metalPlate;
        [SerializeField] private Plate _dielectricPlate;

        [SerializeField] private float _moveDuration = 1;

        [SerializeField] private CinemachineVirtualCamera _virtualCamera;

        private float _length;
        private float _thickness;
        private float _resistance;

        private PlateType _plateType;
        private Plate _currentPlate;
        
        private RoutineService _routines;
        private float _uiShowWaitTime = 1f;
        
        private InputController _inputController;

        private void Start()
        {
            _plateUiView.Init(Show, Back,
                _lengthMinValue,
                _lengthMaxValue,
                _thicknessMinValue,
                _thicknessMaxValue,
                _resistanceMinValue,
                _resistanceMaxValue);

            _plateUiView.LengthChanged += OnLengthChanged;
            _plateUiView.ThicknessChanged += OnThicknessChanged;
            _plateUiView.ResistanceChanged += OnResistanceChanged;
            
            _selectMetalButton.Init();
            _selectDielectricButton.Init();

            _selectMetalButton.Clicked.AddListener(SelectMetalPlate);
            _selectDielectricButton.Clicked.AddListener(SelectDielectricPlate);

            if (ServiceManager.TryGetService(out RoutineService routines)) _routines = routines;
            if (ServiceManager.TryGetService(out InputController inputController)) _inputController = inputController;
        }

        private void OnDestroy()
        {
            _plateUiView.LengthChanged -= OnLengthChanged;
            _plateUiView.ThicknessChanged -= OnThicknessChanged;
            _plateUiView.ResistanceChanged -= OnResistanceChanged;
        }

        private void OnLengthChanged(float value) => _length = value;
        private void OnThicknessChanged(float value) => _thickness = value;
        private void OnResistanceChanged(float value) => _resistance = value;

        private void SelectMetalPlate()
        {
            _inputController.BlockUserInput(true);
            
            _virtualCamera.gameObject.SetActive(true);
            
            _plateType = PlateType.Metal;
            
            _routines.WaitTime(_uiShowWaitTime, this, () =>
            {
                _plateUiView.gameObject.SetActive(true);
                _plateUiView.SelectMetalPlate();
            });
        }

        private void SelectDielectricPlate()
        {
            _inputController.BlockUserInput(true);
            
            _virtualCamera.gameObject.SetActive(true);
            
            _plateType = PlateType.Dielectric;
            
            _routines.WaitTime(_uiShowWaitTime, this, () =>
            {
                _plateUiView.gameObject.SetActive(true);
                _plateUiView.SelectDielectricPlate();
            });
        }

        private void Show()
        {
            _receiver.SetPhaseShiftPlate(_plateType, _length, _thickness, _resistance);

            if (_currentPlate != null) 
                _currentPlate.Hide();
            
            _currentPlate = _plateType == PlateType.Metal ? _metalPlate : _dielectricPlate;
            _currentPlate.gameObject.SetActive(true);

            _currentPlate.SetStart();
            _currentPlate.MoveToAntenna(_moveDuration, OnPlateMoved);
            
            _plateUiView.gameObject.SetActive(false);
        }

        private void OnPlateMoved()
        {
            _currentPlate.Init();
            _inputController.ExternSubscribe(_currentPlate.MovementInteractable);

            _currentPlate.MovementInteractable.ChangingFinished += FinishPlateInserting;
        }

        private void Back()
        {
            _plateUiView.gameObject.SetActive(false);
            FinishPlateInserting();
        }

        private void FinishPlateInserting()
        {
            if (_currentPlate != null) 
                _currentPlate.MovementInteractable.ChangingFinished -= FinishPlateInserting;

            _virtualCamera.gameObject.SetActive(false);
            _inputController.BlockUserInput(false);
        }
    }
}
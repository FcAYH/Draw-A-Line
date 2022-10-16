using DrawALine.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DrawALine.Logic
{
    public class ToolBoxManager : MonoBehaviour
    {
        private TMP_Dropdown _currentAlgorithm;
        private Slider _samplingPointCount;
        private TextMeshProUGUI _countText;
        private Toggle _autoSampling;

        private void Awake()
        {
            _currentAlgorithm = transform
                                    .Find("Algorithm Panel/Algorithm Selector")
                                    .gameObject
                                    .GetComponent<TMP_Dropdown>();

            _samplingPointCount = transform
                                    .Find("Algorithm Panel/Sampling Point Count")
                                    .gameObject
                                    .GetComponent<Slider>();

            _countText = _samplingPointCount.transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>();

            _autoSampling = transform.Find("Algorithm Panel/Auto Sampling")
                                .gameObject.GetComponent<Toggle>();
        }

        // Start is called before the first frame update
        void Start()
        {
            InInitializeAlgorithmSelectorOptions();
            InitializeSamplingPointCountSlider();
            InitializeAutoSamplingToggle();
        }

        # region Initialize Controls
        private void InInitializeAlgorithmSelectorOptions()
        {
            TMP_Dropdown.DropdownEvent valueChanged = new TMP_Dropdown.DropdownEvent();
            valueChanged.AddListener(OnAlgorithmSelectorValueChanged);
            _currentAlgorithm.onValueChanged = valueChanged;
            _currentAlgorithm.ClearOptions();

            var algs = Enum.GetValues(typeof(DrawAlgorithm));
            List<TMP_Dropdown.OptionData> optionList = new List<TMP_Dropdown.OptionData>(algs.Length);
            foreach (var alg in algs)
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
                data.text = alg.ToString();

                optionList.Add(data);
            }

            _currentAlgorithm.options = optionList;
        }

        private void InitializeSamplingPointCountSlider()
        {
            Slider.SliderEvent valueChanged = new Slider.SliderEvent();
            valueChanged.AddListener(OnSamplingPointCountSliderValueChanged);
            _samplingPointCount.onValueChanged = valueChanged;
        }

        private void InitializeAutoSamplingToggle()
        {
            Toggle.ToggleEvent valueChanged = new Toggle.ToggleEvent();
            valueChanged.AddListener(OnAutoSamplingToggleValueChanged);
            _autoSampling.onValueChanged = valueChanged;
        }
        #endregion

        # region On Value Changed Events
        private void OnAlgorithmSelectorValueChanged(int index)
        {
            var option = _currentAlgorithm.options[index];

            GameSettings.CurrentAlgorithm = Enum.Parse<DrawAlgorithm>(option.text);

            if (GameSettings.CurrentAlgorithm == DrawAlgorithm.Interpolation)
            {
                _samplingPointCount.gameObject.SetActive(true);
                _autoSampling.gameObject.SetActive(true);
            }
            else
            {
                _samplingPointCount.gameObject.SetActive(false);
                _autoSampling.gameObject.SetActive(false);
            }
        }

        private void OnSamplingPointCountSliderValueChanged(Single value)
        {
            _countText.text = value.ToString();
            LineGenerator.Instance.SamplingPointCount = (int)value;
        }

        private void OnAutoSamplingToggleValueChanged(bool value)
        {
            LineGenerator.Instance.AutoSampling = value;
        }
        #endregion
    }
}
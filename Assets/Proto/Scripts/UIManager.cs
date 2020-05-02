using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField] private Slider powerbarSlide;

    private void Start() {
        PlayerController.onJumpbarChangeEvent.AddListener(OnPowerbarChange);
    }

    private void OnPowerbarChange(float newValue, Vector2 powerbarRange) {
        powerbarSlide.minValue = powerbarRange.x;
        powerbarSlide.maxValue = powerbarRange.y;
        powerbarSlide.value = newValue;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototipo {

    public class InputController : MonoBehaviour {

        public static bool DeviceBasedInput(PlayerInputType input) {
            switch (input) {
                case PlayerInputType.JumpDown:
                    if (SystemInfo.deviceType == DeviceType.Desktop) return Input.GetKeyDown(KeyCode.Space);
                    if (SystemInfo.deviceType == DeviceType.Handheld) return Input.GetMouseButtonDown(0);
                    break;
                case PlayerInputType.JumpUp:
                    if (SystemInfo.deviceType == DeviceType.Desktop) return Input.GetKeyUp(KeyCode.Space);
                    if (SystemInfo.deviceType == DeviceType.Handheld) return Input.GetMouseButtonUp(0);
                    break;
                case PlayerInputType.Pause:
                    if (SystemInfo.deviceType == DeviceType.Desktop) return Input.GetKeyDown(KeyCode.Escape);
                    break;
                default:
                    return false;
            }
            return false;
        }
    }

    public enum PlayerInputType { JumpDown, JumpUp, Pause }
}
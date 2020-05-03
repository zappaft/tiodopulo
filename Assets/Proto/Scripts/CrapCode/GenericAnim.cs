using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototipo {

    public class GenericAnim : MonoBehaviour {

        private Action callback;

        public void EndOfAnimation() {
            callback?.Invoke();
        }

        public void SetListener(Action callback) {
            this.callback = callback;
        }
    }
}
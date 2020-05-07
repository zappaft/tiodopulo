using Prototipo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Prototipo {

    public class UIManager : MonoBehaviour, IStatedBehaviour {

        [SerializeField] private Slider powerbarSlide;

        #region Animations
        [SerializeField] private GameObject introObj;
        private Animation introAnimation;

        [SerializeField] private GameObject endObj;
        #endregion

        private void Start() {
            PlayerController.onJumpbarChangeEvent.AddListener(OnPowerbarChange);
            introObj.GetComponent<GenericAnim>().SetListener(IntroOut);
            introAnimation = introObj.GetComponent<Animation>();

            GameManager.Instance.stateChangeEvent.AddListener(OnStateChange);
        }

        private void Update() {
            if (GameManager.Instance.InMenu) {
                IntroInput();
            }
            if (GameManager.Instance.InEndGame) {
                EndGameInput();
            }
        }

        /// <summary>
        /// Callback do evento da barra de força do pulo.
        /// </summary>
        /// <param name="newValue">Valor atual da barra de força.</param>
        /// <param name="powerbarRange">Vetor com valor inicial e final da barra.</param>
        private void OnPowerbarChange(float newValue, Vector2 powerbarRange) {
            powerbarSlide.minValue = powerbarRange.x;
            powerbarSlide.maxValue = powerbarRange.y;
            powerbarSlide.value = newValue;
        }

        private void IntroInput() {
            if (Input.GetKeyUp(KeyCode.Space)) {
                introAnimation.Play();
            }
        }

        private void EndGameInput() {
            if (Input.GetKeyUp(KeyCode.Space)) {
                GameSceneManager.Instance.ReloadScene();
            }
        }

        public void IntroOut() {
            introObj.SetActive(false);
            GameManager.Instance.StartGame();
        }

        public void EndScreen() {
            endObj.SetActive(true);
        }

        public void OnStateChange(GameManager.GameState oldState, GameManager.GameState newState) {
            if(newState == GameManager.GameState.EndGame) {
                EndScreen();
            }
        }
    }
}
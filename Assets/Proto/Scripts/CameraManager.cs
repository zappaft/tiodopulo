using Prototipo;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Prototipo {

    public class CameraManager : MonoBehaviour, IStatedBehaviour {

        [Header("Background")]
        [SerializeField] private GameObject background1;
        [SerializeField] private GameObject background2;
        [SerializeField] private int distanceBetweenEach;

        [Header("Camera")]
        [SerializeField] private GameObject cam;

        private void Start() {
            GameManager.Instance.stateChangeEvent?.AddListener(OnStateChange);
            cam.GetComponent<CameraController>().CamCollisionEvent?.AddListener(OnCamCollision);
        }

        private void Update() {
            if (!GameManager.Instance.InGame) return;
            MoveCamera();
        }

        public void OnStateChange(GameManager.GameState oldState, GameManager.GameState newState) {
            Debug.Log($"cameramanager state changed: {oldState} => {newState}");
        }

        /// <summary>
        /// Basicamente, move a câmera. Processa qualquer cálculo para movimentação da mesma.
        /// </summary>
        private void MoveCamera() {
            cam.transform.position += Vector3.right * GameManager.Instance.CamSpeed * Time.deltaTime;
        }

        /// <summary>
        /// Callback do evento de colisão da câmera com o colisor do background.
        /// Executa a movimentação de backgrounds, passando como parâmetro tanto o background
        /// que vai mover, quanto o que foi encostado.
        /// </summary>
        /// <param name="collision"></param>
        private void OnCamCollision(GameObject collision) {
            MoveBackground(collision == background1 ? background2 : background1, collision == background1 ? background1 : background2);
        }

        /// <summary>
        /// Recebe dois backgrounds, o primeiro, "bgToMove" será o afetado e o segundo,
        /// "collided", foi o que encostou. O "collided" é referência para reposicionar o "bgToMove".
        /// </summary>
        /// <param name="bgToMove">Background que deverá ser movido, normalmente o que ficou para trás.</param>
        /// <param name="collided">Background que houve colisão, serve como referencia para reajustar o primeiro background, "bgToMove".</param>
        private void MoveBackground(GameObject bgToMove, GameObject collided) {
            bgToMove.transform.position = collided.transform.position + (Vector3.right * distanceBetweenEach);
        }
    }

}
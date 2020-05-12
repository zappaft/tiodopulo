using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Prototipo {

    public class CamCollisionEvent : UnityEvent<GameObject> { }

    public class CameraController : MonoBehaviour {

        public CamCollisionEvent CamCollisionEvent { get; private set; }

        private void Awake() {
            if (CamCollisionEvent == null) CamCollisionEvent = new CamCollisionEvent();
        }

        private void Start() {
            PlayerController.OnPlayerJumpEvent.AddListener(OnPlayerJump);
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.CompareTag("BGCollider")) CamCollisionEvent?.Invoke(collision.transform.parent.gameObject);
        }

        private void OnPlayerJump(PlayerController.PlayerState oldState, PlayerController.PlayerState newState, bool repeatedCollision) {
            if(oldState == PlayerController.PlayerState.Jumping && newState == PlayerController.PlayerState.Grounded && !repeatedCollision) {
                StartCoroutine(CameraShake());
            }
        }

        private IEnumerator CameraShake() {
            Vector3 originalPos = transform.localPosition;
            float time = 0;
            float duration = .2f;
            float strength = .02f;
            while (time < duration) {
                float x = Random.Range(-1, 1) * strength;
                float y = Random.Range(-1, 1) * strength;
                transform.localPosition = new Vector3(x, y, originalPos.z);
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}

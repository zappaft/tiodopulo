using UnityEngine;
using UnityEngine.Events;

public class CamCollisionEvent : UnityEvent<GameObject> { }

public class CameraController : MonoBehaviour {

    public CamCollisionEvent CamCollisionEvent { get; private set; }

    private void Awake() {
        if (CamCollisionEvent == null) CamCollisionEvent = new CamCollisionEvent();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("BGCollider")) CamCollisionEvent?.Invoke(collision.transform.parent.gameObject);
    }
}

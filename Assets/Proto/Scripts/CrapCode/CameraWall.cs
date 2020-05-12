using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototipo {

    public class CameraWall : MonoBehaviour {

        private Collider2D ownCollider;

        private void Start() {
            ownCollider = GetComponent<Collider2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (!collision.collider.CompareTag("Player")) Physics2D.IgnoreCollision(collision.collider, ownCollider);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototipo {

    public class PlatformDespawner : MonoBehaviour {

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.CompareTag("Platform")) Destroy(collision.gameObject);
        }
    }
}
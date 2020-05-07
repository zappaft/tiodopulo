using System.Collections.Generic;
using UnityEngine;

namespace Prototipo {

    public class PlatformManager : MonoBehaviour, IStatedBehaviour {

        [SerializeField] private List<GameObject> platforms;

        [SerializeField] private Transform platformsParent;
        [SerializeField] private Transform spawnPoint; 

        private float spawnTime;
        private float counter;

        private void Start() {
            GameManager.Instance.StateChangeEvent?.AddListener(OnStateChange);
            SetNextSpawnTimer();
        }

        private void Update() {
            if (!GameManager.Instance.InGame) return;
            CountdownToSpawn();
        }

        public void OnStateChange(GameManager.GameState oldState, GameManager.GameState newState) {
            Debug.Log($"platformanager state changed: {oldState} => {newState}");
        }

        /// <summary>
        /// Contador do spawner de plataformas. Chama o criador de plataformas.
        /// </summary>
        private void CountdownToSpawn() {
            counter += Time.deltaTime;
            if (counter >= spawnTime) CreateNewPlatform();
        }

        /// <summary>
        /// Sorteia uma plataforma da lista "platforms", instancia a plataforma e chama o reset do timer de spawner.
        /// </summary>
        private void CreateNewPlatform() {
            int platformIndex = Random.Range(0, platforms.Count);
            GameObject spawnedPlatform = Instantiate(platforms[platformIndex], spawnPoint.position, Quaternion.identity, platformsParent);
            SetNextSpawnTimer();
        }

        /// <summary>
        /// Reseta o spawner e sorteia um tempo variável baseado entre o vetor SpawnTimeRange do GameManager.
        /// </summary>
        private void SetNextSpawnTimer() {
            counter = 0;
            spawnTime = Random.Range(GameManager.Instance.SpawnTimeRange.x, GameManager.Instance.SpawnTimeRange.y);
        }
    }
}

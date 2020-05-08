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
        }

        private void Update() {
            if (!GameManager.Instance.InGame) return;
            CountdownToSpawn();
        }

        public void OnStateChange(GameManager.GameState oldState, GameManager.GameState newState) {
            if (oldState == GameManager.GameState.Menu && newState == GameManager.GameState.Playing) SetNextSpawnTimer();
        }

        /// <summary>
        /// Contador do spawner de plataformas. Chama o criador de plataformas.
        /// </summary>
        private void CountdownToSpawn() {
            counter += Time.deltaTime;
            if (counter >= spawnTime) CreateNewPlatform();
        }

        [SerializeField] private GameObject platformPrefab;
        /// <summary>
        /// Sorteia uma plataforma da lista "platforms", instancia a plataforma e chama o reset do timer de spawner.
        /// </summary>
        private void CreateNewPlatform() {
            Instantiate(platformPrefab, spawnPoint.position, Quaternion.identity, platformsParent).transform.localScale = new Vector3(
                Random.Range(1, 2.2f), Random.Range(5, 15), 1);
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

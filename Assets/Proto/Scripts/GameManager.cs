using UnityEngine;
using UnityEngine.Events;

namespace Prototipo {

    public class StateChangeEvent : UnityEvent<GameManager.GameState, GameManager.GameState> { }

    public class GameManager : MonoBehaviour, IStatedBehaviour {

        #region GameState
        [System.Serializable]
        public enum GameState {
            Menu,
            Playing,
            Paused
        }

        private GameState lastState;
        [SerializeField] private GameState _state;
        private GameState State {
            get => _state;
            set {
                stateChangeEvent?.Invoke(_state, value);
                lastState = _state;
                _state = value;
            }
        }

        public bool InGame { get => State == GameState.Playing; }
        public bool InMenu { get => State == GameState.Menu; }
        public bool InPause { get => State == GameState.Paused; }

        public StateChangeEvent stateChangeEvent;
        #endregion

        [SerializeField] private float _camSpeed;
        [SerializeField] private Vector2 _spawnTimeRange;

        public float CamSpeed { get => _camSpeed; }
        public Vector2 SpawnTimeRange { get => _spawnTimeRange / (_camSpeed * 0.75f); }

        public static GameManager Instance { get; private set; }

        private void Awake() {
            if (!Instance) Instance = this;
            else Destroy(this);
            if (stateChangeEvent == null) stateChangeEvent = new StateChangeEvent();
            State = GameState.Playing;
        }

        private void Start() {
            stateChangeEvent.AddListener(OnStateChange);
        }

        private void Update() {
            switch (State) {
                case GameState.Menu:
                    break;
                case GameState.Playing:
                    TogglePause();
                    break;
                case GameState.Paused:
                    TogglePause();
                    break;
            }
        }

        /// <summary>
        /// Troca o estado do jogo entre pausado e jogando. Executando o evento stateChangeEvent.
        /// </summary>
        private void TogglePause() { if (Input.GetKeyDown(KeyCode.Escape)) State = InGame ? GameState.Paused : GameState.Playing; }
        
        public void OnStateChange(GameState oldState, GameState newState) {
            if(newState == GameState.Paused) {
                Time.timeScale = 0;
            }

            if (newState == GameState.Playing) {
                Time.timeScale = 1;
            }
        }

        public void GameOver() {
            State = GameState.Paused;
        }
    }

}

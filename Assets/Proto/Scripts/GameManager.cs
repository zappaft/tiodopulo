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
            Paused,
            EndGame
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
        public bool InEndGame { get => State == GameState.EndGame; }

        public StateChangeEvent stateChangeEvent;
        #endregion

        [SerializeField] private bool _onlyOneJump;
        [SerializeField] private float _camSpeed;
        [SerializeField] private Vector2 _spawnTimeRange;

        public float CamSpeed { get => _camSpeed; }
        public Vector2 SpawnTimeRange { get => _spawnTimeRange / (_camSpeed * 0.75f); }
        public bool OnlyOneJump { get => _onlyOneJump; }

        public static GameManager Instance { get; private set; }

        private void Awake() {
            if (!Instance) Instance = this;
            else Destroy(this);
            if (stateChangeEvent == null) stateChangeEvent = new StateChangeEvent();
            GameBeginning();
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

        public void OnStateChange(GameState oldState, GameState newState) {
            if(newState == GameState.Paused || newState == GameState.EndGame) {
                Time.timeScale = 0;
            }

            if (newState == GameState.Playing || newState == GameState.Menu) {
                Time.timeScale = 1;
            }
        }

        /// <summary>
        /// Troca o estado do jogo entre pausado e jogando. Executando o evento stateChangeEvent.
        /// </summary>
        private void TogglePause() { if (Input.GetKeyDown(KeyCode.Escape)) State = InGame ? GameState.Paused : GameState.Playing; }

        /// <summary>
        /// Sai do menu e vai para o jogo.
        /// </summary>
        public void StartGame() {
            State = GameState.Playing;
        }

        /// <summary>
        /// Execução do fim do jogo.
        /// </summary>
        public void GameOver() {
            State = GameState.EndGame;
        }

        /// <summary>
        /// Executado no inicio e no restart do jogo.
        /// </summary>
        public void GameBeginning() {
            State = GameState.Menu;
        }
    }
}

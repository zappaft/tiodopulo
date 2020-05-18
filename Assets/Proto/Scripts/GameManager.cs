using System;
using UnityEngine;
using UnityEngine.Events;

namespace Prototipo {

    public class StateChangeEvent : UnityEvent<GameManager.GameState, GameManager.GameState> { }
    public class ScoreChangeEvent : UnityEvent<int> { }

    public class GameManager : MonoBehaviour {

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
                StateChangeEvent?.Invoke(_state, value);
                lastState = _state;
                _state = value;
            }
        }

        public bool InGame { get => State == GameState.Playing; }
        public bool InMenu { get => State == GameState.Menu; }
        public bool InPause { get => State == GameState.Paused; }
        public bool InEndGame { get => State == GameState.EndGame; }

        public StateChangeEvent StateChangeEvent;
        #endregion

        public static ScoreChangeEvent ScoreChangeEvent { get; private set; }

        [SerializeField] private bool _onlyOneJump;
        [SerializeField] private float _camSpeed;
        [SerializeField] private Vector2 _spawnTimeRange;

        public float CamSpeed { get => _camSpeed; }
        public Vector2 SpawnTimeRange { 
            get {
                return _spawnTimeRange / (_camSpeed * .90f);
            }
        }
        public bool OnlyOneJump { get => _onlyOneJump; }

        private int _score;
        public int Score { get => _score; private set { ScoreChangeEvent?.Invoke(value); _score = value; } }

        public static GameManager Instance { get; private set; }

        private void Awake() {
            if (!Instance) Instance = this;
            else Destroy(this);
            if (StateChangeEvent == null) StateChangeEvent = new StateChangeEvent();
            if (ScoreChangeEvent == null) ScoreChangeEvent = new ScoreChangeEvent();
            GameBeginning();
        }

        private void Start() {
            StateChangeEvent?.AddListener(OnStateChange);
            PlayerController.OnPlayerJumpEvent?.AddListener(OnPlayerJump);
            UIManager.DevMenuEvent?.AddListener(OnDevMenuChange);
            DevSetup();
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

        private void OnStateChange(GameState oldState, GameState newState) {
            if(newState == GameState.Paused || newState == GameState.EndGame) {
                Time.timeScale = 0;
            }

            if (newState == GameState.Playing || newState == GameState.Menu) {
                Time.timeScale = 1;
            }
        }

        private void OnPlayerJump() {
            Score++;
        }

        /// <summary>
        /// Troca o estado do jogo entre pausado e jogando. Executando o evento stateChangeEvent.
        /// </summary>
        private void TogglePause() { if (InputController.DeviceBasedInput(PlayerInputType.Pause)) State = InGame ? GameState.Paused : GameState.Playing; }

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

        private void OnDevMenuChange(DevOpts opts) {
            _onlyOneJump = opts.onlyOneJump;
            _camSpeed = opts.camSpeed;
        }

        private void DevSetup() {
            UIManager.initializer.onlyOneJump = Convert.ToBoolean(PlayerPrefs.GetInt("OnlyOneJump", _onlyOneJump ? 1 : 0));
            UIManager.initializer.camSpeed = PlayerPrefs.GetFloat("CamSpeed", _camSpeed);
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Prototipo {

    public class JumpbarChangeEvent : UnityEvent<float> { }
    public class PlayerJumpEvent : UnityEvent { }

    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour {

        #region PlayerState
        public enum PlayerState {
            Grounded,
            Jumping
        }
        private PlayerState _state;
        private PlayerState State { get => _state; set { _state = value; } }
        #endregion

        private bool firstCollision;
        private GameObject lastCollision;
        private bool repeatedCollision;

        private bool canJump;
        [SerializeField] private Vector2 jumpPowerbarRange;
        [SerializeField] private float jumpPowerbarModifier;
        [SerializeField] private float verticalJumpModifier;
        [SerializeField] private float horizontalJumpModifier;
        private bool positiveJumpbarPower;
        private bool shouldJump;
        private float _jumpbarPower;
        private float JumpbarPower {
            get => _jumpbarPower;
            set {
                _jumpbarPower = value;
                float adjustedValue = (((value - jumpPowerbarRange.x) * (Vector2.up.y - Vector2.up.x)) 
                    / (jumpPowerbarRange.y - jumpPowerbarRange.x)) + Vector2.up.x;
                OnJumpbarChangeEvent?.Invoke(adjustedValue);
            }
        }

        private Rigidbody2D rb;

        public static JumpbarChangeEvent OnJumpbarChangeEvent { get; private set; }
        public static PlayerJumpEvent OnPlayerJumpEvent { get; private set; }

        private void Awake() {
            if (OnJumpbarChangeEvent == null) OnJumpbarChangeEvent = new JumpbarChangeEvent();
            if (OnPlayerJumpEvent == null) OnPlayerJumpEvent = new PlayerJumpEvent();
        }

        private void Start() {
            GameManager.Instance.StateChangeEvent?.AddListener(OnStateChange);
            UIManager.DevMenuEvent?.AddListener(OnDevMenuChange);
            rb = GetComponent<Rigidbody2D>();
            canJump = true;
            firstCollision = true;

            DevSetup();
        }

        private void Update() {
            if (!GameManager.Instance.InGame) return;
            PlayerInput();
            VerifyPlayerState();
        }

        private void FixedUpdate() {
            if (!GameManager.Instance.InGame) return;
            Jump();
        }

        private void OnStateChange(GameManager.GameState oldState, GameManager.GameState newState) {
            //Debug.Log($"playercontroller state changed: {oldState} => {newState}");
        }

        /// <summary>
        /// Responsável por pegar o Input do jogador, em jogo apenas!
        /// Input de Menu ou outra coisa qualquer deve ser feito separadamente.
        /// </summary>
        private void PlayerInput() {
            if (!canJump) return;
            if (SystemInfo.deviceType == DeviceType.Desktop) {
                if (InputController.DeviceBasedInput(PlayerInputType.JumpDown) && State == PlayerState.Grounded) StartCoroutine("IncreasePowerbar");
                if (InputController.DeviceBasedInput(PlayerInputType.JumpUp) && State == PlayerState.Grounded) ReleaseJump();
            }
            else if(SystemInfo.deviceType == DeviceType.Handheld)
            {
                if (InputController.DeviceBasedInput(PlayerInputType.JumpDown) && State == PlayerState.Grounded) StartCoroutine("IncreasePowerbar");
                if (InputController.DeviceBasedInput(PlayerInputType.JumpUp) && State == PlayerState.Grounded) ReleaseJump();
            }
        }

        /// <summary>
        /// Aumenta ou diminui a barra de força do pulo, baseado no limite definido no vetor jumpPowerRange.
        /// </summary>
        /// <returns></returns>
        private IEnumerator IncreasePowerbar() {
            positiveJumpbarPower = true;
            JumpbarPower = jumpPowerbarRange.x;
            while (true) {
                if (JumpbarPower >= jumpPowerbarRange.y) positiveJumpbarPower = false;
                if (JumpbarPower <= jumpPowerbarRange.x) positiveJumpbarPower = true;

                JumpbarPower += jumpPowerbarModifier * Time.deltaTime * (positiveJumpbarPower ? 1 : -1);
                Debug.Log(JumpbarPower);
                yield return null;
            }
        }

        /// <summary>
        /// Permite que o FixedUpdate execute a física do pulo.
        /// </summary>
        private void ReleaseJump() {
            StopCoroutine("IncreasePowerbar");
            shouldJump = true;
        }

        /// <summary>
        /// Adiciona impulso no rigidbody baseado no verticalJumpModifier, horizontalJumpModifier and JumpbarPower.
        /// </summary>
        private void Jump() {
            if (shouldJump) {
                shouldJump = false;
                rb.AddForce(
                    ((Vector2.up * verticalJumpModifier) + (Vector2.right * horizontalJumpModifier)) * JumpbarPower,
                    ForceMode2D.Impulse);
            }
        }

        /// <summary>
        /// Verifica a velocidade do rigidbody para definir o estado do jogador.
        /// </summary>
        private void VerifyPlayerState() {
            PlayerState _state = rb.velocity == Vector2.zero ? PlayerState.Grounded : PlayerState.Jumping;
            if (State != _state) State = _state;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.CompareTag("Platform")) {
                if (collision.gameObject != lastCollision) {
                    repeatedCollision = false;
                    lastCollision = collision.gameObject;
                    if (firstCollision) firstCollision = false;
                    else OnPlayerJumpEvent?.Invoke();
                    if (GameManager.Instance.OnlyOneJump) canJump = true;
                } else {
                    repeatedCollision = true;
                    if (GameManager.Instance.OnlyOneJump) canJump = false;
                }
            }
            if (collision.CompareTag("CreateNewTemplate"))
            {
                GetComponent<SpawnTemplate>().InstantiateNewTemplate();
            }
            if (collision.CompareTag("Death")) {
                GameManager.Instance.GameOver();
            }
        }

        private void OnDevMenuChange(DevOpts opts) {
            jumpPowerbarRange = new Vector2(opts.minJumpbar, opts.maxJumpbar);
            jumpPowerbarModifier = opts.jumpbarModifier;
            verticalJumpModifier = opts.verticalJump;
            horizontalJumpModifier = opts.horizontalJump;
            if (!opts.onlyOneJump) canJump = true;
        }

        private void DevSetup() {
            UIManager.initializer.jumpbarModifier = PlayerPrefs.GetFloat("JumpbarModifier", jumpPowerbarModifier);
            UIManager.initializer.verticalJump = PlayerPrefs.GetFloat("VerticalJump", verticalJumpModifier);
            UIManager.initializer.horizontalJump = PlayerPrefs.GetFloat("HorizontalJump", horizontalJumpModifier);
            UIManager.initializer.minJumpbar = PlayerPrefs.GetFloat("MinJumpbar", jumpPowerbarRange.x);
            UIManager.initializer.maxJumpbar = PlayerPrefs.GetFloat("MaxJumpbar", jumpPowerbarRange.y);
        }
    }
}
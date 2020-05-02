
namespace Prototipo {

    public interface IStatedBehaviour {

        /// <summary>
        /// Callback do evento de mudança de estado do jogo, deve ser adicionado como listener ao evento alvo.
        /// </summary>
        /// <param name="oldState">Estado antigo.</param>
        /// <param name="newState">Novo estado.</param>
        void OnStateChange(GameManager.GameState oldState, GameManager.GameState newState);
    }
}

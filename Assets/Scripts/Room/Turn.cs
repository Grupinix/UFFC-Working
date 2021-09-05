using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using JsonClasses;
using Menu;
using UnityEngine;
using UnityEngine.UI;

namespace Room {
    
    /**
     * Classe responsável por gerenciar
     * em tempo real o sistema de turnos
     * da partida
     */
    public class Turn : MonoBehaviour {
        [SerializeField] private Button turno;
        public List<GameObject> allyCards;
        public List<GameObject> enemyCards;
        
        private string _oldPlayerTurn;
        public string _nameOfRoom;
        private string _playerTwoUid;

        private UserInterface _userInterface;

        public bool userTurn;

        private DatabaseReference _turnReference;
        private DatabaseReference _eventReference;

        private int _oldEvent;

        private void Awake() {
            GameObject.FindGameObjectsWithTag("music")[0].GetComponent<Sound>().playMusic(1);
        }

        private void Start() {
            _nameOfRoom = PlayerPrefs.GetString("room", null);
            _userInterface = GetComponent<UserInterface>();

            _turnReference = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("turn");
            _eventReference = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("event");
            
            _turnReference.ValueChanged += handleTurnChanged;
            _eventReference.ValueChanged += handleEventChanged;
        }
        
        private void OnDisable() {
            if (_turnReference != null) {
                _turnReference.ValueChanged -= handleTurnChanged;
                _turnReference = null;
            }

            if (_eventReference != null) {
                _eventReference.ValueChanged -= handleEventChanged;
                _eventReference = null;
            }
        }

        /** "ação" responsável por passar o turno para o próximo jogador */
        public void passarTurno() {
            turno.gameObject.SetActive(false);
            userTurn = false;
            StartCoroutine(goToNextTurn());
        }

        /** Define o turno como do outro jogador de maneira assincrona */
        private IEnumerator goToNextTurn() {
            if (_playerTwoUid == null) {
                yield return getSeccondPlayer();
            }
            
            string nextPlayerUidTurn = DatabaseAPI.user.UserId == _nameOfRoom ? _playerTwoUid : _nameOfRoom;

            IDictionary<string, object> data = new Dictionary<string, object> {
                {"turn", nextPlayerUidTurn}
            };
            Task task = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).UpdateChildrenAsync(data);

            yield return new WaitUntil(() => task.IsCompleted);
        }

        /** Carrega de maneira assincrona a UUID do segundo jogador */
        private IEnumerator getSeccondPlayer() {
            Task<DataSnapshot> taskSet = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("playerTwo").GetValueAsync();
            yield return new WaitUntil(() => taskSet.IsCompleted);
            if (!taskSet.Result.Exists) {
                yield break;
            }

            _playerTwoUid = taskSet.Result.Value.ToString();
        }

        /**
         * Inicia o turno do jogador definindo
         * variáveis padrões para o mesmo poder
         * realizar ações
         */
        private void turnEvent(string playerTurn) {
            if (playerTurn != DatabaseAPI.user.UserId) {
                return;
            }
            userTurn = true;
            
            for (int i = 0; i < 3; i++) {
                CardProperties allycard = allyCards[i].GetComponent<CardProperties>();
                if (allycard.cardId != 9999) {
                    allycard.ataque = true;
                }
            }
            
            _userInterface.canDropMana = true;
            _userInterface.resetMana();
            turno.gameObject.SetActive(true);
            DeckController.canBuy = true;
        }

        /**
         * Transforma a condição atual da
         * partida em um CardEvent e retorna
         * se baseando em qual jogador é o
         * segundo jogador
         */
        public CardEvent getField() {
            if (DatabaseAPI.user.UserId == _nameOfRoom) {
                return generateCardEvent(allyCards, enemyCards, _userInterface.life, _userInterface.enemyLife);
            }

            return generateCardEvent(enemyCards, allyCards, _userInterface.enemyLife, _userInterface.life);
        }

        /**
         * Transforma a condição atual da
         * partida em um CardEvent e retorna
         * o mesmo
         */
        private CardEvent generateCardEvent(IReadOnlyList<GameObject> listOne, IReadOnlyList<GameObject> listTwo, int lifeOne, int lifeTwo) {
            CardEvent cardEvent = new CardEvent();
            string cardsOne = "";
            string cardsTwo = "";
            for (int i = 0; i < 3; i++) {
                CardProperties cardAlly = listOne[i].GetComponent<CardProperties>();
                CardProperties cardEnemy = listTwo[i].GetComponent<CardProperties>();

                if (i == 0) {
                    cardsOne += $"{cardAlly.cardId}:{cardAlly.cardPower}:{cardAlly.cardDefense}";
                    cardsTwo += $"{cardEnemy.cardId}:{cardEnemy.cardPower}:{cardEnemy.cardDefense}";
                }
                else {
                    cardsOne += $"x{cardAlly.cardId}:{cardAlly.cardPower}:{cardAlly.cardDefense}";
                    cardsTwo += $"x{cardEnemy.cardId}:{cardEnemy.cardPower}:{cardEnemy.cardDefense}";
                }
            }

            ++_oldEvent;
            cardEvent.vidaPlayerOne = lifeOne;
            cardEvent.vidaPlayerTwo = lifeTwo;
            cardEvent.cardsPlayerOne = cardsOne;
            cardEvent.cardsPlayerTwo = cardsTwo;
            cardEvent.id = _oldEvent;
            return cardEvent;
        }

        /**
         * Verifica se o campo de batalha
         * do usuário está cheio
         */
        public bool campoCheio() {
            for (int i = 0; i < 3; i++) {
                CardProperties cardProperties = allyCards[i].GetComponent<CardProperties>();
                if (cardProperties.cardId == 9999) {
                    return false;
                }
            }

            return true;
        }

        /**
         * Insere uma carta no campo
         * de batalha do usuário
         */
        public void inserirCarta(int cardId) {
            for (int i = 0; i < 3; i++) {
                CardProperties cardProperties = allyCards[i].GetComponent<CardProperties>();
                if (cardProperties.cardId == 9999) {
                    cardProperties.cardId = cardId;
                    cardProperties.setMaterial();
                    IDictionary<string, object> data = new Dictionary<string, object> {
                        {"event", JsonUtility.ToJson(getField())}
                    };
                    DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).UpdateChildrenAsync(data);
                    break;
                }
            }
        }
        
        /**
         * Listener responsável por verificar
         * de qual jogador é o turno
         */
        private void handleTurnChanged(object sender, ValueChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            
            string playerTurn = args.Snapshot.Value.ToString();
            if (playerTurn == _oldPlayerTurn) {
                return;
            }
            _oldPlayerTurn = playerTurn;
            turnEvent(playerTurn);
        }

        /**
         * Listener responsável por atualiza
         * a condição e estado atual da partida
         */
        private void handleEventChanged(object sender, ValueChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            
            CardEvent cardEvent = JsonUtility.FromJson<CardEvent>(args.Snapshot.Value.ToString());
            if (_oldEvent == cardEvent.id) {
                return;
            }
            
            _oldEvent = cardEvent.id;
            setField(cardEvent);
        }
        
        /**
         * Atualiza a condição atual da partida
         * se baseando em um CardEvent verificando
         * qual jogador é o segundo jogador
         */
        private void setField(CardEvent cardEvent) {
            if (DatabaseAPI.user.UserId == _nameOfRoom) {
                setCardToField(allyCards, enemyCards, cardEvent, cardEvent.vidaPlayerOne, cardEvent.vidaPlayerTwo);
                return;
            }
            setCardToField(enemyCards, allyCards, cardEvent, cardEvent.vidaPlayerTwo, cardEvent.vidaPlayerOne);
        }

        /**
         * Atualiza a condição atual da partida
         * se baseando em um CardEvent
         */
        private void setCardToField(IReadOnlyList<GameObject> listOne,IReadOnlyList<GameObject> listTwo, CardEvent cardEvent, int lifeOne, int lifeTwo) {
            string[] cardsOne = cardEvent.cardsPlayerOne.Split('x');
            string[] cardsTwo = cardEvent.cardsPlayerTwo.Split('x');
            
            for (int i = 0; i < 3; i++) {
                CardProperties cardAlly = listOne[i].GetComponent<CardProperties>();
                CardProperties cardEnemy = listTwo[i].GetComponent<CardProperties>();

                string[] cardStatusOne = cardsOne[i].Split(':');
                string[] cardStatusTwo = cardsTwo[i].Split(':');

                cardAlly.cardId = int.Parse(cardStatusOne[0]);
                cardEnemy.cardId = int.Parse(cardStatusTwo[0]);

                cardAlly.setMaterial();
                cardEnemy.setMaterial();

                cardAlly.cardPower = int.Parse(cardStatusOne[1]);
                cardAlly.cardDefense = int.Parse(cardStatusOne[2]);
                cardEnemy.cardPower = int.Parse(cardStatusTwo[1]);
                cardEnemy.cardDefense = int.Parse(cardStatusTwo[2]);
            }

            _userInterface.life = lifeOne;
            _userInterface.enemyLife = lifeTwo;
            _userInterface.attVidaDisplay();
        }
    }
}
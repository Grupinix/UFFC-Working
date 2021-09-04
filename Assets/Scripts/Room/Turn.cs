using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using JsonClasses;
using Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Room {
    public class Turn : MonoBehaviour {
        [SerializeField] private Button turno;
        public List<GameObject> allyCards;
        public List<GameObject> enemyCards;

        private string _oldPlayerTurn;
        private string _nameOfRoom;
        private string _playerTwoUid;

        private UserInterface _userInterface;

        public bool userTurn;

        private void Start() {
            GameObject.FindGameObjectsWithTag("music")[0].GetComponent<Sound>().playMusic(1);
            _nameOfRoom = PlayerPrefs.GetString("room", null);
            StartCoroutine(getSeccondPlayer());

            StartCoroutine(turnCheckEvent());
            _userInterface = GetComponent<UserInterface>();
        }

        public void passarTurno() {
            turno.gameObject.SetActive(false);
            userTurn = false;
            StartCoroutine(goToNextTurn());
        }

        private IEnumerator goToNextTurn() {
            string nextPlayerUidTurn = DatabaseAPI.user.UserId == _nameOfRoom ? _playerTwoUid : _nameOfRoom;


            Task taskOne = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("event").SetValueAsync(JsonUtility.ToJson(getField()));
            yield return new WaitUntil(() => taskOne.IsCompleted);


            Task taskTwo = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("turn").SetValueAsync(nextPlayerUidTurn);
            yield return new WaitUntil(() => taskTwo.IsCompleted);

            if (_userInterface.enemyLife <= 0) {
                PlayerPrefs.SetInt("playerWins", PlayerPrefs.GetInt("playerWins", 0) + 1);
                PlayerPrefs.Save();
                DatabaseAPI.getDatabase().Child("users").Child(DatabaseAPI.user.UserId).Child("wins").SetValueAsync(long.Parse(PlayerPrefs.GetInt("playerWins").ToString()));
                SceneManager.LoadScene("Lobby");
            } 
            if (_userInterface.life <= 0) {
                PlayerPrefs.SetInt("playerLoses", PlayerPrefs.GetInt("playerLoses", 0) + 1);
                PlayerPrefs.Save();
                SceneManager.LoadScene("Lobby");
            }
        }

        private IEnumerator getSeccondPlayer() {
            Task<DataSnapshot> taskSet = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("playerTwo").GetValueAsync();
            yield return new WaitUntil(() => taskSet.IsCompleted);
            if (!taskSet.Result.Exists) {
                yield break;
            }

            _playerTwoUid = taskSet.Result.Value.ToString();
        }

        private IEnumerator turnEvent(string playerTurn) {
            if (playerTurn != DatabaseAPI.user.UserId) {
                yield break;
            }
            userTurn = true;

            Task<DataSnapshot> turn = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("event").GetValueAsync();
            yield return new WaitUntil(() => turn.IsCompleted);

            CardEvent cardEvent = JsonUtility.FromJson<CardEvent>(turn.Result.Value.ToString());
            setField(cardEvent);
            
            if (_userInterface.enemyLife <= 0) {
                PlayerPrefs.SetInt("playerWins", PlayerPrefs.GetInt("playerWins", 0) + 1);
                PlayerPrefs.Save();
                DatabaseAPI.getDatabase().Child("users").Child(DatabaseAPI.user.UserId).Child("wins").SetValueAsync(long.Parse(PlayerPrefs.GetInt("playerWins").ToString()));
                SceneManager.LoadScene("Lobby");
                yield break;
            } 
            if (_userInterface.life <= 0) {
                PlayerPrefs.SetInt("playerLoses", PlayerPrefs.GetInt("playerLoses", 0) + 1);
                PlayerPrefs.Save();

                SceneManager.LoadScene("Lobby");
                yield break;
            }

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

        private CardEvent getField() {
            if (DatabaseAPI.user.UserId == _nameOfRoom) {
                return generateCardEvent(allyCards, enemyCards, _userInterface.life, _userInterface.enemyLife);
            }

            return generateCardEvent(enemyCards, allyCards, _userInterface.enemyLife, _userInterface.life);
        }

        private CardEvent generateCardEvent(List<GameObject> listOne, List<GameObject> listTwo, int lifeOne, int lifeTwo) {
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

            cardEvent.vidaPlayerOne = lifeOne;
            cardEvent.vidaPlayerTwo = lifeTwo;
            cardEvent.cardsPlayerOne = cardsOne;
            cardEvent.cardsPlayerTwo = cardsTwo;
            return cardEvent;
        }

        public bool campoCheio() {
            for (int i = 0; i < 3; i++) {
                CardProperties cardProperties = allyCards[i].GetComponent<CardProperties>();
                if (cardProperties.cardId == 9999) {
                    return false;
                }
            }

            return true;
        }

        public void inserirCarta(int cardId) {
            for (int i = 0; i < 3; i++) {
                CardProperties cardProperties = allyCards[i].GetComponent<CardProperties>();
                if (cardProperties.cardId == 9999) {
                    cardProperties.cardId = cardId;
                    cardProperties.setMaterial();
                    break;
                }
            }
        }

        private void setField(CardEvent cardEvent) {
            if (DatabaseAPI.user.UserId == _nameOfRoom) {
                setCardToField(allyCards, enemyCards, cardEvent, cardEvent.vidaPlayerOne, cardEvent.vidaPlayerTwo);
            }
            else {
                setCardToField(enemyCards, allyCards, cardEvent, cardEvent.vidaPlayerTwo, cardEvent.vidaPlayerOne);
            }
        }

        private void setCardToField(List<GameObject> listOne, List<GameObject> listTwo, CardEvent cardEvent, int lifeOne, int lifeTwo) {
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

        private IEnumerator turnCheckEvent() {
            Task<DataSnapshot> turn = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("turn").GetValueAsync();

            yield return new WaitUntil(() => turn.IsCompleted);
            if (!turn.Result.Exists) {
                yield break;
            }

            string playerTurn = turn.Result.Value.ToString();
            if (playerTurn != _oldPlayerTurn) {
                _oldPlayerTurn = playerTurn;
                StartCoroutine(turnEvent(playerTurn));
            }

            yield return new WaitForSeconds(2);
            StartCoroutine(turnCheckEvent());
        }
    }
}
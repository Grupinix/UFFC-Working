using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using JsonClasses;
using UnityEngine;
using UnityEngine.UI;

namespace Room {
    public class Turn : MonoBehaviour {

        [SerializeField] private Button turno;
        [SerializeField] private List<GameObject> allyCards;
        [SerializeField] private List<GameObject> enemyCards;
        
        private bool nextTurn;
        private string _oldPlayerTurn;
        private string _nameOfRoom;
        private string _playerTwoUID;

        private void Start() { 
            _nameOfRoom = PlayerPrefs.GetString("room", null);
            StartCoroutine(getSeccondPlayer());
            
            StartCoroutine(turnCheckEvent());
        }

        public void passarTurno() {
            nextTurn = true;
        }

        public IEnumerator getSeccondPlayer() {
            Task<DataSnapshot> taskSet = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("playerTwo").GetValueAsync();
            yield return new WaitUntil(() => taskSet.IsCompleted);
            if (!taskSet.Result.Exists) {
                yield break;
            }

            _playerTwoUID = taskSet.Result.Value.ToString();
        }

        private IEnumerator turnEvent(string playerTurn) {
            if (playerTurn != DatabaseAPI.user.UserId) {
                yield break;
            }
            
            turno.gameObject.SetActive(true);
            DeckController.canBuy = true;
            Task<DataSnapshot> turn = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("event").GetValueAsync();
            yield return new WaitUntil(() => turn.IsCompleted);

            CardEvent cardEvent = JsonUtility.FromJson<CardEvent>(turn.Result.Value.ToString());
            setField(cardEvent);

            while (!nextTurn) {
                yield return new WaitForSeconds(1);
            }

            nextTurn = false;
            turno.gameObject.SetActive(false);
            string nextPlayerUidTurn = playerTurn == _nameOfRoom ? _playerTwoUID : _nameOfRoom;
            Task taskOne = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("event").SetValueAsync(JsonUtility.ToJson(getField()));
            yield return new WaitUntil(() => taskOne.IsCompleted);
            Task taskTwo = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("turn").SetValueAsync(nextPlayerUidTurn);
            yield return new WaitUntil(() => taskTwo.IsCompleted);
        }

        private CardEvent getField() {
            if (DatabaseAPI.user.UserId == _nameOfRoom) {
                return generateCardEvent(allyCards, enemyCards);
            }
            return generateCardEvent(enemyCards, allyCards);
        }

        private CardEvent generateCardEvent(List<GameObject> listOne, List<GameObject> listTwo) {
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

            cardEvent.cardsPlayerOne = cardsOne;
            cardEvent.cardsPlayerTwo = cardsTwo;
            return cardEvent;
        }

        private void setField(CardEvent cardEvent) {
            if (DatabaseAPI.user.UserId == _nameOfRoom) {
                setCardToField(allyCards, enemyCards, cardEvent);
            }
            else {
                setCardToField(enemyCards, allyCards, cardEvent);
            }
        }

        private void setCardToField(List<GameObject> listOne, List<GameObject> listTwo, CardEvent cardEvent) {
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

            yield return new WaitForSeconds(1);
            StartCoroutine(turnCheckEvent());
        }

    }
}
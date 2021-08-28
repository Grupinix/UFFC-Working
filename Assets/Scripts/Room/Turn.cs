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

        private void Start() { 
            _nameOfRoom = PlayerPrefs.GetString("room", null);
            
            StartCoroutine(turnCheckEvent());
        }

        private async Task turnEvent(string playerTurn) {
            if (playerTurn != DatabaseAPI.user.UserId) {
                return;
            }

            DeckController.canBuy = true;
            Task<DataSnapshot> turn = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("event").GetValueAsync();
            await turn;

            CardEvent cardEvent = JsonUtility.FromJson<CardEvent>(turn.Result.Value.ToString());
            setField(cardEvent);

            while (!nextTurn) {
                await Task.Delay(1000);
            }
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
            for (int i = 0; i < 3; i++) {
                CardProperties cardAlly = listOne[i].GetComponent<CardProperties>();
                CardProperties cardEnemy = listTwo[i].GetComponent<CardProperties>();

                cardAlly.cardId = cardEvent.cardsPlayerOne[i, 0];
                cardEnemy.cardId = cardEvent.cardsPlayerTwo[i, 0];

                cardAlly.setMaterial();
                cardEnemy.setMaterial();

                cardAlly.cardPower = cardEvent.cardsPlayerOne[i, 1];
                cardAlly.cardDefense = cardEvent.cardsPlayerOne[i, 2];
                cardEnemy.cardPower = cardEvent.cardsPlayerTwo[i, 1];
                cardEnemy.cardDefense = cardEvent.cardsPlayerTwo[i, 2];
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
                Task task = turnEvent(playerTurn);
                task.Wait();
            }

            yield return new WaitForSeconds(1);
            StartCoroutine(turnCheckEvent());
        }

    }
}
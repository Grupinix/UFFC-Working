using System.Collections;
using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using JsonClasses;
using UnityEngine;
using UnityEngine.UI;

namespace Room {
    public class Turn : MonoBehaviour {

        [SerializeField] private Button turno;
        
        private string _oldPlayerTurn;
        private string _nameOfRoom;
        private CardEvent _lastEvent;

        private void Start() { 
            _nameOfRoom = PlayerPrefs.GetString("room", null);
            
            StartCoroutine(turnCheckEvent());
            StartCoroutine(cardEventCheckEvent());
        }

        private void turnEvent(bool isTurn) {
            
        }

        private void cardActionEvent() {
            
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
                turnEvent(turnDecodificate(playerTurn));
            }

            yield return new WaitForSeconds(1);
            StartCoroutine(turnCheckEvent());
        }

        private IEnumerator cardEventCheckEvent() {
            Task<DataSnapshot> turn = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("event").GetValueAsync();

            yield return new WaitUntil(() => turn.IsCompleted);
            if (!turn.Result.Exists) {
                yield break;
            }
            
            CardEvent cardEvent = JsonUtility.FromJson<CardEvent>(turn.Result.Value.ToString());
            if (ReferenceEquals(_lastEvent, null) || cardEvent.id != _lastEvent.id) {
                _lastEvent = cardEvent;
                cardActionEvent();
            }

            yield return new WaitForSeconds(1);
            StartCoroutine(cardEventCheckEvent());
        }

        private bool turnDecodificate(string playerUid) {
            return playerUid == DatabaseAPI.user.UserId;
        }
    }
}
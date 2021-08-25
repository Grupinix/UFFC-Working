using System.Collections;
using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

namespace Room {
    public class Turn : MonoBehaviour {

        [SerializeField] private Button turno;
        
        private string _oldPlayerTurn;
        private string _nameOfRoom;
        
        private void Start() { 
            _nameOfRoom = PlayerPrefs.GetString("room", null);
            
            StartCoroutine(turnCheckEvent());
        }

        private void turnEvent(bool isTurn) {
            
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

        private bool turnDecodificate(string playerUid) {
            return playerUid == DatabaseAPI.user.UserId;
        }
    }
}
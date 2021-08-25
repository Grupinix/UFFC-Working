using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Room {
    public class Turn : MonoBehaviour {

        [SerializeField] private Button turno;
        
        private string _nameOfRoom;
        private bool _playerOne;
        private string _stage;
        private bool _myTime;
        
        private void Start() { 
            _nameOfRoom = PlayerPrefs.GetString("room", null);
            _playerOne = _nameOfRoom == DatabaseAPI.user.UserId;
            _myTime = _playerOne;

            turno.onClick.AddListener(executeTurn);

            verifyTurn();
        }

        private void executeTurn() {
            if (!_myTime) {
                return;
            }

            if (_stage == "MAIN") {
                _stage = "BATTLE";
                turno.GetComponent<Text>().text = "BATTLE";
            }
            else if (_stage == "BATTLE") {
                _stage = "MAIN";
                _myTime = false;
                passTurn();
            }
        }

        private async void passTurn() {
            string nextPlayer = _playerOne ? "playerTwo" : "playerOne";
            Task task = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("turn").SetValueAsync(nextPlayer);
            await Task.WhenAll(task);
            verifyTurn();
        }

        private async void verifyTurn() {
            Task<DataSnapshot> turn = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("turn").GetValueAsync();

            await Task.WhenAll(turn);
            if (!turn.Result.Exists) {
                return;
            }

            string playerTurn = turn.Result.Value.ToString();
            if (playerTurn == "playerOne" && _playerOne || playerTurn != "playerOne" && !_playerOne) {
                runTurn();
            }
            else {
                turno.interactable = false;
                turno.GetComponent<Text>().text = "TURNO DO OPONENTE";
                
                await Task.Delay(1000);
                verifyTurn();
            }
        }

        private void runTurn() {
            _myTime = true;

            turno.interactable = true;
            turno.GetComponent<Text>().text = "MAIN";
        }
    }
}
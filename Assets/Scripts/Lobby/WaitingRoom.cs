using System.Collections.Generic;
using APIs;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby {
    public class WaitingRoom : MonoBehaviour {
        [SerializeField] private string roomSceneName;
        
        private DatabaseReference _roomVerify;
        private string _nameOfRoom;
        
        private void Start() { 
            _nameOfRoom = PlayerPrefs.GetString("room", null);

            _roomVerify = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("read");
            _roomVerify.ValueChanged += handleRoomStatusChange;
        }
        
        private void OnDisable() {
            if (_roomVerify != null) {
                _roomVerify.ValueChanged -= handleRoomStatusChange;
                _roomVerify = null;
            }
        }

        private void handleRoomStatusChange(object sender, ValueChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            
            if (!args.Snapshot.Exists) {
                IDictionary<string, object> data = new Dictionary<string, object> {
                    {"read", "false"}
                };
                DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).UpdateChildrenAsync(data);
                return;
            }

            if (!args.Snapshot.Value.ToString().Equals("true")) {
                return;
            }
            
            PlayerPrefs.SetInt("games", PlayerPrefs.GetInt("games", 0) + 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene(roomSceneName);
        }
    }
}
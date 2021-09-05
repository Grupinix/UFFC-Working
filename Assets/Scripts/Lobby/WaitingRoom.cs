using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lobby {
    public class WaitingRoom : MonoBehaviour {
        [SerializeField] private string roomSceneName;
        [SerializeField] private Text time;
        
        private DatabaseReference _roomVerify;
        private string _nameOfRoom;
        private long _currentTimePassed;
        
        private void Start() { 
            _nameOfRoom = PlayerPrefs.GetString("room", null);

            _roomVerify = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("read");
            _roomVerify.ValueChanged += handleRoomStatusChange;
            StartCoroutine(createdTime());
        }

        private void Update() {
            if (_currentTimePassed == 0) {
                return;
            }
            
            long actualMillis = DatabaseAPI.currentTimeMillis();
            actualMillis = actualMillis / 1000;
            if (actualMillis > _currentTimePassed) {
                loadLobby();
            }
            else {
                time.text = ((int) (_currentTimePassed - actualMillis)).ToString();
            }
        }

        public void loadLobby() {
            SceneManager.LoadScene("Lobby");
        }

        private void OnDisable() {
            if (_roomVerify != null) {
                _roomVerify.ValueChanged -= handleRoomStatusChange;
                _roomVerify = null;
            }
        }

        private IEnumerator createdTime() {
            Task<DataSnapshot> task = DatabaseAPI.getDatabase().Child("rooms").Child(_nameOfRoom).Child("created").GetValueAsync();

            yield return new WaitUntil(() => task.IsCompleted);
            
            _currentTimePassed = long.Parse(task.Result.Value.ToString());
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
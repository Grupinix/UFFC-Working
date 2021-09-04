using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using JsonClasses;
using Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lobby {
    public class Lobby : MonoBehaviour {

        [SerializeField] private string waitingSceneName;
        [SerializeField] private string roomSceneName;
        
        [SerializeField] private InputField inputField;
        [SerializeField] private GameObject parent;
        [SerializeField] private GameObject element;

        private DatabaseReference _roomReference;

        private void Awake() {
            GameObject.FindGameObjectsWithTag("music")[0].GetComponent<Sound>().playMusic(0);
        }

        private void Start() {
            _roomReference = DatabaseAPI.getDatabase().Child("rooms");
            _roomReference.ValueChanged += handleRoomChanged;
        }
        
        private void OnDisable() {
            if (_roomReference != null) {
                _roomReference.ValueChanged -= handleRoomChanged;
                _roomReference = null;
            }
        }
        
        public void loadChangeDeckScene() {
            SceneManager.LoadScene("ChangeDeck");
        }

        public void carregarPerfil() {
            SceneManager.LoadScene("Perfil");
        }

        public void sairDoJogo() {
            Application.Quit();
        }

        public void createRoom() {
            string nameOfRoom = inputField.text;
            string userUid = DatabaseAPI.user.UserId;

            if (nameOfRoom == "") {
                return;
            }
            
            createRoomOnDatabase(userUid, nameOfRoom, waitingSceneName);
        }

        private async void createRoomOnDatabase(string uid, string roomName, string waitScene) {
            Task taskSetZero = DatabaseAPI.getDatabase().Child("rooms").Child(uid).RemoveValueAsync();
            Task taskSetOne = DatabaseAPI.getDatabase().Child("rooms").Child(uid).Child("uid").SetValueAsync(uid);
            Task taskSetTwo = DatabaseAPI.getDatabase().Child("rooms").Child(uid).Child("read").SetValueAsync("false");
            Task taskSetTree = DatabaseAPI.getDatabase().Child("rooms").Child(uid).Child("roomName").SetValueAsync(roomName);
            Task taskSetFour = DatabaseAPI.getDatabase().Child("rooms").Child(uid).Child("turn").SetValueAsync(uid);
            Task taskSetFive = DatabaseAPI.getDatabase().Child("rooms").Child(uid).Child("event").SetValueAsync(JsonUtility.ToJson(new CardEvent()));

            await Task.WhenAll(taskSetZero, taskSetOne, taskSetTwo, taskSetTree, taskSetFour, taskSetFive);

            PlayerPrefs.SetString("room", uid);
            PlayerPrefs.Save();
            SceneManager.LoadScene(waitScene);
        }
        
        private void handleRoomChanged(object sender, ValueChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            
            foreach (Transform child in parent.transform) {
                Destroy(child.gameObject);
            }
            
            foreach (DataSnapshot childSnapshot in args.Snapshot.Children) {
                if (!childSnapshot.Exists 
                    || !childSnapshot.Child("read").Exists
                    || !childSnapshot.Child("roomName").Exists
                    || !childSnapshot.Child("uid").Exists
                    || childSnapshot.Child("read").Value.ToString() == "true") {
                    continue;
                }

                string roomName = childSnapshot.Child("roomName").Value.ToString();
                string uid = childSnapshot.Child("uid").Value.ToString();

                GameObject roomButton = Instantiate(element, parent.transform);
                Button button = roomButton.GetComponent<Button>();
                Text buttonText = roomButton.GetComponentInChildren<Text>();
                buttonText.text = roomName;
                button.onClick.AddListener(() => {
                    loadRoom(uid, roomSceneName);
                });
            }
        }

        private async void loadRoom(string uid, string roomScene) {
            Task taskSet = DatabaseAPI.getDatabase().Child("rooms").Child(uid).Child("playerTwo").SetValueAsync(DatabaseAPI.user.UserId);
            Task taskSetTwo = DatabaseAPI.getDatabase().Child("rooms").Child(uid).Child("read").SetValueAsync("true");

            await Task.WhenAll(taskSet, taskSetTwo);

            PlayerPrefs.SetString("room", uid);
            PlayerPrefs.SetString("battleEnemy", uid);
            PlayerPrefs.SetInt("games", PlayerPrefs.GetInt("games", 0) + 1);
            PlayerPrefs.Save();
            
            SceneManager.LoadScene(roomScene);
        }
    }
}
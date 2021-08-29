using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby {
    public class WaitingRoom : MonoBehaviour {
        [SerializeField] private string roomSceneName;

        private void Start() { 
            verifyRoom();
        }

        private async void verifyRoom() {
            string nameOfRoom = PlayerPrefs.GetString("room", null);
            if (nameOfRoom == null) {
                return;
            }
            
            Task<DataSnapshot> data = DatabaseAPI.getDatabase().Child("rooms").Child(nameOfRoom).Child("read").GetValueAsync();

            await Task.WhenAll(data);
            if (!data.Result.Exists) {
                return;
            }

            if (data.Result.Value.ToString().Equals("true")) {
                PlayerPrefs.SetInt("games", PlayerPrefs.GetInt("games", 0) + 1);
                PlayerPrefs.Save();
                SceneManager.LoadScene(roomSceneName);
            }
            else {
                await Task.Delay(1000);
                verifyRoom();
            }
        }
    }
}
using System.Collections;
using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby {
    public class WaitingRoom : MonoBehaviour {
        [SerializeField] private string roomSceneName;

        private void Start() { 
            StartCoroutine(verifyRoom());
        }

        private IEnumerator verifyRoom() {
            string nameOfRoom = PlayerPrefs.GetString("room", null);
            if (nameOfRoom == null) {
                yield break;
            }
            
            Task<DataSnapshot> data = DatabaseAPI.getDatabase().Child("rooms").Child(nameOfRoom).Child("read").GetValueAsync();

            yield return new WaitUntil(() => data.IsCompleted);
            if (!data.Result.Exists) {
                yield break;
            }

            if (data.Result.Value.ToString().Equals("true")) {
                PlayerPrefs.SetInt("games", PlayerPrefs.GetInt("games", 0) + 1);
                PlayerPrefs.Save();
                SceneManager.LoadScene(roomSceneName);
            }
            else {
                yield return new WaitForSeconds(2);
                StartCoroutine(verifyRoom());
            }
        }
    }
}
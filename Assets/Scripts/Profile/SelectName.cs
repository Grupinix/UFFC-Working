using System.Threading.Tasks;

using APIs;

using Firebase.Database;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Profile {
    public class SelectName : MonoBehaviour {

        [SerializeField] private string nextScene;
    
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        
        private void Start() {
            Button btn = button.GetComponent<Button>();

            btn.onClick.AddListener(buttonClickEvent);
        }

        private void buttonClickEvent() {
            string nameOfPlayer = DatabaseAPI.slugify(inputField.text);
            PlayerPrefs.SetString("playerName", inputField.text);
            PlayerPrefs.SetString("playerName_slug", nameOfPlayer);
            checkName(DatabaseAPI.getAsyncData("player/" + nameOfPlayer), nameOfPlayer, nextScene);
        }

        private static async void checkName(Task<DataSnapshot> data, string nameOfPlayer, string scene) {
            await Task.WhenAll(data);
            
            DatabaseAPI.setAsyncData("player/" + nameOfPlayer, false);
            SceneManager.LoadScene(scene);
        }
    }
}

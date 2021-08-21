using APIs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Profile {
    public class SelectName : MonoBehaviour {

        [SerializeField] private string nextScene;
    
        [SerializeField] private InputField iField;
        [SerializeField] private Button button;
        
        private void Start() {
            button.onClick.AddListener(buttonClickEvent);
        }

        private void buttonClickEvent() {
            string nameOfPlayer = DatabaseAPI.slugify(iField.text);

            PlayerPrefs.SetString("playerName", iField.text);
            PlayerPrefs.Save();
            
            PlayerPrefs.SetString("playerName_slug", nameOfPlayer);
            PlayerPrefs.Save();

            DatabaseAPI.setAsyncData("player/" + nameOfPlayer, false);
            SceneManager.LoadScene(nextScene);
        }
    }
}

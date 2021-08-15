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
            Button btn = button.GetComponent<Button>();

            btn.onClick.AddListener(buttonClickEvent);
        }

        private void buttonClickEvent() {
            InputField inputField = iField.GetComponent<InputField>();
            string nameOfPlayer = DatabaseAPI.slugify(inputField.text);
            
            PlayerPrefs.SetString("playerName", inputField.text);
            PlayerPrefs.Save();
            
            PlayerPrefs.SetString("playerName_slug", nameOfPlayer);
            PlayerPrefs.Save();
            
            DatabaseAPI.setAsyncData("player/" + nameOfPlayer, false);
            SceneManager.LoadScene(nextScene);
        }
    }
}

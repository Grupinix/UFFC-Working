using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lobby {
    public class Lobby : MonoBehaviour {

        [SerializeField] private string waitingSceneName;
        [SerializeField] private string roomSceneName;
        
        [SerializeField] private Text textField;

        [SerializeField] private InputField inputField;

        [SerializeField] private GameObject panelReturn;

        [SerializeField] private Button enterButton;
        [SerializeField] private Button createButton;
        [SerializeField] private Button returnButton;

        private void Start() {
            Text text = textField.GetComponent<Text>();

            text.text = PlayerPrefs.GetString("playerName", "error");

            Button buttonEnter = enterButton.GetComponent<Button>();
            Button buttonCreate = createButton.GetComponent<Button>();
            Button buttonReturn = returnButton.GetComponent<Button>();

            buttonEnter.onClick.AddListener(enterClickEvent);
            buttonCreate.onClick.AddListener(createClickEvent);
            buttonReturn.onClick.AddListener(returnClickEvent);
        }

        private void enterClickEvent() {
            string nameOfRoom = DatabaseAPI.slugify(inputField.text);
            checkRoom(DatabaseAPI.getAsyncData("room/" + nameOfRoom), nameOfRoom, panelReturn, roomSceneName);
        }
        
        private static async void checkRoom(Task<DataSnapshot> data, string nameOfRoom, GameObject panelReturn, string roomScene) {
            await Task.WhenAll(data);

            if (!data.Result.Exists) {
                panelReturn.SetActive(true);
                return;
            }
            
            string playerName = PlayerPrefs.GetString("playerName_slug", "error");
            PlayerPrefs.SetString("room", nameOfRoom);
            DatabaseAPI.setAsyncData("room/" + nameOfRoom + "/" + "player_2", playerName);
            DatabaseAPI.setAsyncData("room/" + nameOfRoom, true);
            SceneManager.LoadScene(roomScene);
        }

        private void createClickEvent() {
            string nameOfRoom = DatabaseAPI.slugify(inputField.text);
            string playerName = PlayerPrefs.GetString("playerName_slug", "error");

            PlayerPrefs.SetString("room", nameOfRoom);
            DatabaseAPI.setAsyncData("room/" + nameOfRoom, false);
            DatabaseAPI.setAsyncData("room/" + nameOfRoom + "/" +  "player_1", playerName);
            SceneManager.LoadScene(waitingSceneName);
        }

        private void returnClickEvent() {
            panelReturn.SetActive(false);
        }
    }
}
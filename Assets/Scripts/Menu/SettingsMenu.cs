using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Button = UnityEngine.UI.Button;

namespace Menu {
    public class SettingsMenu : MonoBehaviour {

        [SerializeField] private string selectNameScene;

        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject menuPanel;

        [SerializeField] private Slider soundSlider;
    
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button backButton;

        private void Start() {
            Button buttonPlay = playButton.GetComponent<Button>();
            Button buttonSettings = settingsButton.GetComponent<Button>();
            Button buttonExit = exitButton.GetComponent<Button>();
            Button buttonBack = backButton.GetComponent<Button>();
            Slider sliderSound = soundSlider.GetComponent<Slider>();

            buttonPlay.onClick.AddListener(playEvent);
            buttonSettings.onClick.AddListener(settingsEvent);
            buttonExit.onClick.AddListener(exitEvent);
            buttonBack.onClick.AddListener(backEvent);
            sliderSound.onValueChanged.AddListener(soundEvent);
        }

        private void playEvent() {
            SceneManager.LoadScene(selectNameScene);
        }

        private void soundEvent(float value) {
            PlayerPrefs.SetFloat("gameSound", value);
        }

        private void settingsEvent() {
            menuPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        private void backEvent() {
            settingsPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
    
        private void exitEvent() {
            Application.Quit();
        }

    }
}

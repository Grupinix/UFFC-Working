using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Button = UnityEngine.UI.Button;

namespace Menu {
    public class SettingsMenu : MonoBehaviour {

        [SerializeField] private string authScene;

        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject menuPanel;

        [SerializeField] private Slider soundSlider;
    
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button backButton;

        private void Start() {
            soundSlider.value = PlayerPrefs.GetFloat("gameSound", soundSlider.value);

            playButton.onClick.AddListener(playEvent);
            settingsButton.onClick.AddListener(settingsEvent);
            exitButton.onClick.AddListener(exitEvent);
            backButton.onClick.AddListener(backEvent);
            soundSlider.onValueChanged.AddListener(soundEvent);
        }

        private void playEvent() {
            SceneManager.LoadScene(authScene);
        }

        private void soundEvent(float value) {
            PlayerPrefs.SetFloat("gameSound", value);
            PlayerPrefs.Save();
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

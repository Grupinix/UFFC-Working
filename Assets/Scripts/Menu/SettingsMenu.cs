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

        private Sound _sound;

        private void Start() {
            soundSlider.value = PlayerPrefs.GetFloat("gameSound", soundSlider.value);

            playButton.onClick.AddListener(playEvent);
            settingsButton.onClick.AddListener(settingsEvent);
            exitButton.onClick.AddListener(exitEvent);
            backButton.onClick.AddListener(backEvent);
            soundSlider.onValueChanged.AddListener(soundEvent);
            
            _sound = GameObject.FindGameObjectsWithTag("music")[0].GetComponent<Sound>();
            _sound.playMusic(0);
        }

        // Carrega uma nova cena

        private void playEvent() {
            SceneManager.LoadScene(authScene);
        }

        private void soundEvent(float value) {
            PlayerPrefs.SetFloat("gameSound", value);
            PlayerPrefs.Save();
            _sound.setVolume(value);
        }

        // Vai para as configurações

        private void settingsEvent() {
            menuPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        // Volta para o menu principal

        private void backEvent() {
            settingsPanel.SetActive(false);
            menuPanel.SetActive(true);
        }

        // Termina o aplicativo

        private void exitEvent() {
            Application.Quit();
        }

    }
}

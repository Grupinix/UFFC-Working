using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Button = UnityEngine.UI.Button;

namespace Menu {
    
    /**
     * Classe responsável pela cena "MainMenu"
     * e pelo controle da mesma
     */
    public class SettingsMenu : MonoBehaviour {

        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject menuPanel;

        [SerializeField] private Slider soundSlider;
    
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button backButton;

        private Sound _sound;

        private void Start() {
            soundSlider.value = PlayerPrefs.GetFloat("gameSound", soundSlider.value);

            settingsButton.onClick.AddListener(settingsEvent);
            exitButton.onClick.AddListener(exitEvent);
            backButton.onClick.AddListener(backEvent);
            soundSlider.onValueChanged.AddListener(soundEvent);
            
            _sound = GameObject.FindGameObjectsWithTag("music")[0].GetComponent<Sound>();
            _sound.playMusic(0);
        }

        /** Listener para a configuração de volume do jogo */
        private void soundEvent(float value) {
            PlayerPrefs.SetFloat("gameSound", value);
            PlayerPrefs.Save();
            _sound.setVolume(value);
        }

        /** Desativa o painel do "MainMenu" e habilita o painel de configurações */
        private void settingsEvent() {
            menuPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        /** Desativa o painel de configurações e habilita o painel do "MainMenu" */
        private void backEvent() {
            settingsPanel.SetActive(false);
            menuPanel.SetActive(true);
        }

        /** "ação" para sair do jogo */
        private void exitEvent() {
            Application.Quit();
        }

    }
}

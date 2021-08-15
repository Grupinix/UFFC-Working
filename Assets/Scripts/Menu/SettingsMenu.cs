using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Button = UnityEngine.UI.Button;

public class SettingsMenu : MonoBehaviour {

    public string selectNameScene;
    public string lobbyScene;

    public GameObject settingsPanel;
    public GameObject menuPanel;

    public Slider soundSlider;
    
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;
    public Button backButton;

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

using System.Text.RegularExpressions;
using System.Threading.Tasks;

using APIs;

using Firebase.Database;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class SelectName : MonoBehaviour {

    public string nextScene;
    
    public InputField inputField;
    public Button button;

    public GameObject panelChoose;
    public Button alreadyChoose;

    private void Start() {
        Button btn = button.GetComponent<Button>();
        Button btnChoose = alreadyChoose.GetComponent<Button>();

        btn.onClick.AddListener(buttonClickEvent);
        btnChoose.onClick.AddListener(buttonChooseEvent);
    }

    private void buttonChooseEvent() {
        panelChoose.SetActive(false);
    }
    
    private void buttonClickEvent() {
        string nameOfPlayer = slugify(inputField.text);
        checkName(DatabaseAPI.getAsyncData("player/" + nameOfPlayer), nameOfPlayer, panelChoose, nextScene);
    }
    
    private static string slugify(string str) {
        byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(str);
        str = System.Text.Encoding.ASCII.GetString(bytes);
        str = str.ToLower();

        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        str = Regex.Replace(str, @"\s+", " ").Trim();
        str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
        return Regex.Replace(str, @"\s", "-");
    }

    private static async void checkName(Task<DataSnapshot> data, string nameOfPlayer, GameObject panel, string scene) {
        await Task.WhenAll(data);

        if (data.Result.Exists) {
            panel.SetActive(true);
            return;
        }

        DatabaseAPI.setAsyncData("player/" + nameOfPlayer, false);
        PlayerPrefs.SetString("playerName", nameOfPlayer);
        SceneManager.LoadScene(scene);
    }
}

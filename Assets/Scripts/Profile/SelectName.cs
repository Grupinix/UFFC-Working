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


    private void Start() {
        Button btn = button.GetComponent<Button>();

        btn.onClick.AddListener(buttonClickEvent);
    }

    private void buttonClickEvent() {
        string nameOfPlayer = slugify(inputField.text);
        PlayerPrefs.SetString("playerName", inputField.text);
        checkName(DatabaseAPI.getAsyncData("player/" + nameOfPlayer), nameOfPlayer, nextScene);
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

    private static async void checkName(Task<DataSnapshot> data, string nameOfPlayer, string scene) {
        await Task.WhenAll(data);
        DatabaseAPI.setAsyncData("player/" + nameOfPlayer, false);
        SceneManager.LoadScene(scene);
    }
}

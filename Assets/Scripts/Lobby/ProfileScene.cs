using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIs;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lobby {
    public class ProfileScene : MonoBehaviour {

        [SerializeField] private Text nickname;
        [SerializeField] private Text email;
        [SerializeField] private Text register;
        [SerializeField] private Text partidas;
        [SerializeField] private Text vitorias;
        [SerializeField] private Text derrotas;
        [SerializeField] private List<Text> ranks;
        
        public void voltarProLobby() {
            SceneManager.LoadScene("Lobby");
        }

        private void Start() {
            nickname.text = $"Apelido : {DatabaseAPI.user.DisplayName}";
            email.text = $"E-mail : {DatabaseAPI.user.Email}";
            partidas.text = $"Partidas : {PlayerPrefs.GetInt("games", 0)}";
            vitorias.text = $"Vitórias : {PlayerPrefs.GetInt("playerWins", 0)}";
            derrotas.text = $"Derrotas : {PlayerPrefs.GetInt("playerLoses", 0)}";

            StartCoroutine(loadRegisterAndRank());
        }

        private IEnumerator loadRegisterAndRank() {
            Task<DataSnapshot> taskOne = DatabaseAPI.getDatabase().Child("users").Child(DatabaseAPI.user.UserId).Child("registerData").GetValueAsync();
            yield return new WaitUntil(() => taskOne.IsCompleted);
            
            register.text = $"Registrado em : {taskOne.Result.Value}";

            Task<DataSnapshot> taskTwo = DatabaseAPI.getDatabase().Child("users").OrderByChild("wins").LimitToLast(3).GetValueAsync();
            yield return new WaitUntil(() => taskTwo.IsCompleted);
            
            DataSnapshot snapshot = taskTwo.Result;

            int i = 0;
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse()) {
                ranks[i++].text = childSnapshot.Child("playerName").Value.ToString();
                if (i == 3) {
                    break;
                }
            }
        }
    }
}
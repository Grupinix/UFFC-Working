using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace APIs {
    
    public class LoginAuth : MonoBehaviour {

        [SerializeField] private InputField emailInputField;
        [SerializeField] private InputField passwordInputField;
        [SerializeField] private Text warningLoginText;
        [SerializeField] private Text confirmLoginText;
        
        [SerializeField] private string registerScene;
        [SerializeField] private string lobbyScene;

        public void loginButton() {
            StartCoroutine(startLogin(emailInputField.text, passwordInputField.text));
        }

        public void register() {
            SceneManager.LoadScene(registerScene);
        }

        private IEnumerator startLogin(string email, string password) {
            Task<FirebaseUser> loginTask = DatabaseAPI.getAuth().SignInWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => loginTask.IsCompleted);

            if (loginTask.Exception != null) {
                handleLoginErrors(loginTask.Exception);
            }
            else {
                loginUser(loginTask);
            }
        }

        private void loginUser(Task<FirebaseUser> loginTask) {
            DatabaseAPI.user = loginTask.Result;

            warningLoginText.text = "";
            confirmLoginText.text = "Logado";

            StartCoroutine(goToLoged());
        }

        private IEnumerator goToLoged() {
            yield return new WaitForSeconds(2);
            
            SceneManager.LoadScene(lobbyScene);
        }

        private void handleLoginErrors(System.AggregateException loginException) {
            FirebaseException firebaseException = loginException.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError) firebaseException.ErrorCode;

            warningLoginText.text = defineLoginErrorMessage(errorCode);
        }

        private string defineLoginErrorMessage(AuthError errorCode) {
            switch (errorCode) {
                case AuthError.MissingEmail:
                    return "Faltando email";
                case AuthError.InvalidEmail:
                    return "Email inválido";
                case AuthError.MissingPassword:
                    return "Faltando senha";
                case AuthError.UserNotFound:
                    return "Conta não existente";
                case AuthError.InvalidCredential:
                    return "Senha incorreta";
                default:
                    return "Error no firebase";
            }
        }
    }
}
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace APIs {
    
    public class RegisterAuth : MonoBehaviour {

        [SerializeField] private InputField usernameInputField;
        [SerializeField] private InputField emailInputField;
        [SerializeField] private InputField passwordInputField;
        [SerializeField] private InputField verifyPasswordInputField;
        [SerializeField] private Text warningRegisterText;

        [SerializeField] private string lobbySceneName;

        public void registerButton() {
            StartCoroutine(startRegister(emailInputField.text, passwordInputField.text, usernameInputField.text));
        }

        private IEnumerator startRegister(string email, string password, string username) {
            if (!checkRegistrationFieldsAndReturnForErrors()) {
                Task<FirebaseUser> registerTask = DatabaseAPI.getAuth().CreateUserWithEmailAndPasswordAsync(email, password);
                
                yield return new WaitUntil(() => registerTask.IsCompleted);
                if (registerTask.Exception != null) {
                    handleRegisterErrors(registerTask.Exception);
                }
                else {
                    StartCoroutine(registerUser(registerTask, username));
                }
            }
        }

        private bool checkRegistrationFieldsAndReturnForErrors() {
            if (usernameInputField.text == "") {
                warningRegisterText.text = "Insira um nome de usuário";
                return true;
            }
            if (passwordInputField.text != verifyPasswordInputField.text) {
                warningRegisterText.text = "Senha e verificar senha não batem";
                return true;
            }

            return false;
        }

        private void handleRegisterErrors(AggregateException registerException) {
            FirebaseException firebaseException = registerException.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError) firebaseException.ErrorCode;

            warningRegisterText.text = defineRegisterErrorMessage(errorCode);
        }

        private string defineRegisterErrorMessage(AuthError errorCode) {
            switch (errorCode) {
                case AuthError.MissingEmail:
                    return "Faltando email";
                case AuthError.InvalidEmail:
                    return "Email inválido";
                case AuthError.MissingPassword:
                    return "Faltando senha";
                case AuthError.WeakPassword:
                    return "Escolha uma senha mais forte";
                case AuthError.EmailAlreadyInUse:
                    return "Esse email já foi utilizado";
                default:
                    return "Error no firebase";
            }
        }

        private IEnumerator registerUser(Task<FirebaseUser> registerTask, string displayName) {
            DatabaseAPI.user = registerTask.Result;

            if (DatabaseAPI.user != null) {
                UserProfile profile = new UserProfile {DisplayName = displayName};
                Task task = DatabaseAPI.user.UpdateUserProfileAsync(profile);

                yield return new WaitUntil(() => task.IsCompleted);

                if (task.Exception != null) {
                    warningRegisterText.text = "Erro ao define nome";
                }
                else {
                    SceneManager.LoadScene(lobbySceneName);
                }
            }
        }
    }
}
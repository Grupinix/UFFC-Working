using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UserData;

namespace APIs {
    
    /**
     * Classe responsável pelo sistema
     * de registro
     */
    public class RegisterAuth : MonoBehaviour {

        [SerializeField] private InputField usernameInputField;
        [SerializeField] private InputField emailInputField;
        [SerializeField] private InputField passwordInputField;
        [SerializeField] private InputField verifyPasswordInputField;
        [SerializeField] private Text warningRegisterText;

        [SerializeField] private string lobbySceneName;

        /** "ação" para ser chamada pelo botão de "REGISTRAR" */
        public void registerButton() {
            StartCoroutine(startRegister(emailInputField.text, passwordInputField.text, usernameInputField.text));
        }

        /**
         * Inicia o cadastro do usuário de
         * maneira assincrona
         */
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

        /** Verifica se os campos foram preenchidos corretamente */
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

        /** Converte um erro genérico em um erro do Firebase */
        private void handleRegisterErrors(AggregateException registerException) {
            FirebaseException firebaseException = registerException.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError) firebaseException.ErrorCode;

            warningRegisterText.text = defineRegisterErrorMessage(errorCode);
        }

        /** Retorna o erro de forma amigável ao usuário */
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

        /** Finaliza o cadastro do usuário e atualiza o perfil do mesmo */
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
                    IDictionary<string, object> data = new Dictionary<string, object> {
                        {"playerName", displayName},
                        {"email", DatabaseAPI.user.Email},
                        {"lastDataSeen", DateTime.Now.ToString("dd/MM/yyyy")},
                        {"registerData", DateTime.Now.ToString("dd/MM/yyyy")},
                        {"wins", 0L}
                    };
                    ProfileManager.updateUserFields(data);
                    SceneManager.LoadScene(lobbySceneName);
                }
            }
        }
    }
}
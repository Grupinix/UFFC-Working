// <copyright file="SigninSampleScript.cs" company="Google Inc.">
// Copyright (C) 2017 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using UserData;
using UnityEngine.Events;



namespace APIs {

    public class SigninSampleScript : MonoBehaviour {

        public UnityEvent OnFirebaseInitialized = new UnityEvent();
        public UnityEvent OnFirebaseSilentLogin = new UnityEvent();
        public UnityEvent OnFirebaseLogin = new UnityEvent();
        public UnityEvent OnFirebaseUnLogin = new UnityEvent();


        [SerializeField] private string webClientId;
        [SerializeField] private string lobbyScene;
        [SerializeField] private string loginScene;
        [SerializeField] private bool checkDependencies = true;

        private GoogleSignInConfiguration configuration;

        private void Awake() {
            configuration = new GoogleSignInConfiguration {
                WebClientId = webClientId,
                RequestIdToken = true
            };

            if (checkDependencies) {
                CheckFirebaseDependencies();
            }
        }

        private void CheckFirebaseDependencies() {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                if (DependencyStatus.Available != task.Result) {
                    return;
                }

                DatabaseAPI.setAuth(FirebaseAuth.DefaultInstance);
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                OnFirebaseInitialized.Invoke();
            });
        }

        public void OnSignIn() {
            if (checkDependencies) {
                GoogleSignIn.Configuration = configuration;
                GoogleSignIn.Configuration.UseGameSignIn = false;
                GoogleSignIn.Configuration.RequestIdToken = true;
            }

            OnFirebaseLogin.Invoke();
            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
        }

        public void OnDisconnect() {
            GoogleSignIn.DefaultInstance.Disconnect();
        }

        internal void OnAuthenticationFinished(Task<GoogleSignInUser> task) {
            if (task.IsFaulted) {
                using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator()) {
                    SceneManager.LoadScene(loginScene);
                }
            } else if(task.IsCanceled) {
                SceneManager.LoadScene(loginScene);
            } else  {
                SignInWithGoogleOnFirebase(task.Result.IdToken);
            }
        }

        private void SignInWithGoogleOnFirebase(string idToken) {
            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

            DatabaseAPI.getAuth().SignInWithCredentialAsync(credential).ContinueWith(task => {
                AggregateException ex = task.Exception;
                if (ex != null) {
                    OnFirebaseUnLogin.Invoke();
                }
                else {
                    StartCoroutine(loginUser(task));
                }
            });
        }

        private IEnumerator loginUser(Task<FirebaseUser> loginTask) {
            DatabaseAPI.user = loginTask.Result;
            IDictionary<string, object> data = new Dictionary<string, object> {
                {"lastDataSeen", DateTime.Now.ToString("dd/MM/yyyy")},
                {"email", DatabaseAPI.user.Email},
                {"playerName", DatabaseAPI.user.DisplayName}
            };
            ProfileManager.updateUserFields(data);
            yield return new WaitForSeconds(1);

            SceneManager.LoadScene(lobbyScene);
        }

        public void OnSignInSilently() {
            if (checkDependencies) {
                GoogleSignIn.Configuration = configuration;
                GoogleSignIn.Configuration.UseGameSignIn = false;
                GoogleSignIn.Configuration.RequestIdToken = true;
            }

            OnFirebaseSilentLogin.Invoke();
            GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
        }
    }
}
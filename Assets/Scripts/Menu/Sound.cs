using System;
using System.Collections;
using UnityEngine;

namespace Menu {
    public class Sound : MonoBehaviour {

        [SerializeField] private AudioClip[] musics;

        private AudioSource _audioSource;
        private float _volume;
        private int _oldMusic = 9999;
        
        private void Awake() {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("music");

            if (objs.Length > 1) {
                Destroy(gameObject);
            }

            _audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            _volume = PlayerPrefs.GetFloat("gameSound", 0.6f);
        }

        public void setVolume(float value) {
            _volume = value;
            _audioSource.volume = value;
        }

        public void playMusic(int music) {
            if (_oldMusic == music) {
                return;
            }
            StartCoroutine(fadeOut(_audioSource, 1, music));
            _oldMusic = music;
        }

        private IEnumerator fadeOut(AudioSource audioSource, float duration, int music) {
            float startVolume = audioSource.volume;
 
            while (audioSource.volume > 0) {
                audioSource.volume -= startVolume * Time.deltaTime / duration;
                yield return new WaitForEndOfFrame();
            }
 
            audioSource.Stop();
            audioSource.clip = musics[music];
            StartCoroutine(fadeIn(audioSource, duration));
        }
        
        private IEnumerator fadeIn(AudioSource audioSource, float duration) {
            audioSource.volume = 0;
            audioSource.Play();
 
            while (audioSource.volume < _volume) {
                audioSource.volume += _volume * Time.deltaTime / duration;
                yield return new WaitForEndOfFrame();
            }
 
            audioSource.volume = _volume;
        }
    }
}
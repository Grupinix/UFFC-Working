using System.Collections.Generic;
using System.Threading.Tasks;
using APIs;
using UnityEngine;

namespace UserData {
    public static class ProfileManager {
        public static async void updateUserFields(IDictionary<string, object> data) {
            foreach (KeyValuePair<string, object> entry in data) {
                if (entry.Key != "wins") {
                    PlayerPrefs.SetString(entry.Key, entry.Value.ToString());
                }
                else {
                    PlayerPrefs.SetInt(entry.Key, int.Parse(entry.Value.ToString()));
                }
            }
            PlayerPrefs.Save();

            Task task = DatabaseAPI.getDatabase().Child("users").Child(DatabaseAPI.user.UserId).UpdateChildrenAsync(data);
            await Task.WhenAll(task);
        }
    }
}
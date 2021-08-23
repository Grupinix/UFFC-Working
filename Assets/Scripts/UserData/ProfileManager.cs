using System.Collections.Generic;
using System.Threading.Tasks;
using APIs;
using UnityEngine;

namespace UserData {
    public static class ProfileManager {
        public static async void updateUserFields(Dictionary<string, string> data) {
            List<Task> tasks = new List<Task>();
            foreach (KeyValuePair<string, string> entry in data) {
                Task task = DatabaseAPI.getDatabase().Child("users").Child(DatabaseAPI.user.UserId).Child(entry.Key).SetValueAsync(entry.Value);
                PlayerPrefs.SetString(entry.Key, entry.Value);
                tasks.Add(task);
            }

            
            await Task.WhenAll(tasks.ToArray());
            PlayerPrefs.Save();
        }
    }
}
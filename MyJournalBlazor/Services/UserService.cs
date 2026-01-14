using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Storage;

namespace MyJournalBlazor.Services
{
    public class UserService
    {
        // We use Preferences for non-secret info
        public bool IsRegistered => Preferences.ContainsKey("UserName");
        public string UserName => Preferences.Get("UserName", "User");

        // REGISTER NEW USER
        public async Task RegisterUserAsync(string name, string phone, string pin, string secQ, string secA)
        {
            // 1. Save Public Info
            Preferences.Set("UserName", name);
            Preferences.Set("UserPhone", phone);
            Preferences.Set("SecurityQuestion", secQ);

            // 2. Save Secrets Securely
            // We store the Answer and PIN in the device's secure keychain
            await SecureStorage.SetAsync("UserPin", pin);
            await SecureStorage.SetAsync("SecurityAnswer", secA.ToLower().Trim());
        }

        // LOGIN CHECK
        public async Task<bool> VerifyPinAsync(string inputPin)
        {
            var storedPin = await SecureStorage.GetAsync("UserPin");
            return storedPin == inputPin;
        }

        // FORGOT PIN CHECK
        public string GetSecurityQuestion()
        {
            return Preferences.Get("SecurityQuestion", "No Question Set");
        }

        public async Task<bool> VerifySecurityAnswerAsync(string inputAnswer)
        {
            var storedAnswer = await SecureStorage.GetAsync("SecurityAnswer");
            return storedAnswer == inputAnswer.ToLower().Trim();
        }

        public async Task ResetPinAsync(string newPin)
        {
            await SecureStorage.SetAsync("UserPin", newPin);
        }
    }
}

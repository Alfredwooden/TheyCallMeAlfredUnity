using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace TheyCallMeAlfredUnity.Editor.ProjectSetup {
    public static class PackageManager {
        private static AddRequest request;
        private static readonly Queue<string> packagesToInstall = new();

        public static void InstallPackages(string[] packages) {
            foreach (string package in packages) {
                packagesToInstall.Enqueue(package);
            }

            if (packagesToInstall.Count > 0) {
                StartNextPackageInstallation();
            }
        }

        private static async void StartNextPackageInstallation() {
            request = Client.Add(packagesToInstall.Dequeue());

            while (!request.IsCompleted) {
                await Task.Delay(10);
            }

            if (request.Status == StatusCode.Success) {
                Debug.Log("Installed: " + request.Result.packageId);
            }
            else if (request.Status >= StatusCode.Failure) {
                Debug.LogError(request.Error.message);
            }

            if (packagesToInstall.Count > 0) {
                await Task.Delay(1000);
                StartNextPackageInstallation();
            }
        }
    }
}

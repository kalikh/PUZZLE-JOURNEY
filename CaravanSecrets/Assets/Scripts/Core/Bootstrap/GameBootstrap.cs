using System.Collections;
using CaravanSecrets.Core.Services;
using CaravanSecrets.Data.Localization;
using CaravanSecrets.Data.Save;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace CaravanSecrets.Core.Bootstrap
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        public ServiceRegistry Services { get; private set; }
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Services = new ServiceRegistry();
            Services.Register<ISaveService>(new JsonFileSaveService());
            Services.Register<ILocalizationService>(new RuntimeLocalizationService());
        }

        private IEnumerator Start()
        {
            yield return LocalizationSettings.InitializationOperation;
            if (SceneManager.GetActiveScene().name == "Bootstrap") SceneManager.LoadScene("Gameplay");
        }
    }
}

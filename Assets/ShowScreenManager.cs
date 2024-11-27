using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowScreenManager : MonoBehaviour
{
    private static AndroidJavaObject webView;
    private static AndroidJavaObject currentActivity;
    private static CustomWebViewClientProxy webViewClientProxy;
    private bool hasError = false;

    public void OpenWebView(string url)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                    if (currentActivity != null)
                    {
                        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                        {
                            webView = new AndroidJavaObject("android.webkit.WebView", currentActivity);
                            if (webView != null)
                            {
                                AndroidJavaObject webSettings = webView.Call<AndroidJavaObject>("getSettings");
                                if (webSettings != null)
                                {
                                    webSettings.Call("setJavaScriptEnabled", true);
                                    webSettings.Call("setMixedContentMode", 0);
                                    webSettings.Call("setDomStorageEnabled", true);
                                    webSettings.Call("setDatabaseEnabled", true);
                                    webSettings.Call("setMinimumFontSize", 1);
                                    webSettings.Call("setMinimumLogicalFontSize", 1);
                                    webSettings.Call("setSupportZoom", false);
                                    webSettings.Call("setAllowFileAccess", true);
                                    webSettings.Call("setAllowContentAccess", true);
                                }
                                else
                                {
                                    Debug.LogError("Не удалось получить настройки WebView.");
                                    return;
                                }

                                webViewClientProxy = new CustomWebViewClientProxy(this);
                                AndroidJavaObject customWebViewClient = new AndroidJavaObject("com.unity3d.player.CustomWebViewClient", webViewClientProxy);
                                webView.Call("setWebViewClient", customWebViewClient);

                                webView.Call("loadUrl", url);
                            }
                            else
                            {
                                Debug.LogError("Не удалось создать WebView.");
                            }
                        }));
                    }
                    else
                    {
                        Debug.LogError("Не удалось получить текущую активность.");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Не удалось открыть WebView: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("WebView поддерживается только на платформе Android.");
            ShowPageScreen.Instance.LoadGame();
        }
    }

    private void AddWebViewToActivity()
    {
        if (webView != null && currentActivity != null && !hasError)
        {
            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                using (AndroidJavaObject decorView = currentActivity.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView"))
                {
                    AndroidJavaObject windowInsets = decorView.Call<AndroidJavaObject>("getRootWindowInsets");
                    if (windowInsets != null)
                    {
                        int leftInset = windowInsets.Call<int>("getStableInsetLeft");
                        int topInset = windowInsets.Call<int>("getStableInsetTop");
                        int rightInset = windowInsets.Call<int>("getStableInsetRight");
                        int bottomInset = windowInsets.Call<int>("getStableInsetBottom");

                        using (AndroidJavaObject layoutParams = new AndroidJavaObject("android.widget.FrameLayout$LayoutParams", -1, -1))
                        {
                            layoutParams.Call("setMargins", leftInset, topInset, rightInset, bottomInset);

                            using (AndroidJavaClass r = new AndroidJavaClass("android.R$id"))
                            {
                                int contentViewId = r.GetStatic<int>("content");
                                AndroidJavaObject contentView = currentActivity.Call<AndroidJavaObject>("findViewById", contentViewId);
                                if (contentView != null)
                                {
                                    contentView.Call("addView", webView, layoutParams);
                                }
                                else
                                {
                                    Debug.LogError("Не удалось найти contentView.");
                                }
                            }
                        }
                    }
                }
            }));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentActivity != null)
            {
                currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    if (webView != null && webView.Call<bool>("canGoBack"))
                    {
                        webView.Call("goBack");
                    }
                }));
            }
        }
    }

    void OnDestroy()
    {
        if (webView != null)
        {
            if (currentActivity != null)
            {
                currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    webView.Call("destroy");
                }));
            }
        }
    }

    public void SetError(bool error)
    {
        hasError = error;
        if (!hasError)
        {
            AddWebViewToActivity();
        }
        else
        {
            RemoveWebView();
            SceneManager.LoadScene("menu");
        }
    }

    private void RemoveWebView()
    {
        if (webView != null && currentActivity != null)
        {
            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                using (AndroidJavaClass r = new AndroidJavaClass("android.R$id"))
                {
                    int contentViewId = r.GetStatic<int>("content");
                    AndroidJavaObject contentView = currentActivity.Call<AndroidJavaObject>("findViewById", contentViewId);
                    if (contentView != null)
                    {
                        contentView.Call("removeView", webView);
                    }
                }
            }));
        }
    }

    public class CustomWebViewClientProxy : AndroidJavaProxy
    {
        private ShowScreenManager webViewManager;

        public CustomWebViewClientProxy(ShowScreenManager manager) : base("com.unity3d.player.IWebViewClient")
        {
            webViewManager = manager;
        }

        public void onPageStarted(string url)
        {
            Debug.Log("onPageStarted: " + url);
        }

        public void onPageFinished(string url)
        {
            Debug.Log("onPageFinished: " + url);
            if (!webViewManager.hasError)
            {
                webViewManager.SetError(false);
            }
        }

        public void onReceivedError(int errorCode, string description, string failingUrl)
        {
            Debug.Log($"onReceivedError: {description}, URL: {failingUrl}");
        }

        public void onReceivedHttpError(string url, int statusCode, string description)
        {
            Debug.Log($"onReceivedHttpError: {statusCode} - {description}");
        }

        public bool shouldOverrideUrlLoading(string url)
        {
            Debug.Log($"Redirect detected: {url}");
            return HandleUrlRedirect(url, true); // true по умолчанию для совместимости
        }

        public bool shouldOverrideUrlLoading(string url, bool isForMainFrame)
        {
            Debug.Log($"Redirect detected for {(isForMainFrame ? "main frame" : "subframe")}: {url}");
            return HandleUrlRedirect(url, isForMainFrame);
        }

        private bool HandleUrlRedirect(string url, bool isForMainFrame)
        {
            // Пример: обработка редиректа
            if (isForMainFrame && url.Contains("specific_redirect_url"))
            {
                Debug.Log("Redirecting to menu scene...");
                SceneManager.LoadScene("menu");
                return true;
            }

            // Разрешаем продолжить загрузку
            return false;
        }
    }
}

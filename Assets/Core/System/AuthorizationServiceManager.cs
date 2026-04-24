using System;
using System.Collections;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AuthorizationServiceManager : MonoBehaviour
{
    [field:SerializeField]
    private string BASE_URL = "http://localhost:7000";
    static public GameplayData currentData;
    static public AuthorizationServiceManager source;
    static public string displayNameRaw {get; private set;}
    static public TelegramUserData TGUserNameData {get; private set;}
    static public bool IsLoaded {get; private set;}
    static private Action OnLoadedActions;
    static private Action<string> OnErrorActions;
    static private string TelegramInitData;
    void Start()
    {
        #region singleton
        if (source != null)
        {
            Destroy(gameObject);
            return;
        }
        source = this;
        DontDestroyOnLoad(gameObject);
        #endregion

        try
        {
            StartCoroutine(AuthEnumerator());
            displayNameRaw = GetUserNameRaw();
        }
        catch (System.Exception e)
        {
            OnTelegramError(e.Message);
        }
    }

    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string InitTelegram();
    [DllImport("__Internal")]
    private static extern string GetTGUser();
    [DllImport("__Internal")]
    private static extern void SetupTelegram();
    [DllImport("__Internal")]
    private static extern void ExpandTelegram();
    #endif
    public bool TryConnectOnErrorActon(Action<string> action)
    {
        try
        {
            if (OnErrorActions != null) OnErrorActions += action;
            OnErrorActions??=action;
            return true;            
        }
        catch (System.Exception e)
        {
            OnTelegramError(e.Message);
            return false;
        }
    }
    public bool TryConnectOnActivationActon(Action action)
    {
        try
        {
            if (IsLoaded)
            {
                action.Invoke();
            } else
            {
                if (OnLoadedActions != null) OnLoadedActions += action;
                OnLoadedActions??=action;
            }
            return true;            
        }
        catch (System.Exception e)
        {
            OnTelegramError(e.Message);
            return false;
        }
    }
    public bool TrySave()
    {
        if (!IsLoaded || currentData == null)
        {
            return false;
        }
        //StartCoroutine(SaveScoreEnumerator());
        return true;
    }
    private void OnTelegramError(string error)
    {
        Debug.LogWarning(error);
        OnErrorActions?.Invoke(error);
    }
    public string Init()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            SetupTelegram();
            ExpandTelegram();
            return InitTelegram();
        #else
            return "test_init_data";
        #endif
    }
    public string GetUserNameRaw()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            return GetTGUser();
        #else
            return "{\"id\":123,\"first_name\":\"test\"}";
        #endif        
    }
    IEnumerator AuthEnumerator()
    {
        TelegramInitData = Init();

        if (string.IsNullOrEmpty(TelegramInitData))
        {
            OnTelegramError("FAILED TO SETUP.");
            yield break;
        }

        TelegramAuthRequestData requestData = new()
        {
            InitData = TelegramInitData
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] BodyRaw = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest reqest = new(BASE_URL, "POST");

        reqest.uploadHandler = new UploadHandlerRaw(BodyRaw);
        reqest.downloadHandler = new DownloadHandlerBuffer();
        reqest.SetRequestHeader("Content-Type", "application/json");

        yield return reqest.SendWebRequest();

        bool IsError = reqest.result != UnityWebRequest.Result.Success;
        if (IsError)
        {
            Debug.Log("Auth request error: " + reqest.error);
            Debug.Log("Response: " + reqest.downloadHandler.text);
            OnTelegramError("FAILED TO SETUP. ERRORED.");
            yield break;
        }

        string responseJson = reqest.downloadHandler.text;

        WebRequestData<GameplayData> response = JsonUtility.FromJson<WebRequestData<GameplayData>>(responseJson);

        if (response == null)
        {
            Debug.Log("Auth request error: " + "Response is null.");
            OnTelegramError("FAILED TO SETUP. RESPONSE NULL.");
            yield break;
        }

        if (!response.success || response.body == null)
        {
            Debug.Log("Auth request error: " + "Response not successful.");
            OnTelegramError("FAILED TO SETUP. MESSAGE: " + response.message);
            yield break;
        }

        TGUserNameData = new()
        {
            first_name = response.body.Pname
        };

        OnLoadedActions?.Invoke();
        IsLoaded = true;
    }
    /*
    IEnumerator SaveScoreEnumerator()
    {
        string url = $"{BASE_URL}/savescore";

        TelegramSaveScoreRequest requestData = new()
        {
            initData = TelegramInitData,
            score = currentData.score,
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type","application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                OnTelegramError("Error: "+request.error);
                yield break;
            }
            Debug.Log("Saved!");
        }

    }*/
}

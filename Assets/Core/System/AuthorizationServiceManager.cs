using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class AuthorizationServiceManager : MonoBehaviour
{
    [field:SerializeField]
    private string BASE_URL = "http://localhost:7000";
    [field: SerializeField]
    private string AUTHORIZE_SUFFIX = "/auth/telegram";
    [field: SerializeField]
    private string SAVE_SUFFIX = "/save-score";
    static public List<GameplayData> Leaderboard;
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
#if UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(AuthEnumerator());
            StartCoroutine(GetLBEnumerator`());
#endif
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
        try
        {
            StartCoroutine(SaveEnumerator());
        } catch (Exception e)
        {
            if (e.Message != null)
            {
                OnTelegramError(e.Message);
                return false;
            }
        }
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
    IEnumerator SaveEnumerator()
    {
        if (string.IsNullOrEmpty(TelegramInitData))
        {
            OnTelegramError("FAILED TO SAVE.");
            yield break;
        }

        TelegramSaveScoreRequest saveScoreRequest = new TelegramSaveScoreRequest()
        {
            InitData = TelegramInitData,
            score = currentData.score
        };

        string json = JsonUtility.ToJson(saveScoreRequest);
        byte[] BodyRaw = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest reqest = new(BASE_URL + SAVE_SUFFIX, "POST");

        reqest.uploadHandler = new UploadHandlerRaw(BodyRaw);
        reqest.downloadHandler = new DownloadHandlerBuffer();
        reqest.SetRequestHeader("Content-Type", "application/json");

        yield return reqest.SendWebRequest();

        bool IsError = reqest.result != UnityWebRequest.Result.Success;
        if (IsError)
        {
            Debug.Log("Auth request error: " + reqest.error);
            Debug.Log("Response: " + reqest.downloadHandler.text);
            OnTelegramError("FAILED TO SAVE. ERRORED.");
            yield break;
        }

        string responseJson = reqest.downloadHandler.text;

        WebRequestData<GameplayData> response = JsonUtility.FromJson<WebRequestData<GameplayData>>(responseJson);

        if (response == null)
        {
            Debug.Log("Auth request error: " + "Response is null.");
            OnTelegramError("FAILED TO SAVE. RESPONSE NULL.");
            yield break;
        }

        if (!response.success || response.body == null)
        {
            Debug.Log("Auth request error: " + "Response not successful.");
            OnTelegramError("FAILED TO SAVE. MESSAGE: " + response.message);
            yield break;
        }
    }
    IEnumerator GetLBEnumerator()
    {

        using UnityWebRequest reqest = new(BASE_URL + "/get-leaderboard", "POST");

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

        WebRequestData<List<GameplayData>> response = JsonUtility.FromJson<WebRequestData<List<GameplayData>>>(responseJson);

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

        Leaderboard = response.body;
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

        using UnityWebRequest reqest = new(BASE_URL+AUTHORIZE_SUFFIX, "POST");

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
        currentData = response.body;
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

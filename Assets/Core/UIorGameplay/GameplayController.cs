using TMPro;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField,Min(0.1f)]
    private float SaveInterval = 4f;
    [field:SerializeField]
    private AuthBehaviour authBehaviour;
    [field:SerializeField]
    private float ClickIncrement = 1;
    [field:SerializeField]
    private TMP_Text scoreLabel;
    [SerializeField]
    private bool NeedToLogin = false;
    private bool Initialized = false;
    private bool Busy = false;
    private float RemainingSavingTime;
    void Initialize()
    {
        Busy = true;
        RemainingSavingTime = SaveInterval;
        Initialized = true;
        Busy = false;
        UpdateLabel();
    }
    void Update()
    {
        if (!Busy&&!Initialized && (!NeedToLogin || authBehaviour.LoggedIn))
        {
            Initialize();
        }
        
        if (!Initialized) return;
        
        if (RemainingSavingTime <= 0 && !Busy)
        {
            SaveData();
            RemainingSavingTime = SaveInterval;
        } else
        {
            RemainingSavingTime -= Time.deltaTime;
        }
    }
    void OnApplicationQuit()
    {
        SaveData();
    }
    public void OnClick()
    {
        if (!Initialized) return;
        AuthorizationServiceManager.currentData.score += ClickIncrement;
        UpdateLabel();
    }
    private void UpdateLabel()
    {
        if (scoreLabel != null)
        {
            scoreLabel.text = "Score: "+ AuthorizationServiceManager.currentData.score.ToString();
        }
    }
    private void SaveData()
    {
        Busy = true;
        try
        {
           //AuthorizationServiceManager.Instance.SaveCounterData(data);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to safe.\nError message:\n"+e.Message);
        }
        Busy = false;
    }
}

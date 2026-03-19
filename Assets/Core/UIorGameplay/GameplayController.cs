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
    private GameplayData data;
    private bool Busy = false;
    private float RemainingSavingTime;
    void Initialize()
    {
        Busy = true;
        data = new();
        RemainingSavingTime = SaveInterval;
        Initialized = true;
        Busy = false;
        UpdateLabel();
        Debug.Log("Readdy!");
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
        data.Score += ClickIncrement;
        UpdateLabel();
    }
    private void UpdateLabel()
    {
        if (scoreLabel != null)
        {
            scoreLabel.text = "Score: "+data.Score.ToString();
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

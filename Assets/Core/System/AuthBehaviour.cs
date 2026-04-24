using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthBehaviour : MonoBehaviour
{
    /*
    [SerializeField]
    private TMP_InputField DisplayNameInputField;
    [SerializeField]
    private TMP_InputField MailInputField;
    [SerializeField]
    private TMP_InputField PasswordInputField;
    [SerializeField]
    private Button ProceedButton;
    [SerializeField]
    private Button ToggleRegisterButton;
    [SerializeField]
    private TMP_Text RegisterButtonText;
    [SerializeField]
    private TMP_Text ProceedButtonText;
    [SerializeField]
    private GameObject RegistrationFrame;*/
    [SerializeField]
    private TMP_Text AuthorizedText;
    [SerializeField]
    private Transform AuthorizedFrame;
    [SerializeField]
    private UIController UIController;
    [SerializeField]
    private int _TargetUIID = 1;
    [SerializeField]
    private float ShowingAuthorizedFrameDuration = 1;
    [SerializeField]
    private float GameFrameActivationDelay = 0.1f;
    [SerializeField]
    private DelayedConnectionHelper delayedConnectionHelper;
    private bool registerVisible;
    internal bool LoggedIn = false;
    //private AuthorizationServiceManager authorization;
    void Start()
    {
        #if UNITY_EDITOR
            OnLoggedIn();
        #else
            AuthorizationServiceManager.source.TryConnectOnActivationActon(OnLoggedIn);
        #endif
        //authorization = AuthorizationServiceManager.Instance;
        //ProceedButton.onClick.AddListener(OnButtonPress);
        //ToggleRegisterButton.onClick.AddListener(OnRegisterPress);
    }
    /*
    void OnRegisterPress()
    {
        registerVisible = !registerVisible;
        RegistrationFrame.SetActive(registerVisible);
        if (registerVisible)
        {
            RegisterButtonText.text = "Log-in";
        } else
        {
            RegisterButtonText.text = "Register";
        }
    }*/
    /*
    private void OnButtonPress() // async does not work, nor ienumerator
    {
        bool valid = false;
        if (!registerVisible)
        {
            (bool, string) Result = (true, "Debug");  //await authorization.SignInAsync(MailInputField.text,PasswordInputField.text);
            if (Result.Item1)
            {
                valid = true;
                ActivateAuthedFrame("Welcome back, " + Result.Item2);
            }
        } else
        {
            if (IsMailValid(MailInputField.text) && IsPassValid(PasswordInputField.text))
            {
                bool suceed = true;// await authorization.CreateOrSigninAndWriteAsync(MailInputField.text, PasswordInputField.text, DisplayNameInputField.text);
                if (suceed)
                {
                    valid = true;
                    ActivateAuthedFrame("Welcome, " + DisplayNameInputField.text);
                    valid = true;
                }                
            }
        }
        if (valid)
        {
            if (delayedConnectionHelper != null)
            {
                delayedConnectionHelper.ConnectDelayedAction(OnLoggedInAction,ShowingAuthorizedFrameDuration+GameFrameActivationDelay);
            }        
        }
    }
    */
    private void OnLoggedIn()
    {
        string DizName = AuthorizationServiceManager.TGUserNameData != null ? AuthorizationServiceManager.TGUserNameData.first_name : "Unknown";
        LoggedIn = true;    
        ActivateAuthedFrame("Welcome back, " + DizName);        
    }
    private void OnLoggedInAction()
    {
        UIController.TargetUIID = _TargetUIID;        
    }
    private void HideAuthedFrameAction()
    {
        AuthorizedFrame.gameObject.SetActive(false);        
    }
    private void ActivateAuthedFrame(string _text)
    {
        AuthorizedFrame.gameObject.SetActive(true);
        AuthorizedText.text = _text;
        StartCoroutine(ShowEnumerator());
    }
    IEnumerator ShowEnumerator()
    {
        yield return new WaitForSeconds(ShowingAuthorizedFrameDuration);
        HideAuthedFrameAction();
        yield return new WaitForSeconds(GameFrameActivationDelay);
        OnLoggedInAction();
    }
}

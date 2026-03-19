using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthBehaviour : MonoBehaviour
{
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
    private TMP_Text AuthorizedText;
    [SerializeField]
    private Transform AuthorizedFrame;
    [SerializeField]
    private GameObject RegistrationFrame;
    [SerializeField]
    private UIController UIController;
    [SerializeField]
    private int _TargetUIID = 1;
    private bool registerVisible;
    internal bool LoggedIn = false;
    //private AuthorizationServiceManager authorization;
    void Start()
    {
        //authorization = AuthorizationServiceManager.Instance;
        ProceedButton.onClick.AddListener(() =>
        {
            OnButtonPress();
        });
        ToggleRegisterButton.onClick.AddListener(OnRegisterPress);
    }
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
    }
    private async void OnButtonPress()
    {
        if (!registerVisible)
        {
            (bool, string) Result = (true, "Debug");  //await authorization.SignInAsync(MailInputField.text,PasswordInputField.text);
            if (Result.Item1)
            {
                await ActivateAuthedFrame("Welcome back, "+Result.Item2);
                LoggedIn = true; 
                await Task.Delay(100);
                UIController.TargetUIID = _TargetUIID;
            }
        } else
        {
            if (IsMailValid(MailInputField.text) && IsPassValid(PasswordInputField.text))
            {
                bool suceed = true;// await authorization.CreateOrSigninAndWriteAsync(MailInputField.text, PasswordInputField.text, DisplayNameInputField.text);
                if (suceed)
                {
                    await ActivateAuthedFrame("Welcome, "+DisplayNameInputField.text);
                    LoggedIn = true; 
                    await Task.Delay(100);
                    UIController.TargetUIID = _TargetUIID;
                }                
            }
        }
    }
    private async Task ActivateAuthedFrame(string _text)
    {
        AuthorizedFrame.gameObject.SetActive(true);
        AuthorizedText.text = _text;  
        await Task.Delay(1000);  
        AuthorizedFrame.gameObject.SetActive(false);
    }
    private bool IsMailValid(string mail) => !string.IsNullOrEmpty(mail) && mail.Contains('@') && mail.Contains('.') && mail.Length >= 5;
    private bool IsPassValid(string pass) => pass.Length >= 6;
}

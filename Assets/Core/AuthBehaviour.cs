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
    private bool registerVisible;
    private bool Loggined = false;
    private AuthorizationServiceManager authorization;
    void Start()
    {
        authorization = AuthorizationServiceManager.Instance;
        ProceedButton.onClick.AddListener(() =>
        {
            Task.Run(OnButtonPress);
        });
        ToggleRegisterButton.onClick.AddListener(OnRegisterPress);
    }
    void Update()
    {
        if (Loggined && !AuthorizedFrame.gameObject.activeInHierarchy)
        {
            Debug.Log("Activating!");
            AuthorizedFrame.gameObject.SetActive(true);
        }
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
            (bool,string) Result = await authorization.SignInAsync(MailInputField.text,PasswordInputField.text);
            if (Result.Item1)
            {
                ActivateAuthedFrame("Welcome back, "+Result.Item2); 
            }
        } else
        {
            if (IsMailValid(MailInputField.text) && IsPassValid(PasswordInputField.text))
            {
                bool suceed = await authorization.CreateOrSigninAndWriteAsync(MailInputField.text, PasswordInputField.text, DisplayNameInputField.text);
                if (suceed)
                {
                    ActivateAuthedFrame("Welcome, "+DisplayNameInputField.text);
                }                
            }
            
        }
    }
    private void ActivateAuthedFrame(string _text)
    {
        Loggined = true;
        AuthorizedText.text = _text;    
    }
    private bool IsMailValid(string mail) => !string.IsNullOrEmpty(mail) && mail.Contains('@') && mail.Contains('.');
    private bool IsPassValid(string pass) => pass.Length >= 6;
}

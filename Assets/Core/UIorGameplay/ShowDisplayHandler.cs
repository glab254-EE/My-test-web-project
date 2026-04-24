using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ShowDisplayHandler : MonoBehaviour
{
    [SerializeField]
    private string Format = "{0}";
    private TMP_Text text;
    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.text = "";    
    }
    void OnEnable()
    {
        if (text.text == "")
        {
            text.text = string.Format(Format,AuthorizationServiceManager.TGUserNameData.first_name);
        }
    }
}

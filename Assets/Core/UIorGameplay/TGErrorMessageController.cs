using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TGErrorMessageController : MonoBehaviour
{
    [SerializeField]
    private float ShowDuration;
    private TMP_Text text;
    private Coroutine coroutine;
    void Start()
    {
        text = GetComponent<TMP_Text>();   
        text.text = "";     
        AuthorizationServiceManager.source.TryConnectOnErrorActon(OnError);
    }
    void OnError(string message)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(ErrorEnumerator(message));
    }
    IEnumerator ErrorEnumerator(string message)
    {
        text.text = message;
        yield return new WaitForSecondsRealtime(ShowDuration);
        text.text = "";
    }
}

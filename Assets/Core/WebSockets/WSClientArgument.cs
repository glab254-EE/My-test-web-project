using UnityEngine;

[System.Serializable]
public class WSClientArgument
{
    public enum WSClientAction
    {
        AwaitMessage,
        SendMessge,
    }
    [field:SerializeField]
    public WSClientAction ClientActionEnum { get; private set; }
    [field:SerializeField]
    public string ResponseString { get; private set; }
}
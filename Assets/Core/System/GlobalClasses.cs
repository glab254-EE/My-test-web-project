using System;

[Serializable]
public class TelegramAuthRequestData
{
    public string InitData;
}
[Serializable]
public class TelegramSaveScoreRequest
{
    public string InitData;
    public double score;
}
[Serializable]
public class GameplayData
{
    public long teleID;
    public string Pname;
    public double score;
}
[Serializable]
public class WebRequestData<T>
{
    #nullable enable
    public bool success;
    public string? message;
    public T? body;
}
[Serializable]
public class TelegramUserData
{
    public string first_name = "";
    public double id;
}
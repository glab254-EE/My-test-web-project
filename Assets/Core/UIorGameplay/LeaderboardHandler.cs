using System.Collections.Generic;
using UnityEngine;

public class LeaderboardHandler : MonoBehaviour
{
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private GameObject template;
    void OnEnable()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        List<GameplayData> entries = AuthorizationServiceManager.Leaderboard;
        for (int i = 0; i < entries.Count; i++)
        {
            GameplayData data = entries[i];
            GameObject cloned = Instantiate(template, parent);
            if (cloned.TryGetComponent(out TMPro.TMP_Text textLabel))
            {
                textLabel.text = $"N{i + 1} - {data.Pname} : {data.score}";
            }
        }
    }
}

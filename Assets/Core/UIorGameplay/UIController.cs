using System;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Serializable]
    struct UIStateOption
    {
        [field:SerializeField]
        internal List<GameObject> objects;
    }
    [field:SerializeField]
    private List<UIStateOption> UIStateToObjects = new();
    [field:SerializeField]
    internal int TargetUIID = 0;
    private int _shownUIID = 0;
    void Update()
    {
        if (TargetUIID != _shownUIID)
        {
            UpdateUI();
        }
    }
    void UpdateUI()
    {
        TargetUIID = Mathf.Clamp(TargetUIID,0,UIStateToObjects.Count);
        for (int i = 0; i < UIStateToObjects.Count; i++)
        {
            List<GameObject> gameObjects = UIStateToObjects[i].objects;
            foreach(GameObject gameObject in gameObjects)
            {
                gameObject.SetActive(i==TargetUIID);
            }
        }
        _shownUIID = TargetUIID;
    }
}

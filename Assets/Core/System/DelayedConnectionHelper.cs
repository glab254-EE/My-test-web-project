using System;
using System.Collections.Generic;
using UnityEngine;

public class DelayedConnectionHelper : MonoBehaviour
{
    [SerializeField]
    private double TickCooldown = 0.05;
    [SerializeField]
    private double MaximumTickSize = 60; // for testing kept at 60, use higher numbers.
    private List<KeyValuePair<double,Action>> _Connections = new();
    private double _Ticks = 0;
    private float _counter = 0;
    void Update()
    {
        if (_Connections.Count > 0)
        {
            _counter += Time.deltaTime;
            if (_counter >= TickCooldown)
            {
                _counter = 0;
                if (_Ticks >= MaximumTickSize)
                {
                    _Ticks = 0;
                }
                _Ticks += TickCooldown;
                HandleOnTickChecks();
            }            
        }
    }
    private void HandleOnTickChecks()
    {
        List<int> _indexesToRemove = new();
        foreach(KeyValuePair<double,Action> keyValuePair in _Connections)
        {
            double checkedTime = keyValuePair.Key;
            if (checkedTime > MaximumTickSize)
            {
                checkedTime -= MaximumTickSize;
            }
            if (checkedTime <= _Ticks)
            {
                keyValuePair.Value();
                _indexesToRemove.Add(_Connections.IndexOf(keyValuePair));
            }
        }
        if (_indexesToRemove.Count > 0)
        {
            foreach(int index in _indexesToRemove)
            {
                _Connections.RemoveAt(index);
            }
        }
    }
    public bool ConnectDelayedAction(Action act,double duration)
    {
        try
        {
            _Connections.Add(new(_Ticks+duration,act));
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}

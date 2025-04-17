using System;
using R3;
using UnityEngine;

public class Presenter : IDisposable
{
    private readonly CompositeDisposable _disposable = new();

    protected void AddDisposable(IDisposable item)
    {
        _disposable.Add(item);
    }

    void IDisposable.Dispose()
    {
        _disposable.Dispose();
        Debug.Log($"Dispose");
    }
}
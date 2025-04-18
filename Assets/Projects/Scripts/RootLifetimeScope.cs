using UnityEngine;
using VContainer;
using VContainer.Unity;

public class RootLifetimeScope : LifetimeScope
{
    [SerializeField] GameData _gameData;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_gameData);
    }
}

using VContainer;
using VContainer.Unity;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope
{
    private static GameLifetimeScope instance = null;
    public static IObjectResolver ObjectResolver => instance.Container;
    protected override void Configure(IContainerBuilder builder)
    {

    }
    protected override void Awake()
    {
        base.Awake();

        instance = this;
    } 
}

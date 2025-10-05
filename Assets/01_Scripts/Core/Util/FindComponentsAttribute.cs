using System;

[AttributeUsage(AttributeTargets.Field)]
public class FindComponentsAttribute : Attribute
{
    public string[] _gameObjectNames { get; }

    public FindComponentsAttribute(params string[] gameObjectNames)
    {
        _gameObjectNames = gameObjectNames;
    }
}
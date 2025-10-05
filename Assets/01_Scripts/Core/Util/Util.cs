using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Collections;

public class Util : MonoBehaviour
{
    public static Transform FindTransformByName(string name)
    {
        Transform tr = GameObject.Find(name).transform;

        if (tr == null) Debug.LogError("잘못된 Attribute Parameter : " + name);

        return tr;
    }
    public static void InjectComponents(object o)
    {
        Type type = o.GetType();
        MonoBehaviour script = o as MonoBehaviour;

        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            var attribute = (FindComponentsAttribute)field.GetCustomAttribute(typeof(FindComponentsAttribute));

            if (attribute == null) continue;

            if (field.FieldType.IsArray)
                InjectArrayField(field, attribute, script);
            
            else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                InjectListField(field, attribute, script);
            
            else
                InjectSingleComponent(field, attribute, script);
        }
    }
    private static void InjectArrayField(FieldInfo field, FindComponentsAttribute attribute, MonoBehaviour script)
    {
        Type elementType = field.FieldType.GetElementType();
        List<Component> componentsList = GetComponentsFromGameObjects(attribute, elementType);

        Array componentArray = Array.CreateInstance(elementType, componentsList.Count);

        for (int i = 0; i < componentsList.Count; i++)
        
            componentArray.SetValue(componentsList[i], i);
        
        field.SetValue(script, componentArray);
    }
    private static void InjectListField(FieldInfo field, FindComponentsAttribute attribute, MonoBehaviour script)
    {
        Type elementType = field.FieldType.GetGenericArguments()[0];
        IList componentsList = (IList)Activator.CreateInstance(field.FieldType);

        foreach (var component in GetComponentsFromGameObjects(attribute, elementType))
        
            componentsList.Add(component);

        field.SetValue(script, componentsList);
    }
    private static void InjectSingleComponent(FieldInfo field, FindComponentsAttribute attribute, MonoBehaviour script)
    {
        Transform tr = FindTransformByName(attribute._gameObjectNames[0]);

        if (tr == null) return;

        Component component = tr.GetComponent(field.FieldType);

        if (component == null) return;

        field.SetValue(script, component);
    }

    private static List<Component> GetComponentsFromGameObjects(FindComponentsAttribute attribute, Type componentType)
    {
        List<Component> componentsList = new List<Component>();

        foreach (string gameObjectName in attribute._gameObjectNames)
        {
            Transform tr = FindTransformByName(gameObjectName);

            if (tr == null) continue;

            Component component = tr.GetComponent(componentType);

            if (component == null) continue;

            componentsList.Add(component);
        }

        return componentsList;
    }
}

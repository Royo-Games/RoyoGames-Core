using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class CustomNameAttribute : PropertyAttribute
{
    public CustomNameAttribute() { }
}
public interface ICustomName
{
    string CustomName { get; }
}
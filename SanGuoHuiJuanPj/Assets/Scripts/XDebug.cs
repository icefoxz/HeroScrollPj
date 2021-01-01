using System;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Unity调试帮助类
/// </summary>
public static class XDebug
{
    public static void Log<T>(string message,[CallerMemberName] string methodName = null,T type = default) where T : class
    {
#if DEBUG
        Debug.Log($"{type?.GetType().Name}:{methodName}() {message}");
#endif
    }
    public static void LogError<T>(string message,[CallerMemberName] string methodName = null,T type = default) where T : class
    {
#if DEBUG
        Debug.LogError($"{type?.GetType().Name}:{methodName}() {message}");
#endif
    }
    public static void Log(Type type,string message,[CallerMemberName] string methodName = null)
    {
#if DEBUG
        Debug.Log($"{type.Name}:{methodName}() {message}");
#endif
    }

}
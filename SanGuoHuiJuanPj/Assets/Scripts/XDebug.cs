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
#if UNITY_EDITOR
        Debug.Log($"{type?.GetType().Name}:{methodName}() {message}");
#endif
    }
    public static void LogError<T>(string message,[CallerMemberName] string methodName = null,T type = default) where T : class
    {
#if UNITY_EDITOR
        Debug.LogError($"{type?.GetType().Name}:{methodName}() {message}");
#endif
    }
    public static void LogError(string message,string typeName,[CallerMemberName] string methodName = null) 
    {
#if UNITY_EDITOR
        Debug.LogError($"{typeName}:{methodName}() {message}");
#endif
    }
    public static void Log(Type type,string message,[CallerMemberName] string methodName = null)
    {
#if UNITY_EDITOR
        Debug.Log($"{type.Name}:{methodName}() {message}");
#endif
    }

    public static XDebugException Throw<T>(string message, [CallerMemberName] string methodName = null,
        T type = default) where T : class => Throw(message, type?.GetType().Name, methodName);
    public static XDebugException Throw(string message,string typeName,[CallerMemberName] string methodName = null)
    {
        return new XDebugException(message, methodName, typeName);
    }

    public class XDebugException : Exception
    {
        public XDebugException(string message = null,string methodName = null,string typeName = null): base($"{typeName}.{methodName}():{message}")
        {
        }
    }

}
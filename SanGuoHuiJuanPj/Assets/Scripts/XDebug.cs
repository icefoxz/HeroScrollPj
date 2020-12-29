using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Unity调试帮助类
/// </summary>
public static class XDebug
{
    public static void Log<T>(string message,[CallerMemberName] string methodName = null,T type = default)
    {
#if DEBUG
        Debug.Log($"{type.GetType().Name}:{methodName}() {message}");
#endif
    }

}
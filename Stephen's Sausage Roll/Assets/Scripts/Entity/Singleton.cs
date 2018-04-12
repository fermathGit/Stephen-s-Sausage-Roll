using System;
using System.Reflection;
using SType = System.Type;

public class Singleton<T> where T : class
{
    static T instance;

    public static T Instance {
        get {
            if (null == instance)
            {
                SType type = typeof(T);
                if (type.IsAbstract || type.IsInterface)
                {
                    throw (new Exception("Class type must could be instantiated. Don't use abstract or interface"));
                }
                if (!type.IsSealed)
                {
                    throw (new Exception("Class type must be a sealed. Use sealed"));
                }
                ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                                                    null,
                                                                    SType.EmptyTypes,
                                                                    null);
                if (null == constructor)
                {
                    throw (new Exception("Constructor must empty"));
                }
                if (!constructor.IsPrivate)
                {
                    throw (new Exception("Constructor must be a private function"));
                }
                instance = constructor.Invoke(null) as T;
            }
            return instance;
        }
    }
}
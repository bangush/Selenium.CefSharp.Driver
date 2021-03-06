﻿using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Selenium.CefSharp.Driver.InTarget
{
    public class ReflectionAccessor
    {
        static Dictionary<string, Type> _fullNameAndType = new Dictionary<string, Type>();

        public object Object { get; }

        public ReflectionAccessor(object obj) => Object = obj;

        public T GetProperty<T>(string name)
            => (T)Object.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(Object, new object[0]);

        public T GetField<T>(string name)
        {
            var type = Object.GetType();
            while (type != null)
            {
                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    return (T)field.GetValue(Object);
                }
                type = type.BaseType;
            }
            throw new NotSupportedException();
        }

        public T InvokeMethod<T>(string name, params object[] args)
             => (T)Object.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(Object, args);

        public T InvokeMethodByType<T>(string name, params object[] args)
             => (T)Object.GetType().GetMethod(name, args.Select(e => e.GetType()).ToArray()).Invoke(Object, args);

        public static Type GetType(string typeFullName)
        {
            lock (_fullNameAndType)
            {
                if (_fullNameAndType.TryGetValue(typeFullName, out var type)) return type;

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var assemblyTypes = new List<Type>();
                foreach (Assembly assembly in assemblies)
                {
                    type = assembly.GetType(typeFullName);
                    if (type != null) break;
                }
                if (type != null)
                {
                    _fullNameAndType.Add(typeFullName, type);
                }
                return type;
            }
        }
    }
}

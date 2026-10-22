using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Concurrent;

namespace Qahr.Mobile
{
    public class CachedAttributeInvocation<TAction, TAttribute, TKey>
        where TAttribute : Attribute
        where TAction : class
        where TKey : struct
    {

        private Dictionary<TKey, Tuple<TAttribute, TAction>> map;
        public CachedAttributeInvocation(Func<TAttribute, TKey> translator)
        {
            try
            {
                var assembly = Assembly.GetCallingAssembly();
                map = new Dictionary<TKey, Tuple<TAttribute, TAction>>();
                foreach (var types in assembly.GetTypes())
                {
                    try
                    {
                        var methods = types.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var method in methods)
                        {
                            foreach (var attribute in method.GetCustomAttributes(true))
                            {
                                try
                                {
                                    var testAttr = attribute as TAttribute;
                                    if (testAttr != null)
                                    {
                                        var key = translator(testAttr);
                                        var invoker = Delegate.CreateDelegate(typeof(TAction), method) as TAction;
                                        if (!map.ContainsKey(key))
                                        {
                                            map.Add(key, new Tuple<TAttribute, TAction>(testAttr, invoker));
                                        }
                                        else
                                        {
                                            MyConsole.WriteLine("[Error] Duplicated Invocation: " + key, ConsoleColor.DarkRed);
                                        }
                                    }
                                }
                                catch (Exception e) { Console.WriteLine(e.ToString()); }
                            }
                        }
                    }
                    catch (Exception e) { MyConsole.WriteException(e); }
                }
            }
            catch (ReflectionTypeLoadException e) { MyConsole.WriteException(e); }
        }
        public bool TryGetInvoker(TKey key, out Tuple<TAttribute, TAction> folded)
        {
            return map.TryGetValue(key, out folded);
        }
        public bool TryGetInvoker(TKey key, out TAction action)
        {
            Tuple<TAttribute, TAction> folded;
            if (this.TryGetInvoker(key, out folded))
            {
                action = folded.Item2;
                return true;
            }
            action = null;
            return false;
        }
    }
}

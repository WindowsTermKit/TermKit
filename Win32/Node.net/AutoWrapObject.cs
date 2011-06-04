using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using IronJS.Native;

namespace IronJS
{
    /// <summary>
    /// This class uses reflection to automatically wrap subclasses of itself.
    /// </summary>
    public class AutoWrapObject : CommonObject
    {
        private delegate BoxedValue AutoBoxedDelegate(BoxedValue[] parameters);
        private delegate BoxedValue AutoSetterDelegate(BoxedValue value);
        private delegate BoxedValue AutoGetterDelegate();
        private delegate BoxedValue AutoEventDelegate(string name, FunctionObject handler);

        protected AutoWrapObject(Environment env) : base(env, env.Maps.Base, env.Prototypes.Object)
        {
            // Handle functions.
            this.WrapFunctions();

            // Handle properties.
            this.WrapProperties();

            // Handle events.
            this.WrapEvents();
        }

        #region Function Wrapping

        /// <summary>
        /// Handle wrapping the functions.
        /// </summary>
        private void WrapFunctions()
        {
            // Generate a list of JavaScript functions and sort them into
            // unique and non-unique functions.
            List<FunctionInfo> functions = this.GenerateFunctions(this.Env, this.GetType());
            FunctionListSorted sorted = this.SortFunctions(functions);

            // Automatically bind the unique functions.
            foreach (FunctionInfo fi in sorted.UniqueFunctions)
            {
                this.Put(fi.Name, fi.Function);
            }

            // Generate BoxedValue[] delegates for handling variable parameters.
            this.GenerateBoxedValueFunctions(sorted);
        }

        /// <summary>
        /// Generates a list of JavaScript functions for the public functions
        /// declared in the specified type.
        /// </summary>
        /// <param name="t">The type to generate JavaScript functions for.</param>
        /// <returns>A list of wrapped functions.</returns>
        private List<FunctionInfo> GenerateFunctions(Environment env, Type t)
        {
            List<FunctionInfo> functions = new List<FunctionInfo>();
            foreach (MethodInfo m in t.GetMethods())
            {
                if (m.DeclaringType != t)
                    continue;

                functions.Add(this.GenerateFunction(m));
            }
            return functions;
        }

        /// <summary>
        /// Generates a FunctionInfo based on the specified method.
        /// </summary>
        /// <param name="m">The method information.</param>
        /// <returns>A FunctionInfo with delegate, parameter, return type and name properties.</returns>
        private FunctionInfo GenerateFunction(MethodInfo m)
        {
            // Create a dynamic delegate type definition.
            List<Type> ts = m.GetParameters().Select(p => p.ParameterType).ToList();
            Type dT = null;
            if (m.ReturnType != typeof(void))
            {
                ts.Add(m.ReturnType);
                dT = Expression.GetFuncType(ts.ToArray());
            }
            else
                dT = Expression.GetActionType(ts.ToArray());

            // Create the required delegate.
            Delegate d = Delegate.CreateDelegate(dT, this, m);

            // Create the function object and function info instances.
            FunctionObject fo = typeof(Utils)
                                    .GetMethod("createHostFunction")
                                    .MakeGenericMethod(dT)
                                    .Invoke(null, new object[] { this.Env, d })
                                    as FunctionObject;
            return new FunctionInfo(m.Name, fo, m.ReturnType, m.GetParameters().Select(p => p.ParameterType).ToArray());
        }

        /// <summary>
        /// Go through a list of functions and sort them into unique functions (for which
        /// there is a single definitions) and non-unique functions (for which there are
        /// multiple definitions with varying arguments or return types).
        /// </summary>
        /// <param name="functions">The list of functions to sort.</param>
        /// <returns>The sorted function lists.</returns>
        private FunctionListSorted SortFunctions(List<FunctionInfo> functions)
        {
            FunctionListSorted result = new FunctionListSorted();
            foreach (FunctionInfo fi in functions)
            {
                if (result.UniqueNames.Contains(fi.Name))
                {
                    // There was previously one instance of this function, but
                    // we found a second one, so we need to move the old one into
                    // nonUniqueFunctions and the name into nonUniqueNames.
                    result.UniqueNames.Remove(fi.Name);
                    result.NonUniqueNames.Add(fi.Name);
                    FunctionInfo fii = result.UniqueFunctions.Where(f => (f.Name == fi.Name)).First();
                    result.UniqueFunctions.Remove(fii);
                    result.NonUniqueFunctions.Add(fii);
                    result.NonUniqueFunctions.Add(fi);
                }
                else if (result.NonUniqueNames.Contains(fi.Name))
                {
                    // We already know this is a non-unique function.
                    result.NonUniqueFunctions.Add(fi);
                }
                else
                {
                    // Assume it's a unique function.
                    result.UniqueNames.Add(fi.Name);
                    result.UniqueFunctions.Add(fi);
                }
            }
            return result;
        }

        /// <summary>
        /// Generates a series of BoxedValue[] handlers for overloaded functions.
        /// </summary>
        /// <param name="sorted">The sorted function list.</param>
        private void GenerateBoxedValueFunctions(FunctionListSorted sorted)
        {
            foreach (string n in sorted.NonUniqueNames)
            {
                List<FunctionInfo> variants = sorted.NonUniqueFunctions.Where(f => (f.Name == n)).ToList();
                AutoBoxedDelegate handler = (parameters) =>
                {
                    foreach (FunctionInfo v in variants)
                    {
                        // Check to see if the parameter count is the same.
                        if (v.ParameterTypes.Length != parameters.Length)
                            continue;

                        // Check to see if the types match.
                        int i;
                        for (i = 0; i < parameters.Length; i += 1)
                        {
                            try
                            {
                                object o = typeof(BoxedValue)
                                            .GetMethod("Unbox")
                                            .MakeGenericMethod(v.ParameterTypes[i])
                                            .Invoke(parameters[i], null);
                            }
                            catch (InvalidCastException)
                            {
                                // We couldn't cast it.
                                break;
                            }
                            catch (TargetInvocationException)
                            {
                                // We couldn't cast it.
                                break;
                            }
                        }
                        if (i != parameters.Length) // We didn't validate all parameters.
                            continue;

                        // At this point we pass all the requirements, so invoke
                        // the function.
                        return v.Function.Call(this, parameters);
                    }

                    // There was no function to handle this.
                    throw new NullReferenceException();
                };
                this.Put(n, Utils.createHostFunction<AutoBoxedDelegate>(this.Env, handler));
            }
        }

        private class FunctionInfo
        {
            public string Name = null;
            public FunctionObject Function = null;
            public Type ReturnType = null;
            public Type[] ParameterTypes = null;

            public FunctionInfo(string name, FunctionObject func, Type ret, Type[] param)
            {
                this.Name = name;
                this.Function = func;
                this.ReturnType = ret;
                this.ParameterTypes = param;
            }
        }

        private class FunctionListSorted
        {
            public List<string> UniqueNames = new List<string>();
            public List<string> NonUniqueNames = new List<string>();
            public List<FunctionInfo> UniqueFunctions = new List<FunctionInfo>();
            public List<FunctionInfo> NonUniqueFunctions = new List<FunctionInfo>();
        }

        #endregion

        #region Property Wrapping

        /// <summary>
        /// Handle wrapping the properties.
        /// </summary>
        private void WrapProperties()
        {
            foreach (PropertyInfo p in this.GetType().GetProperties())
            {
                MethodInfo get = p.GetGetMethod();
                MethodInfo set = p.GetSetMethod();

                /*this.Get("__defineGetter__").Func.Call(this, p.Name,
                    this.GenerateFunction(get).Function);
                this.Get("__defineSetter__").Func.Call(this, p.Name,
                    this.GenerateFunction(set).Function);*/
            }
        }

        #endregion

        #region Event Wrapping

        /// <summary>
        /// Handle wrapping the events.
        /// </summary>
        private void WrapEvents()
        {
            // Create a series of mappings to match event names with the event information.
            Dictionary<string, EventInfo> mappings = new Dictionary<string, EventInfo>();
            foreach (EventInfo e in this.GetType().GetEvents())
            {
                mappings.Add(e.Name.ToLower().Substring(2), e);
            }

            // Create the on(name, func) function.
            AutoEventDelegate on = (string name, FunctionObject func) =>
                {
                    // Attach the event.
                    try
                    {
                        EventInfo e = mappings[name];
                        Type[] t = e.EventHandlerType.GetGenericArguments();
                        EventHandler<AutoWrapEventArgs> backend = (sender, raw) =>
                        {
                            AutoWrapEventArgs args = raw as AutoWrapEventArgs;
                            func.Call(this, args.GetParameters());
                        };
                        Delegate handler = (Delegate)typeof(EventHandler<>).MakeGenericType(t[0]).GetConstructors()[0].Invoke(new object[] { backend.Target, backend.Method.MethodHandle.GetFunctionPointer() });
                        e.AddEventHandler(this, handler);
                    }
                    catch (KeyNotFoundException)
                    {
                        // TODO: Return error to Node.js somehow...
                    }
                    catch (Exception e)
                    {
                        return IronJS.Undefined.Boxed;
                    }

                    // The object might want to listen for attachments.
                    MethodInfo ma = this.GetType().GetMethod("OnAttach", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (ma != null)
                        ma.Invoke(this, new object[] { name });

                    return IronJS.Undefined.Boxed;
                };
            this.Put("on", Utils.createHostFunction<AutoEventDelegate>(this.Env, on));
        }

        #endregion

        public abstract class AutoWrapEventArgs : EventArgs
        {
            public abstract BoxedValue[] GetParameters();
        }
    }
}

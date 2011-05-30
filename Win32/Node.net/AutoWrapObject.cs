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
        protected AutoWrapObject(Environment env) : base(env, env.Maps.Base, env.Prototypes.Object)
        {
            // Generate a list of JavaScript functions and sort them into
            // unique and non-unique functions.
            List<FunctionInfo> functions = this.GenerateFunctions(env, this.GetType());
            FunctionListSorted sorted = this.SortFunctions(functions);

            // Automatically bind the unique functions.
            foreach (FunctionInfo fi in sorted.UniqueFunctions)
            {
                this.Put(fi.Name, fi.Function);
            }

            // Generate BoxedValue[] delegates for handling variable parameters.
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
                this.Put(n, Utils.createHostFunction<AutoBoxedDelegate>(env, handler));
            }
        }

        private delegate BoxedValue AutoBoxedDelegate(BoxedValue[] parameters);

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

                FunctionObject fo = typeof(Utils)
                                        .GetMethod("createHostFunction")
                                        .MakeGenericMethod(dT)
                                        .Invoke(null, new object[] { env, d })
                                        as FunctionObject;
                functions.Add(new FunctionInfo(m.Name, fo, m.ReturnType, m.GetParameters().Select(p => p.ParameterType).ToArray()));
            }
            return functions;
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

        public abstract class AutoWrapEventArgs : EventArgs
        {
            public abstract BoxedValue[] GetParameters();
        }
    }
}

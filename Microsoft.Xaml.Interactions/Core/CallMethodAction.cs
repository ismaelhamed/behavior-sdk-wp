using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Xaml.Interactivity;

namespace Microsoft.Xaml.Interactions.Core
{
    /// <summary>
    /// Represents an action that calls a method on a specified object when invoked.
    /// </summary>
    public class CallMethodAction : DependencyObject, IAction
    {
        private Type targetObjectType;
        private List<MethodDescriptor> methodDescriptors = new List<MethodDescriptor>();

        public readonly static DependencyProperty MethodNameProperty;
        public readonly static DependencyProperty TargetObjectProperty;

        /// <summary>
        /// Gets or sets the name of the method to invoke. This is a dependency property.
        /// </summary>
        public string MethodName
        {
            get { return (string)GetValue(CallMethodAction.MethodNameProperty); }
            set { SetValue(CallMethodAction.MethodNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the object that exposes the method of interest. This is a dependency property.
        /// </summary>
        public object TargetObject
        {
            get { return GetValue(CallMethodAction.TargetObjectProperty); }
            set { SetValue(CallMethodAction.TargetObjectProperty, value); }
        }

        static CallMethodAction()
        {
            CallMethodAction.MethodNameProperty = DependencyProperty.Register("MethodName", typeof(String), typeof(CallMethodAction), new PropertyMetadata(null, CallMethodAction.OnMethodNameChanged));
            CallMethodAction.TargetObjectProperty = DependencyProperty.Register("TargetObject", typeof(Object), typeof(CallMethodAction), new PropertyMetadata(null, CallMethodAction.OnTargetObjectChanged));
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that is passed to the action by the behavior. Generally this is AssociatedObject or the target object.</param>
        /// <param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <returns>true if updating the property value succeeds; otherwise, false.</returns>
        public object Execute(object sender, object parameter)
        {
            var obj = (ReadLocalValue(CallMethodAction.TargetObjectProperty) == DependencyProperty.UnsetValue ? sender : TargetObject);
            if (obj == null || string.IsNullOrEmpty(MethodName))
            {
                return false;
            }

            UpdateTargetType(obj.GetType());

            var methodDescriptor = FindBestMethod(parameter);
            if (methodDescriptor == null)
            {
                if (TargetObject != null)
                {
                    var currentCulture = CultureInfo.CurrentCulture;
                    var callMethodActionValidMethodNotFoundExceptionMessage = ExceptionStringTable.CallMethodActionValidMethodNotFoundExceptionMessage;
                    var methodName = new object[] { MethodName, targetObjectType };
                    throw new ArgumentException(String.Format(currentCulture, callMethodActionValidMethodNotFoundExceptionMessage, methodName));
                }
                return false;
            }

            var parameters = methodDescriptor.Parameters;
            if (parameters.Length == 0)
            {
                methodDescriptor.MethodInfo.Invoke(obj, null);
                return true;
            }

            if (parameters.Length != 2)
            {
                return false;
            }

            var methodInfo = methodDescriptor.MethodInfo;
            var objArray = new[] { obj, parameter };
            methodInfo.Invoke(obj, objArray);
            return true;
        }

        private MethodDescriptor FindBestMethod(object parameter)
        {
            return methodDescriptors.FirstOrDefault(methodDescriptor =>
            {
                if (!methodDescriptor.HasParameters)
                {
                    return true;
                }
                return parameter != null && methodDescriptor.SecondParameterType.IsInstanceOfType(parameter);
            });
        }

        private static void OnMethodNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((CallMethodAction)sender).UpdateMethodDescriptors();
        }

        private static void OnTargetObjectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var type = args.NewValue != null ? args.NewValue.GetType() : null;

            var callMethodAction = (CallMethodAction)sender;
            callMethodAction.UpdateTargetType(type);
        }

        private void UpdateTargetType(Type newTargetType)
        {
            if (newTargetType == targetObjectType)
            {
                return;
            }
            targetObjectType = newTargetType;
            UpdateMethodDescriptors();
        }

        private void UpdateMethodDescriptors()
        {
            methodDescriptors.Clear();

            if (string.IsNullOrEmpty(MethodName) || targetObjectType == null)
            {
                return;
            }
            var methods = targetObjectType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                if (!string.Equals(method.Name, MethodName, StringComparison.Ordinal) || method.ReturnType != typeof(void))
                {
                    continue;
                }

                var parameters = method.GetParameters();
                if (CallMethodAction.AreMethodParamsValid(parameters))
                {
                    methodDescriptors.Add(new CallMethodAction.MethodDescriptor(method, parameters));
                }
            }

            methodDescriptors = methodDescriptors.OrderByDescending(methodDescriptor =>
            {
                var num = 0;
                if (methodDescriptor.HasParameters)
                {
                    for (var i = methodDescriptor.SecondParameterType; i != typeof(EventArgs); i = i.BaseType)
                    {
                        num++;
                    }
                }
                return methodDescriptor.ParameterCount + num;
            }).ToList();
        }

        private static bool AreMethodParamsValid(IList<ParameterInfo> methodParams)
        {
            if (methodParams.Count == 2)
            {
                if (methodParams[0].ParameterType != typeof(object))
                {
                    return false;
                }
                if (!typeof(EventArgs).IsAssignableFrom(methodParams[1].ParameterType))
                {
                    return false;
                }
            }
            else if (methodParams.Count != 0)
            {
                return false;
            }
            return true;
        }

        [DebuggerDisplay("{MethodInfo}")]
        private class MethodDescriptor
        {
            public bool HasParameters
            {
                get { return Parameters.Length > 0; }
            }

            public MethodInfo MethodInfo
            {
                get;
                private set;
            }

            public int ParameterCount
            {
                get { return Parameters.Length; }
            }

            public ParameterInfo[] Parameters
            {
                get;
                private set;
            }

            public Type SecondParameterType
            {
                get { return ParameterCount < 2 ? null : Parameters[1].ParameterType; }
            }

            public MethodDescriptor(MethodInfo methodInfo, ParameterInfo[] methodParameters)
            {
                MethodInfo = methodInfo;
                Parameters = methodParameters;
            }
        }
    }
}

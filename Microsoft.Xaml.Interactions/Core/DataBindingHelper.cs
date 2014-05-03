using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.Xaml.Interactivity;

namespace Microsoft.Xaml.Interactions.Core
{
    internal static class DataBindingHelper
    {
        private readonly static Dictionary<Type, List<DependencyProperty>> DependenciesPropertyCache;

        static DataBindingHelper()
        {
            DataBindingHelper.DependenciesPropertyCache = new Dictionary<Type, List<DependencyProperty>>();
        }

        private static IEnumerable<DependencyProperty> GetDependencyProperties(Type type)
        {
            List<DependencyProperty> dependencyProperties;
            if (DataBindingHelper.DependenciesPropertyCache.TryGetValue(type, out dependencyProperties))
            {
                return dependencyProperties;
            }

            dependencyProperties = new List<DependencyProperty>();
            while (type != null && type != typeof(DependencyObject))
            {
                dependencyProperties.AddRange((from runtimeField in type.GetFields() 
                                               where runtimeField.IsPublic && runtimeField.FieldType == typeof(DependencyProperty) 
                                               select runtimeField.GetValue(null)).OfType<DependencyProperty>());
                type = type.BaseType;
            }

            if (type != null)
            {
                DataBindingHelper.DependenciesPropertyCache[type] = dependencyProperties;
            }
            return dependencyProperties;
        }

        private static void RefreshBinding(DependencyObject target, DependencyProperty property)
        {
            var bindingExpression = target.ReadLocalValue(property) as BindingExpression;
            if (bindingExpression != null && bindingExpression.ParentBinding != null)
            {
                BindingOperations.SetBinding(target, property, bindingExpression.ParentBinding);
            }
        }

        public static void RefreshDataBindingsOnActions(ActionCollection actions)
        {
            foreach (var action in actions)
            {
                foreach (var dependencyProperty in DataBindingHelper.GetDependencyProperties(action.GetType()))
                {
                    DataBindingHelper.RefreshBinding(action, dependencyProperty);
                }
            }
        }
    }
}

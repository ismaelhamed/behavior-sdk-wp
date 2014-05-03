using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using Microsoft.Xaml.Interactivity;

namespace Microsoft.Xaml.Interactions.Core
{
    /// <summary>
    /// Represents an action that will change a specified property to a specified value when invoked.
    /// </summary>
    public class ChangePropertyAction : DependencyObject, IAction
    {
        public readonly static DependencyProperty PropertyNameProperty;
        public readonly static DependencyProperty TargetObjectProperty;
        public readonly static DependencyProperty ValueProperty;

        /// <summary>
        /// Gets or sets the name of the property to change. This is a dependency property.
        /// </summary>
        public string PropertyName
        {
            get { return (string)GetValue(ChangePropertyAction.PropertyNameProperty); }
            set { SetValue(ChangePropertyAction.PropertyNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the object whose property will be changed. If TargetObject is not set or cannot be resolved, 
        /// the sender of Execute will be used. This is a dependency property.
        /// </summary>
        public object TargetObject
        {
            get { return GetValue(ChangePropertyAction.TargetObjectProperty); }
            set { SetValue(ChangePropertyAction.TargetObjectProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value to set. This is a dependency property.
        /// </summary>
        public object Value
        {
            get { return GetValue(ChangePropertyAction.ValueProperty); }
            set { SetValue(ChangePropertyAction.ValueProperty, value); }
        }

        static ChangePropertyAction()
        {
            ChangePropertyAction.PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(ChangePropertyAction), new PropertyMetadata(null));
            ChangePropertyAction.TargetObjectProperty = DependencyProperty.Register("TargetObject", typeof(object), typeof(ChangePropertyAction), new PropertyMetadata(null));
            ChangePropertyAction.ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(ChangePropertyAction), new PropertyMetadata(null));
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that is passed to the action by the behavior. Generally this is AssociatedObject or the target object.</param>
        /// <param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <returns>true if updating the property value succeeds; otherwise, false.</returns>
        public object Execute(object sender, object parameter)
        {
            var obj = (ReadLocalValue(ChangePropertyAction.TargetObjectProperty) == DependencyProperty.UnsetValue ? sender : TargetObject);
            if (obj == null || string.IsNullOrEmpty(PropertyName))
            {
                return false;
            }
            UpdatePropertyValue(obj);
            return true;
        }

        private void UpdatePropertyValue(object targetObject)
        {
            var type = targetObject.GetType();
            var property = type.GetProperty(PropertyName);
            ValidateProperty(type.Name, property);
            Exception exception = null;
            try
            {
                object value;
                var propertyType = property.PropertyType;

                if (Value == null)
                {
                    value = (propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null);
                }
                else if (!propertyType.IsInstanceOfType(Value))
                {
                    var str = Value.ToString();
                    value = (propertyType.IsEnum ? Enum.Parse(propertyType, str, false) : TypeConverterHelper.Convert(str, propertyType.FullName));
                }
                else
                {
                    value = Value;
                }
                property.SetValue(targetObject, value, new Object[0]);
            }
            catch (FormatException formatException)
            {
                exception = formatException;
            }
            catch (ArgumentException argumentException)
            {
                exception = argumentException;
            }

            if (exception != null)
            {
                var currentCulture = CultureInfo.CurrentCulture;
                var changePropertyActionCannotSetValueExceptionMessage = ExceptionStringTable.ChangePropertyActionCannotSetValueExceptionMessage;
                var objArray = new object[] { (Value != null ? Value.GetType().Name : "null"), PropertyName, property.Name };
                throw new ArgumentException(string.Format(currentCulture, changePropertyActionCannotSetValueExceptionMessage, objArray), exception);
            }
        }

        private void ValidateProperty(string targetTypeName, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                var currentCulture = CultureInfo.CurrentCulture;
                var changePropertyActionCannotFindPropertyNameExceptionMessage = ExceptionStringTable.ChangePropertyActionCannotFindPropertyNameExceptionMessage;
                var propertyName = new object[] { PropertyName, targetTypeName };
                throw new ArgumentException(string.Format(currentCulture, changePropertyActionCannotFindPropertyNameExceptionMessage, propertyName));
            }
            if (!propertyInfo.CanWrite)
            {
                var cultureInfo = CultureInfo.CurrentCulture;
                var changePropertyActionPropertyIsReadOnlyExceptionMessage = ExceptionStringTable.ChangePropertyActionPropertyIsReadOnlyExceptionMessage;
                var objArray = new object[] { PropertyName, targetTypeName };
                throw new ArgumentException(string.Format(cultureInfo, changePropertyActionPropertyIsReadOnlyExceptionMessage, objArray));
            }
        }
    }
}

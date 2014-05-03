using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Xaml.Interactivity;

namespace Microsoft.Xaml.Interactions.Core
{
    /// <summary>
    /// Executes a specified ICommand when invoked.
    /// </summary>
    public class InvokeCommandAction : DependencyObject, IAction
    {
        public readonly static DependencyProperty CommandProperty;
        public readonly static DependencyProperty CommandParameterProperty;
        public readonly static DependencyProperty InputConverterProperty;
        public readonly static DependencyProperty InputConverterParameterProperty;
        public readonly static DependencyProperty InputConverterLanguageProperty;

        /// <summary>
        /// Gets or sets the command this action should invoke. This is a dependency property.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(InvokeCommandAction.CommandProperty); }
            set { SetValue(InvokeCommandAction.CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the parameter that is passed to Execute. If this is not set, the parameter from the Execute 
        /// method will be used. This is an optional dependency property.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(InvokeCommandAction.CommandParameterProperty); }
            set { SetValue(InvokeCommandAction.CommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the converter that is run on the parameter from the Execute method. This is an optional dependency property.
        /// </summary>
        public IValueConverter InputConverter
        {
            get { return (IValueConverter)GetValue(InvokeCommandAction.InputConverterProperty); }
            set { SetValue(InvokeCommandAction.InputConverterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the language that is passed to the Convert method of InputConverter. This is an optional dependency property.
        /// </summary>
        public string InputConverterLanguage
        {
            get { return (string)GetValue(InvokeCommandAction.InputConverterLanguageProperty); }
            set { SetValue(InvokeCommandAction.InputConverterLanguageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the parameter that is passed to the Convert method of InputConverter. This is an optional dependency property.
        /// </summary>
        public object InputConverterParameter
        {
            get { return GetValue(InvokeCommandAction.InputConverterParameterProperty); }
            set { SetValue(InvokeCommandAction.InputConverterParameterProperty, value); }
        }

        static InvokeCommandAction()
        {
            InvokeCommandAction.CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandAction), new PropertyMetadata(null));
            InvokeCommandAction.CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandAction), new PropertyMetadata(null));
            InvokeCommandAction.InputConverterProperty = DependencyProperty.Register("InputConverter", typeof(IValueConverter), typeof(InvokeCommandAction), new PropertyMetadata(null));
            InvokeCommandAction.InputConverterParameterProperty = DependencyProperty.Register("InputConverterParameter", typeof(object), typeof(InvokeCommandAction), new PropertyMetadata(null));
            InvokeCommandAction.InputConverterLanguageProperty = DependencyProperty.Register("InputConverterLanguage", typeof(string), typeof(InvokeCommandAction), new PropertyMetadata(String.Empty));
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that is passed to the action by the behavior. Generally this is AssociatedObject or the target object.</param>
        /// <param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <returns>true if updating the property value succeeds; otherwise, false.</returns>
        public object Execute(object sender, object parameter)
        {
            object commandParameter;

            if (Command == null)
                return false;

            if (CommandParameter == null)
            {
                commandParameter = (InputConverter != null
                    ? InputConverter.Convert(parameter, typeof(object), InputConverterParameter, new CultureInfo(InputConverterLanguage))
                    : parameter);
            }
            else
            {
                commandParameter = CommandParameter;
            }

            if (Command.CanExecute(commandParameter))
            {
                Command.Execute(commandParameter);
                return true;
            }

            return false;
        }
    }
}

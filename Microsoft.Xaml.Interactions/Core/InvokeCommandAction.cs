using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Xaml.Interactivity;

namespace Microsoft.Xaml.Interactions.Core
{
    public class InvokeCommandAction : DependencyObject, IAction
    {
        public readonly static DependencyProperty CommandProperty;
        public readonly static DependencyProperty CommandParameterProperty;
        public readonly static DependencyProperty InputConverterProperty;
        public readonly static DependencyProperty InputConverterParameterProperty;
        public readonly static DependencyProperty InputConverterLanguageProperty;

        public ICommand Command
        {
            get { return (ICommand)GetValue(InvokeCommandAction.CommandProperty); }
            set { SetValue(InvokeCommandAction.CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(InvokeCommandAction.CommandParameterProperty); }
            set { SetValue(InvokeCommandAction.CommandParameterProperty, value); }
        }

        public IValueConverter InputConverter
        {
            get { return (IValueConverter)GetValue(InvokeCommandAction.InputConverterProperty); }
            set { SetValue(InvokeCommandAction.InputConverterProperty, value); }
        }

        public string InputConverterLanguage
        {
            get { return (string)GetValue(InvokeCommandAction.InputConverterLanguageProperty); }
            set { SetValue(InvokeCommandAction.InputConverterLanguageProperty, value); }
        }

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

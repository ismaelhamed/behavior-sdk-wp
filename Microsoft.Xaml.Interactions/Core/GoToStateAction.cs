using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Interactivity;

namespace Microsoft.Xaml.Interactions.Core
{
    public class GoToStateAction : DependencyObject, IAction
    {
        public readonly static DependencyProperty UseTransitionsProperty;
        public readonly static DependencyProperty StateNameProperty;
        public readonly static DependencyProperty TargetObjectProperty;

        public string StateName
        {
            get { return (string)GetValue(GoToStateAction.StateNameProperty); }
            set { SetValue(GoToStateAction.StateNameProperty, value); }
        }

        public FrameworkElement TargetObject
        {
            get { return (FrameworkElement)GetValue(GoToStateAction.TargetObjectProperty); }
            set { SetValue(GoToStateAction.TargetObjectProperty, value); }
        }

        public bool UseTransitions
        {
            get { return (bool)GetValue(GoToStateAction.UseTransitionsProperty); }
            set { SetValue(GoToStateAction.UseTransitionsProperty, value); }
        }

        static GoToStateAction()
        {
            GoToStateAction.UseTransitionsProperty = DependencyProperty.Register("UseTransitions", typeof(bool), typeof(GoToStateAction), new PropertyMetadata(true));
            GoToStateAction.StateNameProperty = DependencyProperty.Register("StateName", typeof(string), typeof(GoToStateAction), new PropertyMetadata(null));
            GoToStateAction.TargetObjectProperty = DependencyProperty.Register("TargetObject", typeof(FrameworkElement), typeof(GoToStateAction), new PropertyMetadata(null));
        }

        public object Execute(object sender, object parameter)
        {
            if (string.IsNullOrEmpty(StateName))
            {
                return false;
            }

            if (ReadLocalValue(GoToStateAction.TargetObjectProperty) != DependencyProperty.UnsetValue)
            {
                var targetObject = TargetObject as Control;
                if (targetObject == null)
                {
                    return false;
                }
                return VisualStateUtilities.GoToState(targetObject, StateName, UseTransitions);
            }

            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null || !EventTriggerBehavior.IsElementLoaded(frameworkElement))
            {
                return false;
            }

            var control = VisualStateUtilities.FindNearestStatefulControl(frameworkElement);
            if (control == null)
            {
                var currentCulture = CultureInfo.CurrentCulture;
                var goToStateActionTargetHasNoStateGroups = ExceptionStringTable.GoToStateActionTargetHasNoStateGroups;
                var name = new object[] { frameworkElement.Name };
                throw new InvalidOperationException(string.Format(currentCulture, goToStateActionTargetHasNoStateGroups, name));
            }
            return VisualStateUtilities.GoToState(control, StateName, UseTransitions);
        }
    }
}

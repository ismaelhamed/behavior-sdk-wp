using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Xaml.Interactivity
{
    public static class VisualStateUtilities
    {
        public static Control FindNearestStatefulControl(FrameworkElement contextElement)
        {
            var frameworkElement = contextElement;
            if (frameworkElement == null)
            {
                throw new ArgumentNullException("contextElement");
            }

            for (var i = contextElement.Parent as FrameworkElement; !VisualStateUtilities.HasVisualStateGroupsDefined(contextElement) && VisualStateUtilities.ShouldContinueTreeWalk(i); i = i.Parent as FrameworkElement)
            {
                contextElement = i;
            }

            if (!VisualStateUtilities.HasVisualStateGroupsDefined(frameworkElement))
            {
                return null;
            }
            
            var parent = VisualTreeHelper.GetParent(frameworkElement) as Control;
            if (parent != null)
            {
                frameworkElement = parent;
            }

            return frameworkElement as Control;
        }

        /// <summary>
        /// Gets the value of the VisualStateManager.VisualStateGroups attached property.
        /// </summary>
        /// <param name="targetObject">The element from which to get the VisualStateManager.VisualStateGroups.</param>
        /// <returns></returns>
        public static IList GetVisualStateGroups(FrameworkElement targetObject)
        {
            IList visualStateGroups = new List<VisualStateGroup>();
            if (targetObject != null)
            {
                visualStateGroups = VisualStateManager.GetVisualStateGroups(targetObject);
                if (visualStateGroups.Count == 0 && VisualTreeHelper.GetChildrenCount(targetObject) > 0)
                {
                    var child = VisualTreeHelper.GetChild(targetObject, 0) as FrameworkElement;
                    if (child != null)
                    {
                        visualStateGroups = VisualStateManager.GetVisualStateGroups(child);
                    }
                }
            }
            return visualStateGroups;
        }

        public static Boolean GoToState(Control control, String stateName, Boolean useTransitions)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (String.IsNullOrEmpty(stateName))
            {
                throw new ArgumentNullException("stateName");
            }
            control.ApplyTemplate();
            return VisualStateManager.GoToState(control, stateName, useTransitions);
        }

        private static Boolean HasVisualStateGroupsDefined(FrameworkElement element)
        {
            if (element == null)
            {
                return false;
            }
            return VisualStateManager.GetVisualStateGroups(element).Count != 0;
        }

        private static Boolean ShouldContinueTreeWalk(FrameworkElement element)
        {
            if (element == null)
                return false;

            if (element is UserControl)
                return false;

            if (element.Parent == null)
            {
                var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
                if (parent == null || !(parent is Control) && !(parent is ContentPresenter))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

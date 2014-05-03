using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Interactivity;

namespace Microsoft.Xaml.Interactions.Core
{
    /// <summary>
    /// Represents an action that switches the current visual to the specified Page.
    /// </summary>
    public class NavigateToPageAction : DependencyObject, IAction
    {
        public readonly static DependencyProperty TargetPageProperty;

        /// <summary>
        /// Gets or sets the fully qualified name of the Page to navigate to. This is a dependency property.
        /// </summary>
        public string TargetPage
        {
            get { return (string)GetValue(NavigateToPageAction.TargetPageProperty); }
            set { SetValue(NavigateToPageAction.TargetPageProperty, value); }
        }

        static NavigateToPageAction()
        {
            NavigateToPageAction.TargetPageProperty = DependencyProperty.Register("TargetPage", typeof(string), typeof(NavigateToPageAction), new PropertyMetadata(null));
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that is passed to the action by the behavior. Generally this is AssociatedObject or the target object.</param>
        /// <param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <returns>true if updating the property value succeeds; otherwise, false.</returns>
        public object Execute(object sender, object parameter)
        {
            if (string.IsNullOrEmpty(TargetPage))
            {
                return false;
            }

            var parent = sender as DependencyObject;
            var rootVisual = Application.Current.RootVisual as INavigate;

            while (parent != null && rootVisual == null)
            {
                rootVisual = sender as INavigate;
                if (rootVisual != null)
                {
                    continue;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return rootVisual != null && rootVisual.Navigate(new Uri(TargetPage, UriKind.Relative));
        }
    }
}

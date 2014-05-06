using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Interactivity;

namespace Microsoft.Xaml.Interactions.Media
{
    /// <summary>
    /// Represents an action that will change the state of the specified Storyboard when executed.
    /// </summary>
    public class ControlStoryboardAction : DependencyObject, IAction
    {
        private Boolean isPaused;

        public readonly static DependencyProperty StoryboardProperty;
        public readonly static DependencyProperty ControlStoryboardOptionProperty;

        /// <summary>
        /// Gets or sets the action to execute on the Storyboard. This is a dependency property.
        /// </summary>
        public ControlStoryboardOption ControlStoryboardOption
        {
            get { return (ControlStoryboardOption)GetValue(ControlStoryboardAction.ControlStoryboardOptionProperty); }
            set { SetValue(ControlStoryboardAction.ControlStoryboardOptionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the targeted Storyboard. This is a dependency property.
        /// </summary>
        public Storyboard Storyboard
        {
            get { return (Storyboard)GetValue(ControlStoryboardAction.StoryboardProperty); }
            set { SetValue(ControlStoryboardAction.StoryboardProperty, value); }
        }

        static ControlStoryboardAction()
        {
            ControlStoryboardAction.ControlStoryboardOptionProperty = DependencyProperty.Register("ControlStoryboardOption", typeof(ControlStoryboardOption), typeof(ControlStoryboardAction), new PropertyMetadata(ControlStoryboardOption.Play));
            ControlStoryboardAction.StoryboardProperty = DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(ControlStoryboardAction), new PropertyMetadata(null, ControlStoryboardAction.OnStoryboardChanged));
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that is passed to the action by the behavior. Generally this is AssociatedObject or the target object.</param>
        /// <param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <returns>true if updating the property value succeeds; otherwise, false.</returns>
        public object Execute(object sender, object parameter)
        {
            if (Storyboard == null)
                return false;

            switch (ControlStoryboardOption)
            {
                case ControlStoryboardOption.Play:
                    {
                        Storyboard.Begin();
                        break;
                    }
                case ControlStoryboardOption.Stop:
                    {
                        Storyboard.Stop();
                        break;
                    }
                case ControlStoryboardOption.TogglePlayPause:
                    {
                        var currentState = ClockState.Stopped;
                        try
                        {
                            currentState = Storyboard.GetCurrentState();
                        }
                        catch (InvalidOperationException)
                        { }

                        if (currentState == ClockState.Stopped)
                        {
                            isPaused = false;
                            Storyboard.Begin();
                            break;
                        }

                        if (!isPaused)
                        {
                            isPaused = true;
                            Storyboard.Pause();
                            break;
                        }

                        isPaused = false;
                        Storyboard.Resume();
                        break;
                    }
                case ControlStoryboardOption.Pause:
                    {
                        Storyboard.Pause();
                        break;
                    }
                case ControlStoryboardOption.Resume:
                    {
                        Storyboard.Resume();
                        break;
                    }
                case ControlStoryboardOption.SkipToFill:
                    {
                        Storyboard.SkipToFill();
                        break;
                    }
                default:
                    return false;
            }
            return true;
        }

        private static void OnStoryboardChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var controlStoryboardAction = sender as ControlStoryboardAction;
            if (controlStoryboardAction != null)
            {
                controlStoryboardAction.isPaused = false;
            }
        }
    }
}

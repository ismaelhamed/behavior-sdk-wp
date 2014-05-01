using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Xaml.Interactivity;

namespace Microsoft.Xaml.Interactions.Core
{
    [ContentProperty("Actions")]
    public sealed class EventTriggerBehavior : DependencyObject, IBehavior
    {
        private object resolvedSource;
        private bool isLoadedEventRegistered;
        private MethodInfo eventHandlerMethodInfo;

        public readonly static DependencyProperty ActionsProperty;
        public readonly static DependencyProperty EventNameProperty;
        public readonly static DependencyProperty SourceObjectProperty;

        public ActionCollection Actions
        {
            get
            {
                var value = (ActionCollection)GetValue(EventTriggerBehavior.ActionsProperty);
                if (value == null)
                {
                    value = new ActionCollection();
                    SetValue(EventTriggerBehavior.ActionsProperty, value);
                }
                return value;
            }
        }

        public DependencyObject AssociatedObject
        {
            get; 
            private set;
        }

        public string EventName
        {
            get { return (string)GetValue(EventTriggerBehavior.EventNameProperty); }
            set { SetValue(EventTriggerBehavior.EventNameProperty, value); }
        }

        public object SourceObject
        {
            get { return GetValue(EventTriggerBehavior.SourceObjectProperty); }
            set { SetValue(EventTriggerBehavior.SourceObjectProperty, value); }
        }

        static EventTriggerBehavior()
        {
            EventTriggerBehavior.ActionsProperty = DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(EventTriggerBehavior), new PropertyMetadata(null));
            EventTriggerBehavior.EventNameProperty = DependencyProperty.Register("EventName", typeof(string), typeof(EventTriggerBehavior), new PropertyMetadata("Loaded", EventTriggerBehavior.OnEventNameChanged));
            EventTriggerBehavior.SourceObjectProperty = DependencyProperty.Register("SourceObject", typeof(object), typeof(EventTriggerBehavior), new PropertyMetadata(null, EventTriggerBehavior.OnSourceObjectChanged));
        }

        public void Attach(DependencyObject obj)
        {
            if (AssociatedObject == obj || DesignerProperties.IsInDesignTool)
                return;
   
            if (AssociatedObject != null)
            {
                throw new InvalidOperationException("CannotAttachBehaviorMultipleTimesExceptionMessage");
            }

            AssociatedObject = obj;
            SetResolvedSource(ComputeResolvedSource());
        }

        public void Detach()
        {
            SetResolvedSource(null);
            AssociatedObject = null;
        }
        
        internal static bool IsElementLoaded(FrameworkElement element)
        {
            if (element == null)
                return false;

            if ((element.Parent ?? VisualTreeHelper.GetParent(element)) != null)
                return true;

            var content = Application.Current.RootVisual;
            if (content == null)
            {
                return false;
            }
            return element == content;
        }

        private void OnEvent(object sender, object eventArgs)
        {
            Interaction.ExecuteActions(resolvedSource, Actions, eventArgs);
        }

        private static void OnEventNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var eventTriggerBehavior = (EventTriggerBehavior)dependencyObject;
            if (eventTriggerBehavior.AssociatedObject == null || eventTriggerBehavior.resolvedSource == null)
                return;
      
            var oldValue = (String)args.OldValue;
            var newValue = (String)args.NewValue;

            eventTriggerBehavior.UnregisterEvent(oldValue);
            eventTriggerBehavior.RegisterEvent(newValue);
        }

        private static void OnSourceObjectChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var eventTriggerBehavior = (EventTriggerBehavior)dependencyObject;
            eventTriggerBehavior.SetResolvedSource(eventTriggerBehavior.ComputeResolvedSource());
        }

        private object ComputeResolvedSource()
        {
            return ReadLocalValue(EventTriggerBehavior.SourceObjectProperty) != DependencyProperty.UnsetValue ? SourceObject : AssociatedObject;
        }

        private void SetResolvedSource(object newSource)
        {
            if (AssociatedObject == null || resolvedSource == newSource)
                return;

            if (resolvedSource != null)
                UnregisterEvent(EventName);

            resolvedSource = newSource;

            if (resolvedSource != null)
            {
                RegisterEvent(EventName);
            }
        }

        private void RegisterEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            if (eventName == "Loaded")
            {
                if (!isLoadedEventRegistered)
                {
                    var frameworkElement = resolvedSource as FrameworkElement;
                    if (frameworkElement != null && !EventTriggerBehavior.IsElementLoaded(frameworkElement))
                    {
                        isLoadedEventRegistered = true;
                        frameworkElement.Loaded += OnEvent;
                    }
                }
                return;
            }

            var runtimeEvent = resolvedSource.GetType().GetEvent(EventName);
            if (runtimeEvent == null)
            {
                throw new ArgumentException("CannotFindEventNameExceptionMessage");
            }

            eventHandlerMethodInfo = typeof(EventTriggerBehavior).GetMethod("OnEvent", BindingFlags.Instance | BindingFlags.NonPublic);
            runtimeEvent.AddEventHandler(resolvedSource, Delegate.CreateDelegate(runtimeEvent.EventHandlerType, this, eventHandlerMethodInfo));
        }

        private void UnregisterEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
                return;

            if (eventName == "Loaded")
            {
                if (isLoadedEventRegistered)
                {
                    isLoadedEventRegistered = false;

                    var frameworkElement = (FrameworkElement)resolvedSource;
                    if (frameworkElement != null)
                    {
                        frameworkElement.Loaded -= OnEvent;
                    }
                }
                return;
            }

            if (eventHandlerMethodInfo == null)
                return;

            var runtimeEvent = resolvedSource.GetType().GetEvent(eventName);
            runtimeEvent.RemoveEventHandler(resolvedSource, Delegate.CreateDelegate(runtimeEvent.EventHandlerType, this, eventHandlerMethodInfo));
            eventHandlerMethodInfo = null;
        }
    }
}
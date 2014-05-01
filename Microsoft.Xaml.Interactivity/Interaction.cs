using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Microsoft.Xaml.Interactivity
{
    public class Interaction
    {
        public readonly static DependencyProperty BehaviorsProperty;

        static Interaction()
        {
            Interaction.BehaviorsProperty = DependencyProperty.RegisterAttached("Behaviors", typeof(BehaviorCollection), typeof(Interaction), new PropertyMetadata(null, Interaction.OnBehaviorsChanged));
        }

        private Interaction()
        { }

        public static BehaviorCollection GetBehaviors(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var value = (BehaviorCollection)obj.GetValue(Interaction.BehaviorsProperty);
            if (value == null)
            {
                value = new BehaviorCollection();
                obj.SetValue(Interaction.BehaviorsProperty, value);
            }
            return value;
        }

        public static void SetBehaviors(DependencyObject obj, BehaviorCollection value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            obj.SetValue(Interaction.BehaviorsProperty, value);
        }

        private static void OnBehaviorsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var oldValue = (BehaviorCollection)args.OldValue;
            var newValue = (BehaviorCollection)args.NewValue;

            if (oldValue == newValue)
            {
                return;
            }
            if (oldValue != null && oldValue.AssociatedObject != null)
            {
                oldValue.Detach();
            }
            if (newValue != null)
            {
                newValue.Attach(sender);
            }
        }
        
        public static IEnumerable<Object> ExecuteActions(Object sender, ActionCollection actions, Object parameter)
        {
            var objs = new List<Object>();
            if (actions == null || DesignerProperties.IsInDesignTool)
            {
                return objs;
            }

            objs.AddRange(actions.Select(action => ((IAction)action).Execute(sender, parameter)));
            return objs;
        }
    }
}
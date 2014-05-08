using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Xaml.Interactivity;

namespace Microsoft.Xaml.Interactions.Core
{
    /// <summary>
    /// Represents a behavior that performs actions when the bound data meets a specified condition.
    /// </summary>
    [ContentProperty("Actions")]
    public class DataTriggerBehavior : DependencyObject, IBehavior
    {
        public readonly static DependencyProperty ActionsProperty;
        public readonly static DependencyProperty BindingProperty;
        public readonly static DependencyProperty ComparisonConditionProperty;
        public readonly static DependencyProperty ValueProperty;

        /// <summary>
        /// Gets the DependencyObject to which the IBehavior is attached.
        /// </summary>
        public DependencyObject AssociatedObject
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection of actions associated with the behavior. This is a dependency property.
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                var value = (ActionCollection)GetValue(DataTriggerBehavior.ActionsProperty);
                if (value == null)
                {
                    value = new ActionCollection();
                    SetValue(DataTriggerBehavior.ActionsProperty, value);
                }
                return value;
            }
        }

        /// <summary>
        /// Gets or sets the bound object that the DataTriggerBehavior will listen to. This is a dependency property.
        /// </summary>
        public object Binding
        {
            get { return GetValue(DataTriggerBehavior.BindingProperty); }
            set { SetValue(DataTriggerBehavior.BindingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of comparison to be performed between Binding and Value. This is a dependency property.
        /// </summary>
        public ComparisonConditionType ComparisonCondition
        {
            get { return (ComparisonConditionType)GetValue(DataTriggerBehavior.ComparisonConditionProperty); }
            set { SetValue(DataTriggerBehavior.ComparisonConditionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value to be compared with the value of Binding. This is a dependency property.
        /// </summary>
        public object Value
        {
            get { return GetValue(DataTriggerBehavior.ValueProperty); }
            set { SetValue(DataTriggerBehavior.ValueProperty, value); }
        }

        static DataTriggerBehavior()
        {
            DataTriggerBehavior.ActionsProperty = DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(DataTriggerBehavior), new PropertyMetadata(null));
            DataTriggerBehavior.BindingProperty = DependencyProperty.Register("Binding", typeof(object), typeof(DataTriggerBehavior), new PropertyMetadata(null, DataTriggerBehavior.OnValueChanged));
            DataTriggerBehavior.ComparisonConditionProperty = DependencyProperty.Register("ComparisonCondition", typeof(ComparisonConditionType), typeof(DataTriggerBehavior), new PropertyMetadata(ComparisonConditionType.Equal, DataTriggerBehavior.OnValueChanged));
            DataTriggerBehavior.ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DataTriggerBehavior), new PropertyMetadata(null, DataTriggerBehavior.OnValueChanged));
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The DependencyObject to which the IBehavior will be attached.</param>
        public void Attach(DependencyObject associatedObject)
        {
            if (AssociatedObject == associatedObject || DesignerProperties.IsInDesignTool)
            {
                return;
            }

            if (AssociatedObject != null)
            {
                var currentCulture = CultureInfo.CurrentCulture;
                var cannotAttachBehaviorMultipleTimesExceptionMessage = ExceptionStringTable.CannotAttachBehaviorMultipleTimesExceptionMessage;
                var objArray = new object[] { associatedObject, AssociatedObject };
                throw new InvalidOperationException(string.Format(currentCulture, cannotAttachBehaviorMultipleTimesExceptionMessage, objArray));
            }

            AssociatedObject = associatedObject;
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            AssociatedObject = null;
        }

        private static bool Compare(object leftOperand, ComparisonConditionType operatorType, object rightOperand)
        {
            if (leftOperand != null && rightOperand != null)
            {
                rightOperand = TypeConverterHelper.Convert(rightOperand.ToString(), leftOperand.GetType().FullName);
            }

            var comparable = leftOperand as IComparable;
            var comparable1 = rightOperand as IComparable;

            if (comparable != null && comparable1 != null)
            {
                return DataTriggerBehavior.EvaluateComparable(comparable, operatorType, comparable1);
            }
            switch (operatorType)
            {
                case ComparisonConditionType.Equal:
                    {
                        return Object.Equals(leftOperand, rightOperand);
                    }
                case ComparisonConditionType.NotEqual:
                    {
                        return !Object.Equals(leftOperand, rightOperand);
                    }
                case ComparisonConditionType.LessThan:
                case ComparisonConditionType.LessThanOrEqual:
                case ComparisonConditionType.GreaterThan:
                case ComparisonConditionType.GreaterThanOrEqual:
                    {
                        if (comparable != null || comparable1 != null)
                        {
                            if (comparable != null)
                            {
                                var currentCulture = CultureInfo.CurrentCulture;
                                var invalidRightOperand = ExceptionStringTable.InvalidRightOperand;
                                var objArray = new object[] { (rightOperand != null ? rightOperand.GetType().Name : "null"), operatorType.ToString() };
                                throw new ArgumentException(String.Format(currentCulture, invalidRightOperand, objArray));
                            }
                            var cultureInfo = CultureInfo.CurrentCulture;
                            var invalidLeftOperand = ExceptionStringTable.InvalidLeftOperand;
                            var objArray1 = new object[] { (leftOperand != null ? leftOperand.GetType().Name : "null"), operatorType.ToString() };
                            throw new ArgumentException(String.Format(cultureInfo, invalidLeftOperand, objArray1));
                        }
                        var currentCulture1 = CultureInfo.CurrentCulture;
                        var invalidOperands = ExceptionStringTable.InvalidOperands;
                        var objArray2 = new object[] { (leftOperand != null ? leftOperand.GetType().Name : "null"), (rightOperand != null ? rightOperand.GetType().Name : "null"), operatorType.ToString() };
                        throw new ArgumentException(String.Format(currentCulture1, invalidOperands, objArray2));
                    }
            }
            return false;
        }

        private static bool EvaluateComparable(IComparable leftOperand, ComparisonConditionType operatorType, IComparable rightOperand)
        {
            object obj = null;
            try
            {
                obj = Convert.ChangeType(rightOperand, leftOperand.GetType(), CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            { }
            catch (InvalidCastException)
            { }

            if (obj == null)
            {
                return operatorType == ComparisonConditionType.NotEqual;
            }

            var num = leftOperand.CompareTo(obj);
            switch (operatorType)
            {
                case ComparisonConditionType.Equal:
                    return num == 0;
                case ComparisonConditionType.NotEqual:
                    return num != 0;
                case ComparisonConditionType.LessThan:
                    return num < 0;
                case ComparisonConditionType.LessThanOrEqual:
                    return num <= 0;
                case ComparisonConditionType.GreaterThan:
                    return num > 0;
                case ComparisonConditionType.GreaterThanOrEqual:
                    return num >= 0;
            }
            return false;
        }

        private static void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var dataTriggerBehavior = (DataTriggerBehavior)dependencyObject;
            if (dataTriggerBehavior.AssociatedObject == null)
            {
                return;
            }

            DataBindingHelper.RefreshDataBindingsOnActions(dataTriggerBehavior.Actions);
            if (DataTriggerBehavior.Compare(dataTriggerBehavior.Binding, dataTriggerBehavior.ComparisonCondition, dataTriggerBehavior.Value))
            {
                Interaction.ExecuteActions(dataTriggerBehavior.AssociatedObject, dataTriggerBehavior.Actions, args);
            }
        }
    }
}

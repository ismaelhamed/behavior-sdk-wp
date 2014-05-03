using System;
using System.Collections.Specialized;
using System.Windows;

namespace Microsoft.Xaml.Interactivity
{
    public sealed class ActionCollection : DependencyObjectCollection
    {
        public ActionCollection()
        {
            var actionCollection = this;
            actionCollection.CollectionChanged += ActionCollection_CollectionChanged;
        }

        private void ActionCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            var collectionChange = eventArgs.Action;
            switch (collectionChange)
            {
                case NotifyCollectionChangedAction.Reset:
                    foreach (var dependencyObject in this)
                    {
                        ActionCollection.VerifyType(dependencyObject);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Add:
                {
                    var action = this[eventArgs.NewStartingIndex] as IAction;
                    if (action != null)
                    {
                        ActionCollection.VerifyType(this[eventArgs.NewStartingIndex]);
                    }
                }
                    break;
            }
        }

        private static void VerifyType(DependencyObject item)
        {
            if (!(item is IAction))
            {
                throw new InvalidOperationException("NonActionAddedToActionCollectionExceptionMessage");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Microsoft.Xaml.Interactivity
{
    public sealed class BehaviorCollection : DependencyObjectCollection
    {
        private readonly List<IBehavior> oldCollection = new List<IBehavior>();

        public DependencyObject AssociatedObject
        {
            get;
            private set;
        }

        public BehaviorCollection()
        {
            var behaviorCollection = this;
            behaviorCollection.CollectionChanged += BehaviorCollection_CollectionChanged;
        }

        public void Attach(DependencyObject associatedObject)
        {
            if (associatedObject == AssociatedObject)
                return;

            if (DesignerProperties.IsInDesignTool)
                return;

            if (AssociatedObject != null)
            {
                throw new InvalidOperationException("CannotAttachBehaviorMultipleTimesExceptionMessage");
            }

            AssociatedObject = associatedObject;

            foreach (var dependencyObject in this)
            {
                ((IBehavior)dependencyObject).Attach(AssociatedObject);
            }
        }

        public void Detach()
        {
            foreach (var behavior in this.Cast<IBehavior>().Where(behavior => behavior.AssociatedObject != null))
            {
                behavior.Detach();
            }
            AssociatedObject = null;
            oldCollection.Clear();
        }

        private void BehaviorCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var behavior in oldCollection.Where(behavior => behavior.AssociatedObject != null))
                {
                    behavior.Detach();
                }

                oldCollection.Clear();

                foreach (var dependencyObject in this)
                {
                    oldCollection.Add(VerifiedAttach(dependencyObject));
                }
                return;
            }

            IBehavior item;
            var index = eventArgs.NewStartingIndex;
            var item1 = this[index];
            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        oldCollection.Insert(index, VerifiedAttach(item1));
                        return;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        item = oldCollection[index];
                        if (item.AssociatedObject != null)
                        {
                            item.Detach();
                        }
                        oldCollection.RemoveAt(index);
                        return;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        item = oldCollection[index];
                        if (item.AssociatedObject != null)
                        {
                            item.Detach();
                        }
                        oldCollection[index] = VerifiedAttach(item1);
                        return;
                    }
            }
        }

        private IBehavior VerifiedAttach(DependencyObject item)
        {
            var behavior = item as IBehavior;
            if (behavior == null)
            {
                throw new InvalidOperationException("NonBehaviorAddedToBehaviorCollectionExceptionMessage");
            }

            if (oldCollection.Contains(behavior))
            {
                throw new InvalidOperationException("DuplicateBehaviorInCollectionExceptionMessage");
            }

            if (AssociatedObject != null)
            {
                behavior.Attach(AssociatedObject);
            }

            return behavior;
        }
    }
}
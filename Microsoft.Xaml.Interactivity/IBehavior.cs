using System.Windows;

namespace Microsoft.Xaml.Interactivity
{
    public interface IBehavior
    {
        DependencyObject AssociatedObject
        {
            get;
        }

        void Attach(DependencyObject obj);
        void Detach();
    }
}
namespace Microsoft.Xaml.Interactivity
{
    public interface IAction
    {
        object Execute(object sender, object parameter);
    }
}
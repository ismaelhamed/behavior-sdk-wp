Behavior SDK (XAML) for Windows Phone (Silverlight)
===================================================

[![Build status](https://ci.appveyor.com/api/projects/status/yfct5fihd1i7cp5c)](https://ci.appveyor.com/project/ismaelhamed/behavior-sdk-wp)

The Behaviors Software Development Kit (XAML) provides information about behaviors, which are pieces of packaged code that you can reuse to add interactivity to your apps. To incorporate a behavior from the SDK, you drag it onto any object and then changing its properties to better suit your application.

With the release of Visual Studio 2013, Microsoft brought back support for Behaviors in Windows 8.1. This is akin to the System.Windows.Interactivity and Microsoft.Blend.Interactivity that you most probably have used already, but the whole process has been simplified. In fact, the concept of "Trigger" has been dropped and should be now implemented using just Behaviors.

**What's in the SDK?**

The Behavior SDK consists of two libraries:

- **The Microsoft.Xaml.Interactivity** contains the core tools needed to extend the capabilities of behaviors in Blend and Visual Studio.
- **The Microsoft.Xaml.Interactions** includes new built-in behaviors and actions.

The list of Behaviors and Actions contained in the Behavior SDK is as follows:

**Microsoft.Xaml.Interactions.Core**

- **CallMethodAction:** Represents an action that calls a method on a specified object when invoked.
- **ChangePropertyAction:** Represents an action that will change a specified property to a specified value when invoked.
- **DataTriggerBehavior:** Represents a behavior that performs actions when the bound data meets a specified condition.
- **EventTriggerBehavior:** Represents a behavior that listens for a specified event on its source and executes its actions when that event is fired.
- **GoToStateAction:** Represents an action that will transition a FrameworkElement to a specified VisualState when executed.
- **InvokeCommandAction:** Executes a specified ICommand when invoked.
- **NavigateToPageAction:** Represents an action that switches the current visual to the specified Page.

**Microsoft.Xaml.Interactions.Media**

- **ControlStoryboardAction:** Represents an action that will change the state of the specified Storyboard when executed.
- **PlaySoundAction:** Represents an action that will play a sound to completion.

**DISCLAIMER!**

This is a port of the Microsoft [Behavior SDK for Visual Studio 2013](http://msdn.microsoft.com/en-us/library/dn457340.aspx), and is provided to you "AS-IS".




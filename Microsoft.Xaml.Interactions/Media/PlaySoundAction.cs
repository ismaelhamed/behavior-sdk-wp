using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Interactivity;

namespace Microsoft.Xaml.Interactions.Media
{
    /// <summary>
    /// Represents an action that will play a sound to completion.
    /// </summary>
    public class PlaySoundAction : DependencyObject, IAction
    {
        public readonly static DependencyProperty SourceProperty;
        public readonly static DependencyProperty VolumeProperty;

        /// <summary>
        /// Gets or sets the location of the sound file. This is used to set the source property of a MediaElement. This is a dependency property.
        /// </summary>
        /// <remarks>The sound can be any file format supported by MediaElement. In the case of a video, it will play only the audio portion.</remarks>
        public Uri Source
        {
            get { return (Uri)GetValue(PlaySoundAction.SourceProperty); }
            set { SetValue(PlaySoundAction.SourceProperty, value); }
        }

        /// <summary>
        /// Gets or set the volume of the sound. This is used to set the Volume property of the MediaElement. This is a dependency property.
        /// </summary>
        public double Volume
        {
            get { return (double)GetValue(PlaySoundAction.VolumeProperty); }
            set { SetValue(PlaySoundAction.VolumeProperty, value); }
        }

        static PlaySoundAction()
        {
            PlaySoundAction.SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(PlaySoundAction), new PropertyMetadata(null));
            PlaySoundAction.VolumeProperty = DependencyProperty.Register("Volume", typeof(double), typeof(PlaySoundAction), new PropertyMetadata(0.5));
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that is passed to the action by the behavior. Generally this is AssociatedObject or the target object.</param>
        /// <param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <returns>true if updating the property value succeeds; otherwise, false.</returns>
        public object Execute(object sender, object parameter)
        {
            if (Source == null)
                return false;
           
            var popup = new Popup();
            var mediaElement = new MediaElement();
            popup.Child = mediaElement;
            mediaElement.Visibility = Visibility.Collapsed;
            mediaElement.Source = Source;
            mediaElement.Volume = Volume;

            mediaElement.MediaEnded += (argument0, argument1) =>
            {
                popup.Child = null;
                popup.IsOpen = false;
                popup = null;
            };
            mediaElement.MediaFailed += (argument2, argument3) =>
            {
                popup.Child = null;
                popup.IsOpen = false;
                popup = null;
            };

            popup.IsOpen = true;
            return true;
        }
    }
}

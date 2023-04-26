using System;
using System.Windows;
using System.Windows.Controls;

namespace TTS_2019.Tools.Utils
{
    public class DMButton : Button
    {
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(DMButton), new PropertyMetadata(null));
    }
}

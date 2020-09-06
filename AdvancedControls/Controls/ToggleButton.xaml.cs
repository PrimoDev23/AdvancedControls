using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AdvancedControls.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "<Ausstehend>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "<Ausstehend>")]
    public partial class ToggleButton : Grid
    {
        public event ToggledEventHandler ToggledEvent;

        private static readonly Dictionary<ToggleButton, Color> btn_color = new Dictionary<ToggleButton, Color>();

        public bool Toggled
        {
            get { return (bool)GetValue(ToggledProperty); }
            set { SetValue(ToggledProperty, value); }
        }
        public static readonly BindableProperty ToggledProperty = BindableProperty.Create(nameof(Toggled), typeof(bool), typeof(ToggleButton), false, propertyChanged: toggleChanged);

        public Color ToggledColor
        {
            get { return (Color)GetValue(ToggledColorProperty); }
            set { SetValue(ToggledColorProperty, value); }
        }
        public static readonly BindableProperty ToggledColorProperty = BindableProperty.Create(nameof(ToggledColor), typeof(Color), typeof(ToggleButton), Color.Black);

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(ImageSource), typeof(ToggleButton), null, propertyChanged: imageChanged);

        public double ImageScale
        {
            get { return (double)GetValue(ImageScaleProperty); }
            set { SetValue(ImageScaleProperty, value); }
        }
        public static readonly BindableProperty ImageScaleProperty = BindableProperty.Create(nameof(ImageScale), typeof(double), typeof(ToggleButton), 1.0, propertyChanged: imageScaleChanged);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ToggleButton), "", propertyChanged: textChanged);

        public ToggleButton()
        {
            InitializeComponent();
        }

        private static void toggleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                bool value = (bool)newValue;
                if (bindable is ToggleButton btn)
                {
                    if (value)
                    {
                        btn_color.Add(btn, btn.BackgroundColor);
                        btn.BackgroundColor = btn.ToggledColor;
                        btn.img.IsVisible = true;
                    }
                    else
                    {
                        btn.BackgroundColor = btn_color[btn];
                        btn.img.IsVisible = false;
                        btn_color.Remove(btn);
                    }
                }
            }
            catch
            {
            }
        }

        private static void textChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if(bindable is ToggleButton btn)
            {
                btn.btnText.Text = newValue?.ToString();
            }
        }

        private static void imageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if(bindable is ToggleButton btn)
            {
                btn.img.Source = (ImageSource)newValue;
            }
        }

        private static void imageScaleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ToggleButton btn)
            {
                btn.img.Scale = (double)newValue;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Toggled = !Toggled;
            ToggledEvent?.Invoke(sender, new ToggledEventArgs(Toggled));
        }
    }
}
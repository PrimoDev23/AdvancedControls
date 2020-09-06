using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AdvancedControls.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Ausstehend>")]
    public partial class ColorPicker : ContentView, IDisposable
    {
        public ColorPickerSettings settings;

        private bool currentlyCalculating = false;
        private readonly object colorLock = new object();

        private readonly TaskCompletionSource<Color> waiter = new TaskCompletionSource<Color>();

        public ColorPicker()
        {
            InitializeComponent();
        }

        public async Task<Color> showDialog(Layout<View> parent, ColorPickerSettings settings = null)
        {
            lock (colorLock)
            {
                if (currentlyCalculating)
                {
                    return settings == null ? Color.Black : settings.defaultColor;
                }

                currentlyCalculating = true;
            }

            if (settings != null)
            {
                this.settings = settings;
                red.Value = settings.selectedColor.R;
                green.Value = settings.selectedColor.G;
                blue.Value = settings.selectedColor.B;
                alpha.Value = settings.selectedColor.A;
                hex.Text = settings.selectedColor.ToHex();
            }
            else
            {
                this.settings = new ColorPickerSettings();
                alpha.Value = 1;
                hex.Text = this.settings.selectedColor.ToHex();
            }

            lock (colorLock)
            {
                currentlyCalculating = false;
            }

            Color color;
            BindingContext = this.settings;

            if (parent is Grid grid)
            {
                grid.Children.Add(this, 0, grid.ColumnDefinitions.Count == 0 ? 1 : grid.ColumnDefinitions.Count, 0, grid.RowDefinitions.Count == 0 ? 1 : grid.RowDefinitions.Count);
                color = await waiter.Task;
                grid.Children.Remove(this);
            }
            else
            {
                parent.Children.Add(this);
                color = await waiter.Task;
                parent.Children.Remove(this);
            }

            return color;
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            waiter?.TrySetResult(settings.defaultColor);
        }

        private void OK_Clicked(object sender, EventArgs e)
        {
            waiter?.TrySetResult(settings.selectedColor);
        }

        private async void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            lock (colorLock)
            {
                if (currentlyCalculating)
                {
                    return;
                }

                currentlyCalculating = true;
            }

            await setColor().ConfigureAwait(false);

            lock (colorLock)
            {
                currentlyCalculating = false;
            }
        }

        private async Task setColor()
        {
            settings.selectedColor = Color.FromRgba(red.Value, green.Value, blue.Value, alpha.Value);

            void setHex()
            {
                hex.Text = settings.selectedColor.ToHex();
            }
            await Device.InvokeOnMainThreadAsync(new Action(setHex));
        }

        public void Dispose()
        {
            OK.Clicked -= OK_Clicked;
            Cancel.Clicked -= Cancel_Clicked;
            red.ValueChanged -= Slider_ValueChanged;
            green.ValueChanged -= Slider_ValueChanged;
            blue.ValueChanged -= Slider_ValueChanged;
            settings = null;
            BindingContext = null;
        }

        private void hex_TextChanged(object sender, TextChangedEventArgs e)
        {
            lock (colorLock)
            {
                if (currentlyCalculating)
                {
                    return;
                }

                currentlyCalculating = true;
            }

            Color color = Color.FromHex(hex.Text);

            if (color.R == -1 && color.G == -1 && color.B == -1 && color.A == -1)
            {
                return;
            }

            settings.selectedColor = color;
            red.Value = settings.selectedColor.R;
            green.Value = settings.selectedColor.G;
            blue.Value = settings.selectedColor.B;
            alpha.Value = settings.selectedColor.A;

            lock (colorLock)
            {
                currentlyCalculating = false;
            }
        }
    }

    public class ColorPickerSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Colors

        private Color _selectedColor = Color.Black;
        public Color selectedColor
        {
            get { return _selectedColor; }
            set { _selectedColor = value; RaiseAllProperties(); }
        }

        public Color defaultColor { get; set; } = Color.Black;

        #endregion

        #region DialogSettings

        public int dialogCornerRadius { get; set; } = 20;

        public Color dialogBackground { get; set; } = Color.White;

        public Color textColor { get; set; } = Color.Black;

        public Color blurBackground { get; set; } = Color.FromHex("#33000000");

        public int entryWidth { get; set; } = 150;

        public int buttonCornerRadius { get; set; } = 10;

        public Color entryBackground { get; set; } = Color.White;

        #endregion

        public void RaiseAllProperties()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
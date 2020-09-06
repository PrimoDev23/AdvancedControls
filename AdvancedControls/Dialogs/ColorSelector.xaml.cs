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
    public partial class ColorSelector : ContentView, IDisposable
    {
        private bool currentlyCalculating = false;
        private readonly object colorLock = new object();

        private readonly TaskCompletionSource<Color> waiter = new TaskCompletionSource<Color>();

        ColorSelectorSettings settings;

        public ColorSelector()
        {
            InitializeComponent();
        }

        public async Task<Color> showDialog(Layout<View> parent, List<Color> colors, ColorSelectorSettings settings = null)
        {
            if(settings == null)
            {
                this.settings = new ColorSelectorSettings();
            }
            else
            {
                this.settings = settings;
            }

            BindingContext = this.settings;

            Color color;

            await addColors(colors);

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

        private async Task addColors(List<Color> colors)
        {
            Button btn = null;
            for(int i = 0;i < colors.Count; i++)
            {
                btn = new Button()
                {
                    BackgroundColor = colors[i],
                    CornerRadius = settings.buttonCornerRadius,
                    HeightRequest = settings.buttonHeight,
                    WidthRequest = settings.buttonWidth,
                    Text = "",
                    VerticalOptions = LayoutOptions.Start
                };
                btn.Clicked += Color_Clicked;
                colorStack.Children.Add(btn);
            }
        }

        private void Color_Clicked(object sender, EventArgs e)
        {
            waiter?.TrySetResult(((Button)sender).BackgroundColor);
        }

        public void Dispose()
        {
            for(int i = 0;i < colorStack.Children.Count; i++)
            {
                if(colorStack.Children[i] is Button button)
                {
                    button.Clicked -= Color_Clicked;
                }
            }
            BindingContext = null;
        }

    }

    public class ColorSelectorSettings
    {
        public GridLength rowHeight { get; set; } = 180;
        public Color blurBackground { get; set; } = Color.FromHex("#33000000");
        public Color dialogBack { get; set; } = Color.White;
        public double buttonHeight { get; set; } = 70;
        public double buttonWidth { get; set; } = 70;
        public int buttonCornerRadius { get; set; } = 50;
    }
}
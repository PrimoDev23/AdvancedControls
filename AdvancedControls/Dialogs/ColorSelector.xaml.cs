using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AdvancedControls.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorSelector : ContentView
    {
        public ColorSelector()
        {
            InitializeComponent();
        }
    }
}
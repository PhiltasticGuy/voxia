using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxIA.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VoxIA.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SongDetailsPage : ContentPage
    {
        public SongDetailsPage()
        {
            InitializeComponent();
            BindingContext = new SongDetailsViewModel();
        }
    }
}
using System.ComponentModel;
using VoxIA.Mobile.ViewModels;
using Xamarin.Forms;

namespace VoxIA.Mobile.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}
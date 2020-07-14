using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using project_cad.Models;
using project_cad.Views;
using project_cad.ViewModels;

namespace project_cad.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class DocumentsPage : ContentPage
    {
        DocumentsViewModel viewModel;

        public DocumentsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new DocumentsViewModel();
        }

        async void OnItemSelected(object sender, EventArgs args)
        {
            var layout = (BindableObject) sender;
            var document = (Document) layout.BindingContext;
            
            await Navigation.PushAsync(new DocumentPage(new DocumentViewModel(document)));
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewDocumentPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Documents.Count == 0)
                viewModel.IsBusy = true;
        }
    }
}
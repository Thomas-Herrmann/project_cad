using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using project_cad.Models;
using project_cad.ViewModels;

namespace project_cad.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentPage : ContentPage
    {
        private DocumentViewModel viewModel;

        public DocumentPage(DocumentViewModel viewModel) // existing document
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public DocumentPage() // new document
        {
            InitializeComponent();

            var model = new Document(); // TODO: fill..
                
            viewModel = new DocumentViewModel(model);
            BindingContext = viewModel;
        }

        async void AddStatement_Clicked(object sender, EventArgs e)
        {
            var layout = (BindableObject) sender;
            var statementViewModel = (StatementViewModel) layout.BindingContext;

            await Navigation.PushModalAsync(new NavigationPage(new NewStatementPage(viewModel.Statements.IndexOf(statementViewModel))));
        }

        async void RemoveStatement_Clicked(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var statementViewModel = (StatementViewModel)layout.BindingContext;

            await Task.Run(() => MessagingCenter.Send(this, "RemoveStatement", statementViewModel));
        }
    }
}
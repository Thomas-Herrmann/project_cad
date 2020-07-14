using project_cad.Models;
using project_cad.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace project_cad.ViewModels
{
    public class DocumentViewModel : BaseViewModel
    {
        private Document model;

        public ObservableCollection<StatementViewModel> Statements { get; set; }

        public DocumentViewModel(Document model)
        {
            this.model = model;

            Title = model.Name == null ? "Untitled" : model.Name;
            Statements = new ObservableCollection<StatementViewModel>() { new StatementViewModel(new Comment("add statement --->")) }; // TODO!

            MessagingCenter.Subscribe<NewStatementPage, Tuple<Statement, int>> (this, "AddStatement", (obj, pair) =>
            {
                var newStatement = pair.Item1 as Statement;
                var index = pair.Item2;
                var viewModel = new StatementViewModel(newStatement);

                model.AddStatement(newStatement, index);
                Statements.Insert(index, viewModel);
            });

            MessagingCenter.Subscribe<DocumentPage, StatementViewModel>(this, "RemoveStatement", (obj, viewmodel) =>
            {
               model.RemoveStatement(viewmodel.Model);
               Statements.Remove(viewmodel);
            });
        }
    }
}

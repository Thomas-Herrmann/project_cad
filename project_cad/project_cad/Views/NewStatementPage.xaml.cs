using project_cad.Models;
using ProjectCadLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace project_cad.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewStatementPage : ContentPage
    {
        public Statement Statement { get; set; }
        public List<string> StatementTypes { get; }
        
        private int index;

        public NewStatementPage(int index)
        {
            InitializeComponent();

            this.index = index;

            StatementTypes = new List<string>() 
            { 
                "Assignment", "Expression", "Comment"
            };

            BindingContext = this;
        }

        private Node getDefaultNode()
        {
            return new Node(Exp.NewHoleExp(0), new Point[] { new Point(0, 0) }, new Tuple<Label, Point>[0]);
        }

        private Statement getStatement(string name)
        {
            switch (name)
            {
                case "Assignment": return new Assignment("TODO", getDefaultNode());
                case "Expression": return new Expression(getDefaultNode());
                case "Comment": return new Comment("TODO");
                default: throw new ArgumentException("Unknown statement type.");
            }
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            string name = statementPicker.SelectedItem as string;

            MessagingCenter.Send(this, "AddStatement", Tuple.Create(getStatement(name), index));

            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
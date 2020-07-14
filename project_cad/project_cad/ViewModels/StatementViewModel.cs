using project_cad.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace project_cad.ViewModels
{
    public class StatementViewModel : BaseViewModel
    {
        public View StatementView { get; }
        public Statement Model { get; }

        public StatementViewModel(Statement model)
        {
            Model = model;
            StatementView = model.GetView();
        }
    }
}

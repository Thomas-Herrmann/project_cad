using project_cad.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace project_cad.ViewModels
{
    class ExpressionViewModel : BaseViewModel
    {
        private Expression model;

        public ExpressionViewModel(Expression model)
        {
            this.model = model;
        }
    }
}

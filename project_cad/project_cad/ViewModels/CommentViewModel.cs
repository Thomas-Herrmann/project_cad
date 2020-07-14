using project_cad.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace project_cad.ViewModels
{
    class CommentViewModel : BaseViewModel
    {
        private Comment model;

        public CommentViewModel(Comment model)
        {
            this.model = model;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace project_cad.Models
{
    public class Comment : Statement
    {
        private Label commentLabel;
        private RelativeLayout layout;

        private string text;

        public Comment(string text)
        {
            this.text = text;

            commentLabel = new Label() { Text = text };
            layout = new RelativeLayout() { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            layout.Children.Add(commentLabel, Constraint.RelativeToParent((parent) => 0));
        }

        public override View GetView()
        {
            return layout;
        }
    }
}

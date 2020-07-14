using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace project_cad.Models
{
    public class Expression : Statement
    {
        private RelativeLayout layout;

        public Expression(Node node)
        {
            this.node = node;

            layout = new RelativeLayout() { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

            layout.Children.Add(node, null);
        }

        public override View GetView()
        {
            return layout;
        }
    }
}

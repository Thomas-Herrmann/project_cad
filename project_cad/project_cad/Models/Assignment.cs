using Microsoft.FSharp.Collections;
using ProjectCadLanguage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Xamarin.Forms;

namespace project_cad.Models
{
    public class Assignment : Statement
    {
        private Label variableLabel;
        private RelativeLayout layout;

        public Assignment(string variable, Node node)
        {
            this.variable = variable;
            this.node = node;

            variableLabel = new Label() { Text = variable };
            layout = new RelativeLayout() { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

            layout.Children.Add(variableLabel, Constraint.RelativeToParent((parent) => 0));
            layout.Children.Add(node, Constraint.RelativeToView(variableLabel, (parent, sibling) => sibling.X + 5));
        }

        public override View GetView()
        {
            return layout;
        }
    }
}

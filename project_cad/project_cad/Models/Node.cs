using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using ProjectCadLanguage;
using System.Linq;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Xamarin.Forms.Internals;
using System.Numerics;

namespace project_cad.Models
{
    public class Node : RelativeLayout
    {
        private Tuple<View, Point>[] nodes;
        private Tuple<Label, Point>[] symbols;
        private Exp expression;
        
        public Node(Exp expression, Point[] childPositions, Tuple<Label, Point>[] symbols)
        {
            nodes = childPositions.Select(point => Tuple.Create<View, Point>(new Label() { Text = "?" }, point)).ToArray();

            this.symbols = symbols;
            this.expression = expression;

            nodes.ForEach(pair => Children.Add(pair.Item1, null));
            symbols.ForEach(pair => Children.Add(pair.Item1, null));

            UpdateLayout();
        }

        private void UpdateLayout()
        {
            var pairsX = new List<Tuple<View, Point>>(nodes.ToList());
            var pairsY = new List<Tuple<View, Point>>(nodes.ToList());

            symbols.ForEach(s =>
            {
                pairsX.Add(Tuple.Create<View, Point>(s.Item1, s.Item2));
                pairsY.Add(Tuple.Create<View, Point>(s.Item1, s.Item2));
            });

            pairsX.Sort((a, b) => (int)(a.Item2.X - b.Item2.X));
            pairsY.Sort((a, b) => (int)(a.Item2.Y - b.Item2.Y));

            if (pairsX.Count > 0)
            {
                var yIndex = pairsY.IndexOf(pairsX[0]) - 1;
                var yConstraint = yIndex >= 0 ? Constraint.RelativeToView(pairsY[yIndex].Item1, (parent, sibling) => sibling.Y + (pairsX[0].Item2.Y - pairsY[yIndex].Item2.Y)) : null;

                Children.Add(pairsX[0].Item1, null, yConstraint);
            }

            for (int i = 1; i < pairsX.Count; ++i)
            {
                var yIndex = pairsY.IndexOf(pairsX[i]) - 1;
                var xConstraint = Constraint.RelativeToView(pairsX[i - 1].Item1, (parent, sibling) => sibling.X + (pairsX[i].Item2.X - pairsX[i - 1].Item2.X));
                var yConstraint = yIndex >= 0 ? Constraint.RelativeToView(pairsY[yIndex].Item1, (parent, sibling) => sibling.Y + (pairsX[i].Item2.Y - pairsY[yIndex].Item2.Y)) : null; // TODO: account for width/height too

                Children.Add(pairsX[i].Item1, xConstraint, yConstraint); // If this does not work, it is because 'Add' does not replace. (we shall see)
            }
        }

        public void UpdateChild(Node child, int index)
        {
            if (index >= 0 && index < nodes.Length)
            {
                var old = nodes[index];

                nodes[index] = Tuple.Create((View) child, old.Item2);
                Children.Remove(old.Item1);
                Children.Add(child, null);

                UpdateLayout();
            }
            else
                throw new ArgumentException("The specified child is outside the range of children for this node type.");
        }

        public Exp Expression()
        {
            return ExpressionRec(expression);
        }

        private Exp ExpressionRec(Exp e1)
        {
            switch (e1)
            {
                case Exp.HoleExp e2:
                    if (nodes[e2.Item].Item1 is Label)
                        throw new NullReferenceException("A node must have children corresponding to its holes.");

                    return ((Node) nodes[e2.Item].Item1).Expression();
                case Exp.AppExp e2:
                    return Exp.NewAppExp(ExpressionRec(e2.Item1), ExpressionRec(e2.Item2));
                case Exp.FunExp e2:
                    return Exp.NewFunExp(e2.Item1, ExpressionRec(e2.Item2));
                case Exp.MatrixExp e2:
                    var es = new Exp[e2.Item2, e2.Item3];

                    for (int i = 0; i < e2.Item2; ++i)
                        for (int j = 0; j < e2.Item3; ++j)
                            es[i, j] = ExpressionRec(e2.Item1[i, j]);

                    return Exp.NewMatrixExp(es, e2.Item2, e2.Item3);
                case Exp.GuardExp e2:
                    var temp = e2
                        .Item
                        .ToList()
                        .Select(pair => Tuple.Create(new FSharpOption<Exp>(pair.Item1 == null ? null : ExpressionRec(pair.Item1.Value)), ExpressionRec(pair.Item2)));

                    return Exp.NewGuardExp(ListModule.OfSeq(temp));
                default:
                    return e1;
            }
        }
    }
}

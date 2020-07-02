using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using ProjectCadLanguage;
using System.Linq;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace project_cad.Models
{
    class Node
    {
        private Node[] children;
        private Exp expression;
        //private List<(Point, )> // TODO: how do we structure the code?
        
        public Node(Exp expression, int numChildren)
        {
            children = new Node[numChildren];

            this.expression = expression;
        }

        public void UpdateChild(Node child, int index)
        {
            if (index >= 0 && index < children.Length)
                children[index] = child;
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
                    if (children[e2.Item] == null)
                        throw new NullReferenceException("A node must have children corresponding to its holes.");

                    return children[e2.Item].Expression();
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

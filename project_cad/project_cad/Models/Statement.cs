using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.FSharp.Collections;
using ProjectCadLanguage;
using Xamarin.Forms;

namespace project_cad.Models
{
    public abstract class Statement
    {
        protected string variable;
        protected Value value;
        protected Node node;
        public event EventHandler OnUpdate;

        public void Run(FSharpMap<string, Value> env)
        {
            if (node != null)
            {
                var expression = node.Expression();

                try
                {
                    value = Language.interpret(env, expression);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message); // TODO: mark note error!
                }
            }
        }

        public abstract View GetView();

        public bool HasBinding()
        {
            return variable != null && value != null;
        }

        public Tuple<string, Value> GetBinding()
        {
            if (variable == null || value == null)
                throw new InvalidOperationException("The statement is not a valid assigment.");
            else
                return Tuple.Create(variable, value);
        }
    }
}

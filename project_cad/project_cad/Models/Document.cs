using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using ProjectCadLanguage;
using Microsoft.FSharp.Collections;

namespace project_cad.Models
{
    public class Document
    {
        private List<Statement> statements;

        public string Name { get; set; }

        public Document()
        {
            statements = new List<Statement>();
        }

        public void AddStatement(Statement statement, int index)
        {
            statement.OnUpdate += new EventHandler(OnStatementChanged);
            statements.Insert(index, statement);
            OnStatementChanged(statement, null);
        }

        public void RemoveStatement(Statement statement)
        {
            var index = statements.IndexOf(statement);

            statements.Remove(statement);

            if (index < statements.Count)
                OnStatementChanged(statements[index], null);
        }

        private FSharpMap<string, Value> GetEnvironment(int index)
        {
            if (index >= statements.Count)
                throw new ArgumentException("Index out of range.");

            var bindings = new List<Tuple<string, Value>>();

            for (int i = 0; i < index; ++i)
                if (statements[i].HasBinding())
                    bindings.Add(statements[i].GetBinding());

            return new FSharpMap<string, Value>(bindings);
        }

        private void OnStatementChanged(object sender, EventArgs e)
        {
            var index = statements.IndexOf((Statement) sender);
            var env = GetEnvironment(index);

            for (int i = index; i < statements.Count - 1; ++i)
            {
                var subject = statements[i];

                subject.Run(env);

                if (subject.HasBinding())
                {
                    var binding = subject.GetBinding();

                    env.Add(binding.Item1, binding.Item2);
                }
            }

            statements[statements.Count - 1].Run(env);
        }
    }
}

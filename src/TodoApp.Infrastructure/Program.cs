using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new TodoAppStack(app, "TodoAppStack", new StackProps { });
            app.Synth();
        }
    }
}

Miracle.Arguments
=================

Simple annotation based command line parser with automatic help generation.

Sample usage:

```csharp
using System;
using Miracle.Arguments;

namespace Samples
{
    [ArgumentDescription("Simple hello world application")]
    public class Arguments
    {
        public Arguments()
        {
            Count = 1; // Default value
        }

        [ArgumentName("Say", "S")]
        [ArgumentRequired]
        [ArgumentDescription("Say something.")]
        public string Message { get; set; }

        [ArgumentName("Times", "T")]
        [ArgumentDescription("Number of times to say.")]
        public int Count { get; set; }

        [ArgumentName("Help", "H", "?")]
        [ArgumentHelp()]
        [ArgumentDescription("Show help.")]
        public bool Help { get; set; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var parsedArguments = args.ParseCommandLine<Arguments>();
            if (parsedArguments != null)
            {
                for (int i = 0; i < parsedArguments.Count; i++)
                    Console.Write(parsedArguments.Message);
            }
        }
    }
}
```

Se sample applications and/or unit tests for more complex usage.

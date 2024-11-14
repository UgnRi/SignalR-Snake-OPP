using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalR_Snake.Models;

namespace SignalR_Snake.Models.Command
{
    public class CommandInvoker : ICommand
    {
        private readonly Queue<ICommand> _commandQueue = new Queue<ICommand>();

        public void AddCommand(ICommand command)
        {
            _commandQueue.Enqueue(command);
        }

        public void ExecuteAll()
        {
            while (_commandQueue.Count > 0)
            {
                var command = _commandQueue.Dequeue();
                command.Execute();
            }
        }

        public void Execute()
        {
            ExecuteAll();
        }
    }
}

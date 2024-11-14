using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Snake.Models.Command
{
    public interface ICommand
    {
        void Execute();
    }
}

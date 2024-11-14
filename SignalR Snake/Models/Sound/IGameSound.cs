using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using System.Threading.Tasks;
using SignalR_Snake.Models.Sound;

namespace SignalR_Snake.Models.Sound
{
    public interface IGameSound
    {
        void PlaySound();
        void StopSound();
    }
}

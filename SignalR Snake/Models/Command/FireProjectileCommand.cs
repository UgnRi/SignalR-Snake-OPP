using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalR_Snake.Models;

namespace SignalR_Snake.Models.Command
{
    public class FireProjectileCommand : ICommand
    {
        private readonly string _ownerId;
        private readonly double _startX, _startY, _targetX, _targetY;
        private readonly double _speed;
        private readonly Action<Projectile> _onProjectileCreated;

        public FireProjectileCommand(string ownerId, double startX, double startY, double targetX, double targetY, double speed, Action<Projectile> onProjectileCreated)
        {
            _ownerId = ownerId;
            _startX = startX;
            _startY = startY;
            _targetX = targetX;
            _targetY = targetY;
            _speed = speed;
            _onProjectileCreated = onProjectileCreated; 
        }
        public void Execute()
        {
            var projectile = new Projectile(Guid.NewGuid().GetHashCode(), _ownerId, _startX, _startY, _targetX, _targetY, _speed);
            _onProjectileCreated?.Invoke(projectile);
        }
    }
}

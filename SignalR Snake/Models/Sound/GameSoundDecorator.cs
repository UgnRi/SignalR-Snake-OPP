namespace SignalR_Snake.Models.Sound
{
    public abstract class GameSoundDecorator : IGameSound
    {
        protected IGameSound _gameSound;

        public GameSoundDecorator(IGameSound gameSound)
        {
            _gameSound = gameSound;
        }

        public virtual void PlaySound()
        {
            _gameSound?.PlaySound();
        }

        public virtual void StopSound()
        {
            _gameSound?.StopSound();
        }
    }
}
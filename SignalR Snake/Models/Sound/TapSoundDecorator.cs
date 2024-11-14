using NAudio.Wave;
using System.Web;

namespace SignalR_Snake.Models.Sound
{
    public class TapSoundDecorator : GameSoundDecorator
    {
        private AudioFileReader _audioFileReader;
        private WaveOutEvent _outputDevice;
        private string _soundsPath = "~/Assets/Sounds/";

        public TapSoundDecorator(IGameSound gameSound, string soundFile) : base(gameSound)
        {
            _audioFileReader = new AudioFileReader(HttpContext.Current.Server.MapPath(_soundsPath + soundFile));
            _outputDevice = new WaveOutEvent();
            _outputDevice.Init(_audioFileReader);
        }

        public override void PlaySound()
        {
            _gameSound.PlaySound();

            _outputDevice.Play();
        }

        public override void StopSound()
        {
            _outputDevice.Stop();
            _audioFileReader.Position = 0; // Resets tap sound effect to the beginning
        }

        public void StopAllSounds()
        {
            _outputDevice.Stop();
            _audioFileReader.Position = 0; // Resets tap sound effect to the beginning
            base.StopSound(); // Stops the underlying audio component if needed
        }
    }
}
using NAudio.Wave;
using System.Web;

namespace SignalR_Snake.Models.Sound
{
    public class BaseGameSound : IGameSound
    {

        private AudioFileReader _audioFileReader;
        private WaveOutEvent _outputDevice;
        private string _soundsPath = "~/Assets/Sounds/";
        private bool _isLooping = false;

        public BaseGameSound(string soundFile)
        {
            _audioFileReader = new AudioFileReader(HttpContext.Current.Server.MapPath(_soundsPath + soundFile));
            _outputDevice = new WaveOutEvent();
            _outputDevice.Init(_audioFileReader);

            _outputDevice.PlaybackStopped += OnPlaybackStopped;
        }

        public void PlaySound()
        {
            _isLooping = true;
            _outputDevice.Play();
        }

        public void StopSound()
        {
            _outputDevice.Stop();
            _isLooping = false;
            _audioFileReader.Position = 0;
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (_isLooping && _outputDevice.PlaybackState == PlaybackState.Stopped)
            {
                _audioFileReader.Position = 0; // Reset to the beginning
                _outputDevice.Play();          // Play again, creating a loop
            }
        }

    }
}
using Hebron.Runtime;
using Myra.Events;
using Myra.Graphics2D.UI;
using PlMpegNet.Samples.VideoPlayer.MonoGame;
using PlMpegNet.Samples.VideoPlayer.MonoGame.UI;

namespace PlMpegNet.Samples.VideoPlayer.UI
{
	public partial class MainFrame
	{
		private readonly VideoPlayerWidget _videoPlayer;
		private bool _updateSlider = true;

		public Video Video => _videoPlayer.Video;

		public MainFrame()
		{
			BuildUI();

			_videoPlayer = new VideoPlayerWidget
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			_videoPlayer.PositionInSecondsChanged += _videoPlayer_PositionInSecondsChanged;
			_videoPlayer.IsPlayingChanged += (s, e) => UpdateButton();

			_panelVideo.Widgets.Add(_videoPlayer);

			_sliderVolume.ValueChanged += (s, a) => _videoPlayer.Volume = _sliderVolume.Value;
			_sliderTime.ValueChangedByUser += _sliderTime_ValueChangedByUser;

			_buttonPlayStop.Click += (s, a) => _videoPlayer.IsPlaying = !_videoPlayer.IsPlaying;
		}

		private void _videoPlayer_PositionInSecondsChanged(object sender, System.EventArgs e)
		{
			UpdateTimeLabel();
			_labelAllocations.Text = $"Allocations: {PlMpegMemoryStats.Allocations}";

			if (!_updateSlider)
			{
				return;
			}

			var sliderPos = _videoPlayer.PositionInSeconds / _videoPlayer.DurationInSeconds;
			_sliderTime.Value = sliderPos;
		}

		private void _sliderTime_ValueChangedByUser(object sender, ValueChangedEventArgs<float> e)
		{
			var positionInSeconds = _sliderTime.Value * _videoPlayer.DurationInSeconds;

			try
			{
				_updateSlider = false;
				_videoPlayer.PositionInSeconds = positionInSeconds;
			}
			finally
			{
				_updateSlider = true;
			}
		}

		public void LoadVideo(string path)
		{
			_videoPlayer.Load(path);
			UpdateTimeLabel();
		}

		private void UpdateButton()
		{
			_labelPlayStop.Text = _videoPlayer.IsPlaying ? "Stop" : "Play";
		}

		private void UpdateTimeLabel()
		{
			_labelTime.Text = _videoPlayer.PositionInSeconds.FormatTimeInSeconds() + "/" + _videoPlayer.DurationInSeconds.FormatTimeInSeconds();
		}
	}
}
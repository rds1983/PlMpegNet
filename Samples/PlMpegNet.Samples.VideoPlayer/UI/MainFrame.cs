using Hebron.Runtime;
using Myra.Graphics2D.UI;
using PlMpegNet.Samples.VideoPlayer.MonoGame;
using PlMpegNet.Samples.VideoPlayer.MonoGame.UI;

namespace PlMpegNet.Samples.VideoPlayer.UI
{
	public partial class MainFrame
	{
		private readonly VideoPlayerWidget _videoPlayer;

		public float PositionInSeconds
		{
			get => _videoPlayer.PositionInSeconds;

			set
			{
				if (value.EpsilonEquals(_videoPlayer.PositionInSeconds))
				{
					return;
				}

				_videoPlayer.PositionInSeconds = value;
				UpdateTimeLabel();

				var sliderPos = _videoPlayer.PositionInSeconds / _videoPlayer.DurationInSeconds;
				_sliderTime.Value = sliderPos;

				_labelAllocations.Text = $"Allocations: {PlMpegMemoryStats.Allocations}";
			}
		}

		public MainFrame()
		{
			BuildUI();

			_videoPlayer = new VideoPlayerWidget
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			_panelVideo.Widgets.Add(_videoPlayer);

			_sliderVolume.ValueChanged += (s, a) => _videoPlayer.Volume = _sliderVolume.Value;
		}

		public void LoadVideo(string path)
		{
			_videoPlayer.Load(path);
			UpdateTimeLabel();
		}

		private void UpdateTimeLabel()
		{
			_labelTime.Text = _videoPlayer.PositionInSeconds.FormatTimeInSeconds() + "/" + _videoPlayer.DurationInSeconds.FormatTimeInSeconds();
		}
	}
}
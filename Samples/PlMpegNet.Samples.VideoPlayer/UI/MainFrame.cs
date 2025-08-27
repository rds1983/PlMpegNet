using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using PlMpegNet.Samples.VideoPlayer.MonoGame.UI;

namespace PlMpegNet.Samples.VideoPlayer.UI
{
	public partial class MainFrame
	{
		private readonly VideoPlayerWidget _videoPlayer;
		
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
		}

		public void UpdateTime(GameTime gameTime)
		{
			_videoPlayer.UpdateTime(gameTime);
		}
	}
}
using Microsoft.Xna.Framework.Graphics;
using Myra;

namespace PlMpegNet.Samples.VideoPlayer.MonoGame.Effects
{
	public class VideoEffect : Effect
	{
		private readonly EffectParameter _yTexParam;
		private readonly EffectParameter _cbTexParam;
		private readonly EffectParameter _crTexParam;

		public Texture2D YTexture { get; set; }
		public Texture2D CbTexture { get; set; }
		public Texture2D CrTexture { get; set; }

		public VideoEffect() : base(MyraEnvironment.GraphicsDevice, Resources.VideoEffect)
		{
			_yTexParam = Parameters["YTex"];
			_cbTexParam = Parameters["CbTex"];
			_crTexParam = Parameters["CrTex"];
		}

		protected override void OnApply()
		{
			base.OnApply();

			_yTexParam.SetValue(YTexture);
			_cbTexParam.SetValue(CbTexture);
			_crTexParam.SetValue(CrTexture);
		}
	}
}

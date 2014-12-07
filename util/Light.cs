using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Light {

		private const float ratio = 1.0f / 1024.0f;

		private float[] alpha = { 1.0f };
		private Color[] color = { Color.White };
		private float[] radius = { 512.0f };

		private float alphaElapsed, alphaTime = 1.0f;
		private bool alphaFlipped = false;
		private float colorElapsed, colorTime = 1.0f;
		private bool colorFlipped = false;
		private float radiusElapsed, radiusTime = 1.0f;
		private bool radiusFlipped = false;

		public Entity entity;
		private Vector2 offset;

		private int image;

		private float fadeInOutTimer = 0.0f;
		private float fadeIn;

		public float Alpha {
			get {
				var amount = fadeInOutTimer / fadeIn;
				if (alpha.Length == 1) return ((fadeIn == 0) ? alpha[0] : Util.Lerp(0.0f, alpha[0], amount));
				if ((amount == 1) || (fadeIn == 0)) {
					return Util.LerpSet(alphaElapsed / alphaTime, alpha);
				} else {
					return Util.Lerp(0.0f, Util.LerpSet(alphaElapsed / alphaTime, alpha), amount);
				}
			}
		}

		public Color Color {
			get {
				if (color.Length == 1) return color[0]; 
				return BretUtil.LerpColorSet(colorElapsed / colorTime, color);
			}
		}

		public int Image {
			get { return image; }
		}

		public float Radius {
			get {
				var amount = fadeInOutTimer / fadeIn;
				if (radius.Length == 1) return ((fadeIn == 0) ? radius[0] : Util.Lerp(0.0f, radius[0], amount));
				if ((amount == 1) || (fadeIn == 0)) {
					return Util.LerpSet(radiusElapsed / radiusTime, radius);
				} else {
					return Util.Lerp(0.0f, Util.LerpSet(radiusElapsed / radiusTime, radius), amount);
				}
			}
		}

		public float Scale {
			get { return Radius * ratio * 2.0f; }
		}

		public float X {
			get { return entity.X - offset.X; }
		}

		public float Y {
			get { return entity.Y - offset.Y; }
		}

		public Light(int image = 0, float fadeIn = 0f) {
			this.image = image;
			FadeIn(fadeIn);
		}

		public void FadeIn(float fadeIn) {
			Game.Instance.Coroutine.Start(FadeInCoroutine(fadeIn));
		}

		IEnumerator FadeInCoroutine(float fadeIn) {
			this.fadeIn = fadeIn;
			fadeInOutTimer = 0;
			while (fadeInOutTimer < fadeIn) {
				fadeInOutTimer += Game.Instance.RealDeltaTime * 0.001f;
				yield return 0;
			}
			fadeInOutTimer = fadeIn;
		}

		public void FadeOut(float fadeOut) {
			Game.Instance.Coroutine.Start(FadeOutCoroutine(fadeOut));
		}

		IEnumerator FadeOutCoroutine(float fadeOut) {
			fadeInOutTimer = fadeOut;
			fadeIn = fadeOut;
			while (fadeInOutTimer > 0) {
				fadeInOutTimer -= Game.Instance.RealDeltaTime * 0.001f;
				yield return 0;
			}
			fadeInOutTimer = 0;
		}

		public void Update(float deltaTime) {
			alphaElapsed += deltaTime * (alphaFlipped ? -1 : 1);
			colorElapsed += deltaTime * (colorFlipped ? -1 : 1);
			radiusElapsed += deltaTime * (radiusFlipped ? -1 : 1);

			alphaElapsed = ExceededTime(alphaElapsed, alphaTime, ref alphaFlipped);
			colorElapsed = ExceededTime(colorElapsed, colorTime, ref colorFlipped);
			radiusElapsed = ExceededTime(radiusElapsed, radiusTime, ref radiusFlipped);
		}

		public float ExceededTime(float amount, float time, ref bool flip) {
			if (amount > time) {
				flip = true;
				return time;
			}
			if (amount < 0) {
				flip = false;
				return 0;
			}
			return amount;
		}

		public void SetAlpha(params float[] alpha) {
			this.alpha = alpha;
		}

		public void SetAlphaSpan(float time) {
			alphaTime = time;
		}

		public void SetColor(params Color[] color) {
			this.color = color;
		}

		public void SetColorSpan(float time) {
			colorTime = time;
		}

		public void SetRadius(params float[] radius) {
			this.radius = radius;
		}

		public void SetRadiusSpan(float time) {
			radiusTime = time;
		}

		public void SetOffset(Vector2 offset) {
			this.offset = offset;
		}

		public void SetOffset(float x, float y) {
			offset.X = x;
			offset.Y = y;
		}

	}
}

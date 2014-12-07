using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Light {

		private const float ratio = 1.0f / 1024.0f;

		private float startAlpha, endAlpha;
		private Color startColor, endColor;
		private float startRadius, endRadius;

		private float alphaElapsed, alphaTime = 1.0f;
		private bool alphaFlipped = false;
		private float colorElapsed, colorTime = 1.0f;
		private bool colorFlipped = false;
		private float radiusElapsed, radiusTime = 1.0f;
		private bool radiusFlipped = false;

		public Entity entity;
		private Vector2 offset;

		public float Alpha {
			get { return Util.Lerp(startAlpha, endAlpha, alphaElapsed / alphaTime); }
		}

		public Color Color {
			get { return Util.LerpColor(startColor, endColor, colorElapsed / colorTime); }
		}

		public float Radius {
			get { return Util.Lerp(startRadius, endRadius, radiusElapsed / radiusTime); }
		}

		public float Scale {
			get { return Radius * ratio * 2.0f; }
		}

		public float X {
			get { return entity.X; }
		}

		public float Y {
			get { return entity.Y; }
		}

		public Light() {
			startAlpha = endAlpha = 1.0f;
			startColor = endColor = Color.White;
			startRadius = endRadius = 512.0f;
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

		public void SetAlpha(float alpha) {
			SetAlpha(alpha, alpha);
		}

		public void SetAlpha(float alpha1, float alpha2) {
			startAlpha = alpha1;
			endAlpha = alpha2;
		}

		public void SetAlphaSpan(float time) {
			alphaTime = time;
		}

		public void SetColor(Color color) {
			SetColor(color, color);
		}

		public void SetColor(Color color1, Color color2) {
			startColor = color1;
			endColor = color2;
		}

		public void SetColorSpan(float time) {
			colorTime = time;
		}

		public void SetRadius(float radius) {
			SetRadius(radius, radius);
		}

		public void SetRadius(float radius1, float radius2) {
			startRadius = radius1;
			endRadius = radius2;
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

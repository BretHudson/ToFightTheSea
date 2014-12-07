using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Explosion : Entity {

		private const float ratio = 1.0f / 580.0f;

		private float[] alpha = { 1.0f };
		private float[] radius = { 290.0f };

		private float alphaElapsed = 0.0f, alphaTime = 1.0f;
		private float radiusElapsed = 0.0f, radiusTime = 1.0f;

		public float Alpha {
			get {
				if (alpha.Length == 1) return alpha[0];
				return Util.LerpSet(alphaElapsed / alphaTime, alpha);
			}
		}

		public float Radius {
			get {
				if (radius.Length == 1) return radius[0];
				return Util.LerpSet(radiusElapsed / radiusTime, radius);
			}
		}

		public float Scale {
			get { return Radius * ratio * 2.0f; }
		}

		public Explosion(float x, float y) : base(x, y) {
			//
		}

		public override void Update() {
			var deltaTime = Game.RealDeltaTime * 0.001f;
			alphaElapsed += deltaTime;
			radiusElapsed += deltaTime;

			var done = -1;

			if (alphaElapsed > alphaTime) {
				alphaElapsed = alphaTime;
				++done;
			}

			if (radiusElapsed > radiusTime) {
				radiusElapsed = radiusTime;
				++done;
			}

			if (done == 1) {
				RemoveSelf();
			}
		}

		public override void Render() {
			Global.shockwave.Alpha = Alpha;
			Global.shockwave.Scale = Scale;
			Global.shaderSurface.Draw(Global.shockwave, X, Y);
		}

		public void SetAlpha(float time, params float[] alpha) {
			alphaTime = time;
			this.alpha = alpha;
		}

		public void SetRadius(float time, params float[] radius) {
			radiusTime = time;
			this.radius = radius;
		}

	}
}

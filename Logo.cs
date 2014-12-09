using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Logo : Entity {

		private Image sprite;

		private float baseY;
		private float floatDirection = -1;
		private float floatVelocity = -1;
		private Range floatSpeed = new Range(0.35f, 0.6f);
		private float maxDistance = 20.0f;

		private float lerpSpeed = 0.01f;

		public Logo(float X, float Y, string filename, float maxDistance = 20.0f) : base(X, Y) {
			baseY = Y;

			this.maxDistance = maxDistance;

			//sprite = Image.CreateRectangle(100, 50);
			sprite = new Image(filename);
			sprite.CenterOrigin();
			AddGraphic(sprite);

			lerpSpeed += Rand.Float(0.0f, 0.01f);

			PreloadFloat();
		}

		public override void Update() {
			Float();
		}

		private void PreloadFloat() {
			var num = Rand.Int(250, 500);
			for (int i = 0; i < num; ++i) {
				Float();
			}
		}

		private void Float() {
			Y += Rand.Float(floatSpeed) * floatVelocity;
			floatVelocity = Util.Lerp(floatVelocity, floatDirection, lerpSpeed);
			var difference = Y - baseY;
			if (Math.Abs(difference) > maxDistance) {
				if (Math.Sign(difference) == floatDirection) {
					floatDirection *= -1;
				}
			}
		}

	}
}

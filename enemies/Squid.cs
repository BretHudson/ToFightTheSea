using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Squid : Enemy {

		private Image sprite = new Image("assets/gfx/squeed.png");

		private Light inkLight;

		public Squid(float x, float y) : base(x, y, 10, 0.5f) {
			Graphic = sprite;
			sprite.CenterOrigin();
		}

		public override void Added() {
			base.Added();

			// Create the squid's glow light
			light = new Light();
			light.SetAlpha(0.7f);
			light.SetColor(new Color("EF473E"));
			light.SetColorSpan(5.0f);
			light.SetRadius(sprite.Height + 30);
			light.entity = this;
			Level.lights.Add(light);

			// Create the squid's black light stuff
			inkLight = new Light();
			inkLight.SetAlpha(0.0f);
			inkLight.SetColor(Color.Black);
			inkLight.SetColorSpan(5.0f);
			inkLight.SetRadius(sprite.Height + 400);
			inkLight.entity = this;
			Level.darkness.Add(inkLight);

			Game.Coroutine.Start(Ink());
		}

		public override void Update() {
			base.Update();

			sprite.Angle = OrientCircularSprite();
		}

		private IEnumerator Ink() {
			yield return 0;
			// TODO: Create a little squid animation where he shits
			inkLight.SetAlpha(1.0f);
			inkLight.FadeIn(0.7f);
		}

		protected override IEnumerator Death() {
			yield return base.Death();
			Level.darkness.Remove(inkLight);
		}

	}
}

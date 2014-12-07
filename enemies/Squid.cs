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
			light.SetRadius(sprite.Height + 30);
			light.entity = this;
			Level.lights.Add(light);

			//Game.Coroutine.Start(Ink());
		}

		public override void Update() {
			sprite.Angle = OrientCircularSprite();
		}

		private IEnumerator Ink() {
			// TODO: Create a little squid animation where he shits
			yield return Coroutine.Instance.WaitForSeconds(1.0f);

			inkLight = new Light(0, 0.7f);
			inkLight.SetAlpha(1.0f);
			inkLight.SetColor(Color.Black);
			inkLight.SetRadius(sprite.Height + 400);
			inkLight.entity = this;
			Level.darkness.Add(inkLight);
			yield return 0;

			float length = 10.0f;
			for (float timer = 0; timer < length; timer += Game.RealDeltaTime * 0.001f) {
				// TODO: Put out little "explosions"
				yield return 0;
			}

			inkLight.FadeOut(10.0f);
		}

		protected override IEnumerator Death() {
			yield return base.Death();
			Level.darkness.Remove(inkLight);
		}

	}
}

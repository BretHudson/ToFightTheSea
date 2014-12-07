using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class LightningArea : Projectile {

		public LightningArea(float x, float y, Vector2 direction) : base(x, y, direction, 0.1f, 100, 1.7f, (int)Tags.PROJECTILE, (int)Tags.PLAYERATTACK) {
			

			light = new Light();
			light.SetAlpha(0.5f, 0.7f);
			light.SetAlphaSpan(0.3f);
			light.SetColor(new Color("FFD700"));
			light.SetColorSpan(5.0f);
			light.SetRadius(300, 355);
			light.SetRadiusSpan(0.07f);
			light.entity = this;
			Level.lights.Add(light);
		}

		protected override IEnumerator Explosion() {
			// TODO: this
			return base.Explosion();
		}

	}
}

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Squid : Enemy {

		private Image sprite = new Image("assets/gfx/squeed.png");

		public Squid(float x, float y) : base(x, y, 10, 0.5f) {
			Graphic = sprite;
			sprite.CenterOrigin();
		}

		public override void Added() {
			base.Added();

			light = new Light();
			light.SetAlpha(0.7f);
			light.SetColor(new Color("EF473E"));
			light.SetColorSpan(5.0f);
			light.SetRadius(sprite.Height + 30);
			light.entity = this;
			Level.lights.Add(light);
		}

	}
}

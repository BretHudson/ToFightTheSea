using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class LightningBall : Projectile {

		private Image sprite = Image.CreateCircle(10, Color.FromHSV(0.166f, 0.40f, 1.0f, 1.0f));

		public LightningBall(float x, float y, Vector2 direction) : base(x, y, direction, 12.0f, new Vector2(20, 20), 1.3f, (int)Tags.PROJECTILE, (int)Tags.PLAYERATTACK) {
			sprite.CenterOrigin();
			Graphic = sprite;
		}

		public override void Render() {
			base.Render();
		}

		protected override IEnumerator Explosion() {
			return base.Explosion();
		}

	}
}

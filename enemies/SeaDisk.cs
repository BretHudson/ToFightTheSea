using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class SeaDisk : Enemy {

		private Image sprite = Image.CreateCircle(30);
		//private Image armSprite

		public SeaDisk(float x, float y) : base(x, y, 20, 5.0f) {
			sprite.CenterOrigin();
			Graphic = sprite;

			SetHitbox(1, 1, 1);
		}

	}
}

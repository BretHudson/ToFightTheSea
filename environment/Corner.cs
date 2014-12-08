using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Corner : Entity {

		public static Vector2 size = new Vector2(190, 160);

		public Corner(int x, int y) : base(x + size.X * 0.5f, y + size.Y * 0.5f) {
			SetHitbox((int)size.X + 80, (int)size.Y + 80, (int)Tags.SOLID);
			Hitbox.CenterOrigin();
		}

		public override void Render() {
			base.Render();
		}

	}
}

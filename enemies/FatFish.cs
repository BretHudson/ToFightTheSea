using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class FatFish : Enemy {

		private Vector2 waypoint;

		private Image sprite = new Image("assets/gfx/moonfish.png");

		private float timeout = 0.0f;

		public FatFish(float x, float y, bool baby = false) : base(x, y, 10, 0.5f) {
			if (baby) {
				// TODO: Set scale
			}

			sprite.CenterOrigin();
			Graphic = sprite;

			Collider = new CircleCollider(50, (int)Tags.ENEMY, (int)Tags.FATFISH);
			Collider.CenterOrigin();

			// Initilize velocity values
			velocity = new Vector2(0, 1);
			velocity = Util.Rotate(velocity, Rand.Int(0, 360));
			minspeed = 2.0f;
			sprite.Angle = Util.RAD_TO_DEG * (float)Math.Atan2(-velocity.Y, velocity.X) - 90;
			direction = velocity;
		}

		public override void Update() {
			base.Update();

			sprite.Alpha = alpha;

			timeout = Rand.Float(0.8f, 2.0f);

			if (timeout <= 0.0f)
				GenerateWaypoint();

			var toTarget = waypoint - new Vector2(X, Y);
			var newAngle = Util.RAD_TO_DEG * (float)Math.Atan2(-toTarget.Y, toTarget.X);
			var angleDiff = ((((newAngle - sprite.Angle) % 360) + 540) % 360) - 180;
			var rotateAmount = Util.Clamp(angleDiff, -dirStepAmount, dirStepAmount);
			velocity = Util.Rotate(velocity, rotateAmount);
			sprite.Angle = (float)Math.Atan2(-velocity.Y, velocity.X) * Util.RAD_TO_DEG;
		}

		private void GenerateWaypoint() {
			waypoint = new Vector2(Rand.Float(300.0f, 1620.0f), Rand.Float(250.0f, 830.0f));
		}

	}
}

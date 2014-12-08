using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class FlyingGurnard : Enemy {

		enum AnimType {
			Idle
		}

		private Spritemap<AnimType> sprite = new Spritemap<AnimType>("assets/gfx/wingwang.png", 425, 696);
		private float floatDirection = -1;
		private Range floatSpeed = new Range(0.4f, 0.7f);
		private float maxDistance = 20.0f;

		private Light eye1, eye2;
		private Vector2 eye1offset, eye2offset;

		private Entity target;

		public FlyingGurnard(Entity target) : base(960, 540, 100, 10.0f) {
			// Initialize sprite
			sprite.Add(AnimType.Idle, new Anim(new int[] { 0, 1, 2, 3, 4, 5, }, new float[] { 6.0f }));
			sprite.Play(AnimType.Idle);
			sprite.CenterOrigin();
			Graphic = sprite;

			// Set the target
			this.target = target;

			// Set up hitbox
			SetHitbox(60, 60, (int)Tags.ENEMY);

			// Lights
			eye1 = new Light();
			eye1.SetAlpha(0.6f);
			eye1.SetRadius(100.0f);
			eye1offset = new Vector2(-170, 50);
			eye1.SetOffset(eye1offset);
			eye1.entity = this;
			Level.lights.Add(eye1);

			eye2 = new Light();
			eye2.SetAlpha(0.6f);
			eye2.SetRadius(100.0f);
			eye2offset = new Vector2(-170, -15);
			eye2.SetOffset(eye2offset);
			eye2.entity = this;
			Level.lights.Add(eye2);

			direction = new Vector2(0, 1);
		}

		public override void Update() {
			// Float
			Y += Rand.Float(floatSpeed) * floatDirection;
			if (Math.Abs(Y - 540) > maxDistance) {
				Y = maxDistance * Math.Sign(Y - 540) + 540;
				floatDirection *= -1;
				// TODO: Make this more smooth
			}

			// TODO: Find a good way to rotate offsets and lights and stuff

			// TODO: Get this shit to lerp
			Vector2 toTarget = new Vector2(target.X, target.Y) - new Vector2(X, Y);
			var newAngle = Util.RAD_TO_DEG * (float)Math.Atan2(-toTarget.Y, toTarget.X);
			var angleDiff = ((((newAngle - sprite.Angle) % 360) + 540) % 360) - 180;
			var rotateAmount = Util.Clamp(angleDiff, -dirStepAmount, dirStepAmount);
			direction = Util.Rotate(direction, rotateAmount);
			sprite.Angle = (float)Math.Atan2(-direction.Y, direction.X) * Util.RAD_TO_DEG;

			base.Update();
		}

	}
}

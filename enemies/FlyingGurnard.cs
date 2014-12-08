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
		private float floatVelocity = -1;
		private Range floatSpeed = new Range(0.35f, 0.6f);
		private float maxDistance = 20.0f;

		private Light eye1, eye2;
		private Vector2 eye1offset, eye2offset;

		public FlyingGurnard(Entity target) : base(960, 540, 100, 10.0f) {
			// Initialize sprite
			sprite.Add(AnimType.Idle, new Anim(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, new float[] { 4.0f }));
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
			
			// Reset direction to right
			direction = new Vector2(0, 1);
		}

		public override void Update() {
			// Float
			Y += Rand.Float(floatSpeed) * floatVelocity;
			floatVelocity = Util.Lerp(floatVelocity, floatDirection, 0.01f);
			var difference = Y - 540;
			if (Math.Abs(difference) > maxDistance) {
				if (Math.Sign(difference) == floatDirection) {
					floatDirection *= -1;
				}
			}

			// TODO: Find a good way to rotate offsets and lights and stuff

			// TODO: Get this shit to lerp
			Vector2 toTarget = new Vector2(target.X, target.Y) - new Vector2(X, Y);
			var newAngle = Util.RAD_TO_DEG * (float)Math.Atan2(-toTarget.Y, toTarget.X);
			var angleDiff = ((((newAngle - sprite.Angle) % 360) + 540) % 360) - 180;
			var rotateAmount = Util.Clamp(angleDiff, -dirStepAmount, dirStepAmount);
			direction = Util.Rotate(direction, rotateAmount);
			sprite.Angle = (float)Math.Atan2(-direction.Y, direction.X) * Util.RAD_TO_DEG;

			setOffset(ref eye1, eye1offset);
			setOffset(ref eye2, eye2offset);

			base.Update();
		}

		private void setOffset(ref Light light, Vector2 offset) {
			light.SetOffset(Util.Rotate(offset, sprite.Angle));
		}

	}
}

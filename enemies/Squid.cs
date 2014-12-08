using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Squid : Enemy {

		enum AnimType {
			Go, Ink
		};

		private Spritemap<AnimType> sprite = new Spritemap<AnimType>("assets/gfx/squeedsquad.png", 75, 179);

		private bool avoid = false;

		private float friction = 0.25f;
		private float aspeed = 1.5f;

		public Squid(float x, float y) : base(x, y, 10, 0.5f) {
			Graphic = sprite;
			sprite.CenterOrigin();

			SetHitbox(20, 80, (int)Tags.ENEMY);

			// Set up sprite
			sprite.Add(AnimType.Go, new Anim(new int[] { 1, 2, 3, 4, 0, }, new float[] { 7.0f }));
			sprite.Add(AnimType.Ink, new Anim(new int[] { 5, 6, 7, 8, 9, }, new float[] { 4.0f }));
			sprite.Play(AnimType.Go);

			// Initilize velocity values
			velocity = new Vector2(0, 1);
			velocity = Util.Rotate(velocity, Rand.Int(0, 360));
			minspeed = 2.0f;
			sprite.Angle = Util.RAD_TO_DEG * (float)Math.Atan2(-velocity.Y, velocity.X) - 90;
			direction = velocity;
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

			// TODO: Contact damage
			
		}

		public override void Update() {
			base.Update();

			// Apply friction
			var magnitude = velocity.Length;
			magnitude = Math.Max(magnitude - friction, minspeed);

			if ((sprite.CurrentFrame == 4) && (sprite.CurrentAnim == AnimType.Go)) {
				magnitude = Util.Approach(magnitude, maxspeed, aspeed);
			}

			velocity.Normalize();
			velocity *= magnitude;

			// Rotate towards target
			Vector2 toTarget;
			if (!avoid) {
				toTarget = GetTargetPos() - new Vector2(X, Y);
			} else {
				// TODO: Enhance this a bit
				toTarget = new Vector2(X, Y) - GetTargetPos();
			}

			var newAngle = Util.RAD_TO_DEG * (float)Math.Atan2(-toTarget.Y, toTarget.X);
			var angleDiff = ((((newAngle - sprite.Angle - 90) % 360) + 540) % 360) - 180;
			var rotateAmount = Util.Clamp(angleDiff, -dirStepAmount, dirStepAmount);
			velocity = Util.Rotate(velocity, rotateAmount);
			sprite.Angle = (float)Math.Atan2(-velocity.Y, velocity.X) * Util.RAD_TO_DEG - 90;

			if (Input.KeyPressed(Key.Space)) {
				Game.Coroutine.Start(Ink());
			}
		}

		private IEnumerator Ink() {
			// Create dat ink
			Scene.Add(new Ink(X, Y, sprite.Angle));
			dirStepAmount = 0.0f;
			yield return 0;

			sprite.Play(AnimType.Ink);
			minspeed = 0.5f;
			yield return Coroutine.Instance.WaitForFrames(20);

			dirStepAmount = 5.0f;
			minspeed = 2.0f;
			sprite.Play(AnimType.Go);
		}

		protected override IEnumerator Death() {
			yield return base.Death();
		}

	}
}

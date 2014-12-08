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

		enum PoofAnimType {
			Go
		};

		private Spritemap<AnimType> sprite = new Spritemap<AnimType>("assets/gfx/squeedsquad.png", 75, 179);
		private Spritemap<PoofAnimType> poof = new Spritemap<PoofAnimType>("assets/gfx/poof.png", 60, 76);

		private Light inkLight;

		public Entity target;
		private bool avoid = false;

		private float friction = 0.25f;
		private float aspeed = 1.5f;

		public Squid(float x, float y) : base(x, y, 10, 0.5f) {
			Graphic = sprite;
			sprite.CenterOrigin();

			SetHitbox(20, 80, (int)Tags.ENEMY);

			// Set up sprite
			sprite.Add(AnimType.Go, new Anim(new int[] { 0, 1, 2, 3, 4, }, new float[] { 7.0f }));
			sprite.Add(AnimType.Ink, new Anim(new int[] { 5, 6, 7, 8, 9, }, new float[] { 4.0f }));
			sprite.Play(AnimType.Go);

			// Initilize velocity values
			velocity = new Vector2(0, 1);
			velocity = Util.Rotate(velocity, Rand.Int(0, 360));
			minspeed = 2.0f;
			sprite.Angle = Util.RAD_TO_DEG * (float)Math.Atan2(-velocity.Y, velocity.X) - 90;
			direction = velocity;

			// Initilize poof
			poof.Add(PoofAnimType.Go, new Anim(new int[] { 0, 1, 2 }, new float[] { 4.0f }));
			poof.Play(PoofAnimType.Go);
			poof.CenterOrigin();
			//AddGraphic(poof);
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
			base.Update();

			// dirStepAmount

			// Apply friction
			var magnitude = velocity.Length;
			magnitude = Math.Max(magnitude - friction, minspeed);

			if ((sprite.CurrentFrame == 0) && (sprite.CurrentAnim == AnimType.Go)) {
				magnitude = Util.Approach(magnitude, maxspeed, aspeed);
			}

			velocity.Normalize();
			velocity *= magnitude;

			// Rotate towards target
			if (!avoid) {
				Vector2 toTarget = new Vector2(target.X, target.Y) - new Vector2(X, Y);
				var newAngle = Util.RAD_TO_DEG * (float)Math.Atan2(-toTarget.Y, toTarget.X);
				var angleDiff = ((((newAngle - sprite.Angle - 90) % 360) + 540) % 360) - 180;
				var rotateAmount = Util.Clamp(angleDiff, -dirStepAmount, dirStepAmount);
				velocity = Util.Rotate(velocity, rotateAmount);
				/*float mag = velocity.Length;
				velocity = toTarget;
				velocity.Normalize();
				velocity *= mag;*/
				sprite.Angle = (float)Math.Atan2(-velocity.Y, velocity.X) * Util.RAD_TO_DEG - 90;
				/*if ((Math.Abs(acceleration.X) > 0.0f) || (Math.Abs(acceleration.Y) > 0.0f)) {
					var newAngle = Util.RAD_TO_DEG * (float)Math.Atan2(-acceleration.Y, acceleration.X);
					var angleDiff = ((((newAngle - angle) % 360) + 540) % 360) - 180;
					var rotateAmount = Util.Clamp(angleDiff, -dirStepAmount, dirStepAmount);
					direction = Util.Rotate(direction, rotateAmount);
					angle = (float)Math.Atan2(-direction.Y, direction.X) * Util.RAD_TO_DEG;
				}*/
			} else {
				// TODO: 
			}
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

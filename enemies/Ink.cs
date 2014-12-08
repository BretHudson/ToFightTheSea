using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Ink : Entity {

		enum AnimType {
			Go
		};

		private Spritemap<AnimType> sprite = new Spritemap<AnimType>("assets/gfx/poof.png", 60, 76);
		
		private Light light;

		private Vector2 offset = new Vector2(8, 0);

		private Sound snd = new Sound("assets/sfx/ink_noise.wav");

		public Ink(float x, float y, float angle) : base(x, y) {
			// Init sprite
			sprite.Add(AnimType.Go, new Anim(new int[] { 0, 1, 2 }, new float[] { 8.0f }));
			sprite.Play(AnimType.Go);
			sprite.CenterOrigin();
			sprite.Angle = angle;
			offset = Util.Rotate(offset, sprite.Angle);
			sprite.OriginX += offset.X;
			sprite.OriginY += offset.Y;
			offset.Normalize();
			offset *= 2.7f;
			Graphic = sprite;

			// Set up collider
			Collider = new CircleCollider(250, (int)Tags.INK);
			Collider.CenterOrigin();

			// Get that sound
			snd.Volume = 0.01f;
			snd.Play();
		}

		public override void Added() {
			base.Added();

			Game.Coroutine.Start(EnjoyLife());
		}

		public override void Update() {
			if (sprite.CurrentFrame == 2) {
				sprite.FreezeFrame(2);
			}

			sprite.OriginX += offset.X;
			sprite.OriginY += offset.Y;
		}

		private IEnumerator EnjoyLife() {
			sprite.Alpha -= 0.2f;
			yield return Coroutine.Instance.WaitForFrames(4);

			light = new Light(0, 0.7f);
			light.SetAlpha(1.0f);
			light.SetColor(Color.Black);
			light.SetRadius(650);
			light.entity = this;
			Level.darkness.Add(light);

			float length = 10.0f;
			for (float timer = 0; timer < length; timer += Game.RealDeltaTime * 0.001f) {
				// TODO: Put out little "explosions"
				sprite.Alpha -= 0.01f;
				yield return 0;
			}

			light.FadeOut(10.0f);
			yield return Coroutine.Instance.WaitForSeconds(10.0f);

			RemoveSelf();
		}

	}
}

using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Projectile : Entity {

		private Vector2 velocity;
		private float lifespan;
		private bool destroyed = false;

		public Projectile(float x, float y, Vector2 direction, float speed, Vector2 size, float lifespan, params int[] tags) : base(x, y) {
			direction.Normalize();
			this.velocity = direction * speed;
			this.lifespan = lifespan;
			SetHitbox((int)size.X, (int)size.Y, tags);
			Hitbox.CenterOrigin();
		}

		public override void Added() {
			Game.Coroutine.Start(BeginDeath());
		}

		IEnumerator BeginDeath() {
			yield return Coroutine.Instance.WaitForSeconds(lifespan);
			if (!destroyed)
				Game.Coroutine.Start(Destroy());
		}

		public override void Update() {
			if (!destroyed) {
				X += velocity.X;
				Y += velocity.Y;

				if (Collide(X, Y, (int)Tags.SOLID) != null) {
					Game.Coroutine.Start(Destroy());
				}
			}

			Wrap();
		}

		IEnumerator Destroy() {
			if (!destroyed) {
				SetHitbox(0, 0, (int)Tags.PROJECTILE);
				destroyed = true;
			}

			yield return Explosion();

			RemoveSelf();
		}

		virtual protected IEnumerator Explosion() {
			yield return 0;
		}

		void Wrap() {
			var left = 0 - ((int)Hitbox.Width >> 1);
			var right = 1920 + ((int)Hitbox.Width >> 1);

			var top = 0 - ((int)Hitbox.Height >> 1);
			var bottom = 1080 + ((int)Hitbox.Height >> 1);

			if (X < left) {
				X = right;
			} else if (X > right) {
				X = left;
			}

			if (Y < top) {
				Y = bottom;
			} else if (Y > bottom) {
				Y = top;
			}
		}

	}
}

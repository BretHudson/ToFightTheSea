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

		protected Light light = null;

		public int damage;

		public Projectile(float x, float y, Vector2 direction, float speed, int radius, float lifespan, int damage, params int[] tags) : base(x, y) {
			direction.Normalize();
			this.velocity = direction * speed;
			this.lifespan = lifespan;
			Collider = new CircleCollider(radius, tags);
			Collider.CenterOrigin();
			this.damage = damage;
		}

		public override void Added() {
			Game.Coroutine.Start(BeginDeath());
		}

		IEnumerator BeginDeath() {
			yield return Coroutine.Instance.WaitForFrames((int)(lifespan * 60));
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

		virtual public void HitEnemy() {
			Game.Coroutine.Start(Destroy());
		}

		/*public override void Render() {
			base.Render();
			Collider.Render();
		}*/

		IEnumerator Destroy() {
			if (!destroyed) {
				SetHitbox(0, 0, (int)Tags.PROJECTILE);
				destroyed = true;
			}

			yield return Explosion();

			if (light != null) {
				Level.lights.Remove(light);
				light = null;
			}

			RemoveSelf();
		}

		virtual protected IEnumerator Explosion() {
			yield return 0;
		}

		void Wrap() {
			var left = 0 - (Collider.Width * 0.95f);
			var right = 1920 + (Collider.Width * 0.95f);

			var top = 0 - (Collider.Height * 0.95f);
			var bottom = 1080 + (Collider.Height * 0.95f);

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

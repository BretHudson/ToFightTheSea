using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Enemy : Entity {

		protected int health;
		private float cooldownTimer;
		private bool hurt = false;

		protected Vector2 acceleration = Vector2.Zero;
		protected Vector2 velocity = Vector2.Zero;
		protected float maxspeed = 5.0f;
		protected float minspeed = 0.0f;

		protected Vector2 direction = new Vector2(0, -1);
		private float angle;
		protected float dirStepAmount = 5.0f;

		public Entity target;

		protected Light light;

		public Enemy(float x, float y, int health, float cooldown) : base(x, y) {
			this.health = health;
			cooldownTimer = cooldown;
		}

		public override void Update() {
			velocity.X += acceleration.X;
			velocity.Y += acceleration.Y;

			var magnitude = Util.Clamp(velocity.Length, minspeed, maxspeed);
			if (magnitude != velocity.Length) {
				velocity.Normalize();
				velocity *= magnitude;
			}

			X += velocity.X;
			Y += velocity.Y;

			CheckCollisions();
			Wrap();
			if (health > 0)
				CheckCollisions();
		}

		public void Kill() {
			ApplyDamage(1000);
		}

		public Vector2 GetTargetPos() {
			if (target != null) {
				Vector2 targetPos = new Vector2(target.X, target.Y);

				if (Math.Abs(targetPos.X - X) > 1000) {
					targetPos.X -= Math.Sign(targetPos.X - X) * 2000;
				}

				if (Math.Abs(targetPos.Y - Y) > 600) {
					targetPos.Y -= Math.Sign(targetPos.Y - Y) * 1200;
				}

				return targetPos;
			}
			return Vector2.Zero;
		}

		public void ApplyDamage(int damage) {
			if (!hurt) {
				health -= damage;
				if (health > 0) {
					Game.Coroutine.Start(DamageCooldown());
				} else {
					Game.Coroutine.Start(Die());
				}
			}
		}

		IEnumerator DamageCooldown() {
			hurt = true;
			yield return Coroutine.Instance.WaitForFrames((int)(cooldownTimer * 60));
			hurt = false;
		}

		virtual protected IEnumerator Death() {
			yield return 0;
		}

		IEnumerator Die() {
			Hitbox.RemoveTag((int)Tags.ENEMYATTACK);
			yield return Death();
			RemoveSelf();
		}

		void CheckCollisions() {
			var attack = Collide(X, Y, (int)Tags.PLAYERATTACK);
			if (attack != null) {
				var e = attack.Entity as Projectile;
				e.HitEnemy();
				ApplyDamage(e.damage);
				// Let attack know it's hit something
			}
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

		protected float OrientCircularSprite() {
			Console.WriteLine(acceleration);

			if ((Math.Abs(acceleration.X) > 0.0f) || (Math.Abs(acceleration.Y) > 0.0f)) {
				var newAngle = Util.RAD_TO_DEG * (float)Math.Atan2(-acceleration.Y, acceleration.X);
				var angleDiff = ((((newAngle - angle) % 360) + 540) % 360) - 180;
				var rotateAmount = Util.Clamp(angleDiff, -dirStepAmount, dirStepAmount);
				direction = Util.Rotate(direction, rotateAmount);
				angle = (float)Math.Atan2(-direction.Y, direction.X) * Util.RAD_TO_DEG;
			}

			return angle;
		}

	}
}

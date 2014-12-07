using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Enemy : Entity {

		private int health;
		private float cooldownTimer;
		private bool hurt = false;

		protected Light light;

		public Enemy(float x, float y, int health, float cooldown) : base(x, y) {
			this.health = health;
			cooldownTimer = cooldown;
		}

		public void Kill() {
			ApplyDamage(1000);
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
			yield return Coroutine.Instance.WaitForSeconds(cooldownTimer);
			hurt = false;
		}

		virtual protected IEnumerator Death() {
			yield return 0;
		}

		IEnumerator Die() {
			SetHitbox(0, 0, -1);
			yield return Death();
			RemoveSelf();
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

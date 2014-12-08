using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class SeaLink : Enemy {

		private SeaLink head = null;
		private SeaLink child = null;

		private float speed = 3.0f;

		private float babies = 0;
		private CircleCollider personalSpace;
		private int spaceAmount = 24;

		private Image sprite = new Image("assets/gfx/salp.png");

		private float angleOffset = 130.0f;

		// TODO: Figure out light stuffs

		public SeaLink(float x, float y, float babies, Vector2 offset) : base(x - offset.X, y - offset.Y, 4, 1.0f) {
			// Initialize sprite
			sprite.CenterOrigin();
			Graphic = sprite;

			this.babies = babies;

			dirStepAmount = 1.3f;

			// Set up colliders
			SetHitbox(28, 28, (int)Tags.ENEMY);
			personalSpace = new CircleCollider((int)(spaceAmount * 0.5), (int)Tags.SEALINK, (int)Tags.ENEMYATTACK);
			AddCollider(personalSpace);

			if (offset == Vector2.Zero) {
				velocity = new Vector2(0, speed);
				velocity = Util.Rotate(velocity, Rand.Angle);
			} else {
				velocity = offset;
				velocity.Normalize();
				velocity = Util.Rotate(velocity, 4);
			}
			
			sprite.Scale = 0.1f;
		}

		public override void Added() {
			Game.Coroutine.Start(AddBaby());
		}

		private IEnumerator AddBaby() {
			yield return Coroutine.Instance.WaitForFrames(1);

			if (babies > 0) {
				velocity.Normalize();
				child = Scene.Add(new SeaLink(X, Y, babies - 1, velocity * (spaceAmount / 2)));
				child.head = this;
				child.target = this;
				EnemySpawner.Instance.AddEnemy(child);
			} else {
				var explosion = Scene.Add(new Explosion(X, Y));
				explosion.SetAlpha(3.0f, 0.5f, 1.0f, 0.8f, 0.5f);
				explosion.SetRadius(3.0f, 20.0f, 100.0f);
			}

			while (sprite.ScaleX < 1.0f) {
				sprite.Scale = sprite.ScaleX + 0.03f;
				yield return 0;
			}

			sprite.Scale = 1.0f;
		}

		public override void Update() {
			if (target == null) {
				if (head != null) {
					target = head;
				} else {
					target = Scene.GetEntity<Player>();
				}
			}

			if (head == null) {
				Vector2 toTarget = GetTargetPos() - new Vector2(X, Y);
				var newAngle = Util.RAD_TO_DEG * (float)Math.Atan2(-toTarget.Y, toTarget.X);
				var angleDiff = ((((newAngle - sprite.Angle - angleOffset) % 360) + 540) % 360) - 180;
				var rotateAmount = Util.Clamp(angleDiff, -dirStepAmount, dirStepAmount);
				velocity = Util.Rotate(velocity, rotateAmount);
				sprite.Angle = (float)Math.Atan2(-velocity.Y, velocity.X) * Util.RAD_TO_DEG - angleOffset;
			} else {
				velocity = GetTargetPos() - new Vector2(X, Y);
				sprite.Angle = (float)Math.Atan2(-velocity.Y, velocity.X) * Util.RAD_TO_DEG - angleOffset;
			}
			
			velocity.Normalize();
			velocity *= speed;
			//velocity = Vector2.Zero;

			base.Update();

			if (health <= 0)
				return;

			if (head == null) {
				
			} else if (personalSpace.Overlap(X, Y, head.personalSpace)) {
				Vector2 direction = GetTargetPos() - new Vector2(X, Y);
				direction.Normalize();
				direction *= spaceAmount;
				X = GetTargetPos().X - direction.X;
				Y = GetTargetPos().Y - direction.Y;
			}
		}

		protected override IEnumerator Death() {
			if (child != null) {
				child.head = null;
				child.target = null;
			}
			if (head != null) {
				head.child = null;
			}
			personalSpace.RemoveTag((int)Tags.SEALINK);
			personalSpace = null;
			yield return base.Death();
		}

	}
}

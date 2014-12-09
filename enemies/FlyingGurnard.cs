using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class FlyingGurnard : Enemy {

		enum State {
			Idle, Swim,
		}

		private State curState = State.Idle;

		private Image sprite = new Image("assets/gfx/wingwang.png");
		private float floatDirection = -1;
		private float floatVelocity = -1;
		private Range floatSpeed = new Range(0.35f, 0.6f);
		private float maxDistance = 20.0f;

		private float scaleDirection = -1;
		private float scaleVelocity = -1;
		private Range scaleSpeed = new Range(0.0035f, 0.006f);
		private float maxScaleDistance = 0.025f;

		private Light eye1, eye2;
		private Vector2 eye1offset, eye2offset;

		private Light softlight1, softlight2;
		
		private Vector2 hitboxOffset = new Vector2(-40, 0);
		private Vector2 softSpot1Offset = new Vector2(30, 130);
		private Vector2 softSpot2Offset = new Vector2(30, -130);
		private Vector2 repelOffset = new Vector2(80, 0);

		private CircleCollider softSpot1 = new CircleCollider(115, -1);
		private CircleCollider softSpot2 = new CircleCollider(115, -1);
		private CircleCollider repelCollider = new CircleCollider(270, (int)Tags.REPEL);

		public FlyingGurnard(Entity target) : base(960, 540, 20, 10.0f) {
			// Initialize sprite
			sprite.CenterOrigin();
			Graphic = sprite;

			// Set the target
			this.target = target;

			// Set up hitbox (actually realized with the repel collider this hitbox is useless)
			SetHitbox(200, 200, -1);
			Hitbox.CenterOrigin();
			Hitbox.OriginX += hitboxOffset.X;
			Hitbox.OriginY += hitboxOffset.Y;

			// Make it so the player can't touch the enemy
			repelCollider.CenterOrigin();
			AddCollider(repelCollider);

			// Set up soft spots
			softSpot1.CenterOrigin();
			softSpot1.OriginX += softSpot1Offset.X;
			softSpot1.OriginY += softSpot1Offset.Y;
			
			softSpot2.CenterOrigin();
			softSpot2.OriginX += softSpot2Offset.X;
			softSpot2.OriginY += softSpot2Offset.Y;

			AddColliders(softSpot1, softSpot2);

			softlight1 = new Light();
			softlight1.SetAlpha(0.0f);
			softlight1.SetColor(new Color("C42679"));
			softlight1.SetRadius(300.0f);
			softlight1.SetOffset(softSpot1Offset);
			softlight1.entity = this;
			Level.lights.Add(softlight1);

			softlight2 = new Light();
			softlight2.SetAlpha(0.0f);
			softlight2.SetColor(new Color("C42679"));
			softlight2.SetRadius(300.0f);
			softlight2.SetOffset(softSpot2Offset);
			softlight2.entity = this;
			Level.lights.Add(softlight2);

			// Lights
			eye1 = new Light(0, 4.0f);
			eye1.SetAlpha(0.6f);
			eye1.SetRadius(100.0f);
			eye1offset = new Vector2(-170, 50);
			eye1.SetOffset(eye1offset);
			eye1.entity = this;
			Level.lights.Add(eye1);

			eye2 = new Light(0, 4.0f);
			eye2.SetAlpha(0.6f);
			eye2.SetRadius(100.0f);
			eye2offset = new Vector2(-170, -15);
			eye2.SetOffset(eye2offset);
			eye2.entity = this;
			Level.lights.Add(eye2);
			
			// Reset direction to right
			direction = new Vector2(0, 1);

			// Set max speed
			maxspeed = 30.0f;
		}

		public override void Added() {
			base.Added();

			var explosion = Scene.Add(new Explosion(1920 >> 1, 1080 >> 1));
			explosion.SetAlpha(2.0f, 1.0f, 0.0f);
			explosion.SetRadius(2.0f, 100.0f, 580.0f, 560.0f, 480.0f);
		}

		protected override IEnumerator FadeOut() {
			eye1.FadeOut(0.5f);
			eye2.FadeOut(0.5f);
			softlight1.FadeOut(0.1f);
			softlight2.FadeOut(0.1f);
			return base.FadeOut();
		}

		protected override IEnumerator FadeIn() {
			yield return base.FadeIn();

			// Add Enemy tag to colliders
			softSpot1.AddTag((int)Tags.ENEMY);
			softSpot2.AddTag((int)Tags.ENEMY);
			repelCollider.AddTag((int)Tags.ENEMY);
			repelCollider.RemoveTag((int)Tags.REPEL);
		}

		/*public override void Render() {
			base.Render();

			for (int i = 0; i < Colliders.Count; ++i) {
				Colliders[i].Render();
			}
		}*/

		public override void Update() {
			sprite.Alpha = alpha;

			// Scale breath
			sprite.ScaleY += Rand.Float(scaleSpeed) * scaleVelocity;
			scaleVelocity = Util.Lerp(scaleVelocity, scaleDirection, 0.05f);
			var difference = sprite.ScaleY - (1 - maxScaleDistance);
			if (Math.Abs(difference) > maxScaleDistance) {
				if (Math.Sign(difference) == scaleDirection) {
					scaleDirection *= -1;
				}
			}

			switch (curState) {
				case State.Idle:
					IdleUpdate();
					break;
				case State.Swim:
					SwimUpdate();
					break;
			}

			base.Update();

			// Set hitbox size/position
			var hOffset = Util.Rotate(hitboxOffset, sprite.Angle);
			Hitbox.CenterOrigin();
			Hitbox.Width = Math.Abs(hOffset.Y) * 10 + 70;
			Hitbox.Height = Math.Abs(hOffset.X) * 10 + 70;
			Hitbox.OriginX += hOffset.X;
			Hitbox.OriginY += hOffset.Y;

			// Set softspot positions
			setOffset(ref softSpot1, softSpot1Offset);
			setOffset(ref softSpot2, softSpot2Offset);
			setOffset(ref repelCollider, repelOffset);

			CheckCollision(softSpot1);
			CheckCollision(softSpot2);
		}

		private void IdleUpdate() {
			Float();
			LookAt(target.X, target.Y);
		}

		private void LookAt(float targetX, float targetY) {
			Vector2 toTarget = new Vector2(targetX, targetY) - new Vector2(X, Y);
			var newAngle = Util.RAD_TO_DEG * (float)Math.Atan2(-toTarget.Y, toTarget.X);
			var angleDiff = ((((newAngle - sprite.Angle) % 360) + 540) % 360) - 180;
			var curAngle = (float)Math.Atan2(-direction.Y, direction.X);
			var rotateAmount = 0.0f;// = Util.Clamp(angleDiff, -dirStepAmount, dirStepAmount);
			rotateAmount = Util.Lerp(0, angleDiff, 0.07f);
			direction = Util.Rotate(direction, rotateAmount);
			sprite.Angle = (float)Math.Atan2(-direction.Y, direction.X) * Util.RAD_TO_DEG;

			setOffset(ref eye1, eye1offset);
			setOffset(ref eye2, eye2offset);
			setOffset(ref softlight1, softSpot1Offset);
			setOffset(ref softlight2, softSpot2Offset);
		}

		private void Float() {
			X = Util.Lerp(X, 960, 0.07f);
			if (Math.Abs(X - 960) < 0.2f) {
				X = 960;
			}
			
			Y += Rand.Float(floatSpeed) * floatVelocity;
			floatVelocity = Util.Lerp(floatVelocity, floatDirection, 0.01f);
			var difference = Y - 540;
			if (Math.Abs(difference) > maxDistance) {
				if (Math.Sign(difference) == floatDirection) {
					floatDirection *= -1;
				}
			}
		}

		private void SwimUpdate() {
			LookAt(X + 20, Y);
		}

		private void setOffset(ref Light light, Vector2 offset) {
			light.SetOffset(Util.Rotate(offset, sprite.Angle));
		}

		private void setOffset(ref CircleCollider collider, Vector2 offset) {
			var o = Util.Rotate(offset, sprite.Angle);
			collider.CenterOrigin();
			collider.OriginX += o.X;
			collider.OriginY += o.Y;
		}

		IEnumerator SwimAttack() {
			curState = State.Swim;

			// Turn on lights
			softlight1.SetAlpha(0.6f);
			softlight1.FadeIn(4f);

			softlight2.SetAlpha(0.6f);
			softlight2.FadeIn(4f);

			float timeElapsed = 0.0f;
			
			while (timeElapsed < 5f) {
				if (Math.Abs(Y - 540) > 3) {
					Float();
				}
				timeElapsed += Game.Instance.RealDeltaTime * 0.001f;
				yield return 0;
			}

			acceleration = new Vector2(0.5f, 0);
			X += 0.5f;

			while (X > 960) {
				yield return 0;
			}

			while (X < 52) {
				yield return 0;
			}

			acceleration = new Vector2(-0.5f, 0);

			while (velocity.X > 0) {
				yield return 0;
			}

			acceleration = Vector2.Zero;

			softlight1.FadeOut(5f);
			softlight2.FadeOut(5f);

			var num = Rand.Int(2, 3);
			for (int i = 0; i < num; ++i) {
				EnemySpawner.Instance.SpawnRandomEnemy();
			}

			yield return Coroutine.Instance.WaitForFrames(1);

			curState = State.Idle;
		}

		private void CheckCollision(CircleCollider collider) {
			var hit = collider.Collide(X, Y, (int)Tags.PLAYERATTACK);
			if (hit != null) {
				bool isHurt = true;
				
				// Make sure it didn't go through the front side
				if (Hitbox.Collide(X, Y, hit.Entity) != null) {
					var hitboxPos = new Vector2(Hitbox.X, Hitbox.Y);
					var colliderPos = new Vector2(collider.X, collider.Y);
					var entityPos = new Vector2(hit.X, hit.Y);

					var toHitbox = entityPos - hitboxPos;
					var angleToHitbox = Math.Atan2(toHitbox.Y, toHitbox.X);
					var hypt = (float)Math.Sin(angleToHitbox) * toHitbox.Y;
					var hitboxPenetration = hypt - hit.HalfWidth;

					var colliderPenetration = (collider.Radius + hit.HalfWidth) - (colliderPos - entityPos).Length;
					isHurt = (colliderPenetration > hitboxPenetration);
				}

				// Apply damage if okay
				if (isHurt) {
					var e = hit.Entity as Projectile;
					if (e.damage > 1) {
						ApplyDamage(e.damage);
						if (health > 0) {
							Game.Coroutine.Start(SwimAttack());
						}
					}
					e.HitEnemy();
				}
			}
		}

	}
}

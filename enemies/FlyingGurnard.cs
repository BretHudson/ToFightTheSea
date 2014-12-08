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

		private float scaleDirection = -1;
		private float scaleVelocity = -1;
		private Range scaleSpeed = new Range(0.0035f, 0.006f);
		private float maxScaleDistance = 0.025f;

		private Light eye1, eye2;
		private Vector2 eye1offset, eye2offset;
		
		private Vector2 hitboxOffset = new Vector2(-40, 0);
		private Vector2 softSpot1Offset = new Vector2(30, 130);
		private Vector2 softSpot2Offset = new Vector2(30, -130);
		private Vector2 repelOffset = new Vector2(80, 0);

		private CircleCollider softSpot1 = new CircleCollider(115, (int)Tags.ENEMY);
		private CircleCollider softSpot2 = new CircleCollider(115, (int)Tags.ENEMY);
		private CircleCollider repelCollider = new CircleCollider(270, (int)Tags.ENEMY);

		public FlyingGurnard(Entity target) : base(960, 540, 100, 10.0f) {
			// Initialize sprite
			sprite.Add(AnimType.Idle, new Anim(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, new float[] { 4.0f }));
			//sprite.Play(AnimType.Idle);
			sprite.CenterOrigin();
			Graphic = sprite;

			// Set the target
			this.target = target;

			// Set up hitbox
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

		public override void Render() {
			base.Render();

			for (int i = 0; i < Colliders.Count; ++i) {
				Colliders[i].Render();
			}
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

			sprite.ScaleY += Rand.Float(scaleSpeed) * scaleVelocity;
			scaleVelocity = Util.Lerp(scaleVelocity, scaleDirection, 0.1f);
			difference = sprite.ScaleY - (1 - maxScaleDistance);
			if (Math.Abs(difference) > maxScaleDistance) {
				if (Math.Sign(difference) == scaleDirection) {
					scaleDirection *= -1;
				}
			}

			Vector2 toTarget = new Vector2(target.X, target.Y) - new Vector2(X, Y);
			var newAngle = Util.RAD_TO_DEG * (float)Math.Atan2(-toTarget.Y, toTarget.X);
			var angleDiff = ((((newAngle - sprite.Angle) % 360) + 540) % 360) - 180;
			var curAngle = (float)Math.Atan2(-direction.Y, direction.X);
			var rotateAmount = 0.0f;// = Util.Clamp(angleDiff, -dirStepAmount, dirStepAmount);
			rotateAmount = Util.Lerp(0, angleDiff, 0.07f);
			direction = Util.Rotate(direction, rotateAmount);
			sprite.Angle = (float)Math.Atan2(-direction.Y, direction.X) * Util.RAD_TO_DEG;

			setOffset(ref eye1, eye1offset);
			setOffset(ref eye2, eye2offset);

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

		private void setOffset(ref Light light, Vector2 offset) {
			light.SetOffset(Util.Rotate(offset, sprite.Angle));
		}

		private void setOffset(ref CircleCollider collider, Vector2 offset) {
			var o = Util.Rotate(offset, sprite.Angle);
			collider.CenterOrigin();
			collider.OriginX += o.X;
			collider.OriginY += o.Y;
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
					ApplyDamage(e.damage);
					e.HitEnemy();
				}
			}
		}

	}
}

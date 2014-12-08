using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class Player : Entity {

		enum AnimType {
			Go, Attack
		}

		private Spritemap<AnimType> sprite = new Spritemap<AnimType>("assets/gfx/tentacool.png", 70, 87);

		private float angle = 90;
		private Vector2 direction = new Vector2(0, -1);
		private Vector2 lastInput = new Vector2(0, -1);
		private float dirStepAmount = 5.0f;

		private Vector2 acceleration;
		private Vector2 velocity;
		private float lastX, lastY;

		private float maxspeed = 9.0f;
		private float aspeed = 0.46f;
		private float friction = 0.30f;
		private Session session;

		private float minBounce = 2.0f;

		private bool canAttack = true;
		private bool canMove = true;

		private Light light;

		public Player(float x, float y, Session session) : base(x, y) {
			// Assign session
			this.session = session;

			// Set up animations
			sprite.Add(AnimType.Go, new Anim(new int[] { 0, 1, 2, 3, 4, }, new float[] { 5.0f }));
			sprite.Add(AnimType.Attack, new Anim(new int[] { 7, 8, 9, 10, 11, 12, 13 }, new float[] { 4.0f }));
			sprite.Play(AnimType.Go);
			
			// Set up hitboxes
			SetHitbox(sprite.Width - 24, sprite.Height - 24, (int)Tags.PLAYER);
			Hitbox.CenterOrigin();

			// Sprite stuff
			sprite.CenterOrigin();
			Graphic = sprite;

			Layer = -1;

			// Initilize light
			light = new Light();
			light.SetAlpha(0.7f);
			light.SetColor(new Color("879DFF"), new Color("62D8E0"));
			light.SetColorSpan(5.0f);
			light.SetRadius(sprite.Width + 30, sprite.Width + 60);
			light.SetRadiusSpan(5.0f);
			light.entity = this;
			Level.lights.Add(light);

			// TODO: Make ink slow you down
		}

		public override void Update() {
			// Reset variables
			acceleration = Vector2.Zero;

			// Get player input
			Vector2 inputDir;
			inputDir.X = Convert.ToInt32(session.Controller.Right.Down) - Convert.ToInt32(session.Controller.Left.Down);
			inputDir.Y = Convert.ToInt32(session.Controller.Down.Down) - Convert.ToInt32(session.Controller.Up.Down);

			if (!canMove)
				inputDir = Vector2.Zero;

			if (inputDir == Vector2.Zero) {
				// Make it slow down as an overall speed rather than each axis
				var length = velocity.Length;
				length = Math.Max(0, length - friction);
				velocity.Normalize();
				velocity *= length;
			} else {
				// Accelerate
				if (inputDir.X != 0) {
					acceleration.X = inputDir.X * aspeed;
					velocity.X += acceleration.X;
					velocity.X = Util.Clamp(velocity.X, -maxspeed, maxspeed);
				} else {
					velocity.X = Util.Approach(velocity.X, 0, friction);
				}

				if (inputDir.Y != 0) {
					acceleration.Y = inputDir.Y * aspeed;
					velocity.Y += acceleration.Y;
					velocity.Y = Util.Clamp(velocity.Y, -maxspeed, maxspeed);
				} else {
					velocity.Y = Util.Approach(velocity.Y, 0, friction);
				}
			}

			// Repel away from enemies
			var enemyHit = Collide(X, Y, (int)Tags.ENEMY);
			if (enemyHit != null) {
				var enemy = enemyHit.Entity;
				Vector2 awayFromEnemy = new Vector2(enemy.X, enemy.Y) - new Vector2(X, Y);
				awayFromEnemy.Normalize();
				velocity -= awayFromEnemy * 3;
			}

			// Make sure velocity doesn't go infinitely
			if (velocity.Length > maxspeed) {
				velocity.Normalize();
				velocity *= maxspeed;
			}

			// Move object
			X += velocity.X;
			Y += velocity.Y;
			
			// Check to see if inside of object
			var c = Collide(X, Y, (int)Tags.SOLID);
			if (c != null) {
				for (int i = 0; i < Math.Max(velocity.X, velocity.Y); ++i) {
					// X collision
					if (!Overlap(X - i * Math.Sign(velocity.X), Y, c.Entity)) {
						X -= i * Math.Sign(velocity.X);
						velocity.X *= -0.9f;
						acceleration.X = Math.Sign(velocity.X);
						if (Math.Abs(velocity.X) < minBounce) {
							velocity.X = minBounce * Math.Sign(velocity.X);
						}
						break;
					}

					// Y collision
					if (!Overlap(X, Y - i * Math.Sign(velocity.Y), c.Entity)) {
						Y -= i * Math.Sign(velocity.Y);
						velocity.Y *= -0.9f;
						acceleration.Y = Math.Sign(velocity.Y);
						if (Math.Abs(velocity.Y) < minBounce) {
							velocity.Y = minBounce * Math.Sign(velocity.Y);
						}
						break;
					}
				}

				if (Overlap(X, Y, c.Entity)) {
					if (!Overlap(lastX, Y, c.Entity)) {
						if (lastX < X) {
							X = c.Left - Hitbox.HalfWidth;
						} else {
							X = c.Right + Hitbox.HalfWidth;
						}
						velocity.X = 0;
					}
					else if (!Overlap(X, lastY, c.Entity)) {
						if (lastY < Y) {
							Y = c.Top - Hitbox.HalfHeight;
						} else {
							Y = c.Bottom + Hitbox.HalfHeight;
						}
						velocity.Y = 0;
					}
				}
			}

			Attack();

			// Get that sprite moving
			OrientSprite();

			// Reset last X/Y
			lastX = X;
			lastY = Y;

			Wrap();
		}

		void Attack() {
			if (canAttack) {
				var shootBall = session.Controller.Square.Pressed;
				var areaAttack = session.Controller.Triangle.Pressed;

				if (shootBall) {
					Game.Coroutine.Start(ShootBall());
				} else if (areaAttack) {
					Game.Coroutine.Start(AreaAttack());
				}
			}
		}

		IEnumerator ShootBall() {
			// Disable shooting
			canAttack = false;
			sprite.Play(AnimType.Attack);
			yield return Coroutine.Instance.WaitForFrames(20);

			// Shoot and shake screen
			Vector2 offset = new Vector2(12, 0);
			offset = Util.Rotate(offset, angle);
			Scene.Add(new LightningBall(X + offset.X, Y + offset.Y, direction));
			//Screenshaker.InitShake(10, 6);
			yield return Coroutine.Instance.WaitForFrames(8);

			// Reenable shooting
			canAttack = true;
			sprite.Play(AnimType.Go);
		}

		IEnumerator AreaAttack() {
			canMove = false;
			canAttack = false;
			sprite.Play(AnimType.Attack);
			yield return Coroutine.Instance.WaitForFrames(20);

			// TODO: Burst it quick
			canMove = true;
			Scene.Add(new LightningArea(X, Y, direction));
			yield return Coroutine.Instance.WaitForFrames(8);

			canAttack = true;
			sprite.Play(AnimType.Go);
		}

		void OrientSprite() {
			if ((Math.Abs(acceleration.X) > 0.0f) || (Math.Abs(acceleration.Y) > 0.0f)) {
				var newAngle = Util.RAD_TO_DEG * (float)Math.Atan2(-acceleration.Y, acceleration.X);
				var angleDiff = ((((newAngle - angle) % 360) + 540) % 360) - 180;
				var rotateAmount = Util.Clamp(angleDiff, -dirStepAmount, dirStepAmount);
				direction = Util.Rotate(direction, rotateAmount);
				angle = (float)Math.Atan2(-direction.Y, direction.X) * Util.RAD_TO_DEG;
			}
			sprite.Angle = angle - 90;
		}

		void Wrap() {
			var left = 0 - (Hitbox.Width * 0.95f);
			var right = 1920 + (Hitbox.Width * 0.95f);

			var top = 0 - (Hitbox.Height * 0.95f);
			var bottom = 1080 + (Hitbox.Height * 0.95f);

			if (X < left) {
				lastX = X = right;
			} else if (X > right) {
				lastX = X = left;
			}

			if (Y < top) {
				lastY = Y = bottom;
			} else if (Y > bottom) {
				lastY = Y = top;
			}
		}

	}
}

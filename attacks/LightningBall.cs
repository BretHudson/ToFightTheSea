﻿using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class LightningBall : Projectile {

		private Image sprite = Image.CreateCircle(14, Color.FromHSV(0.166f, 0.40f, 1.0f, 1.0f));

		public LightningBall(float x, float y, Vector2 direction) : base(x, y, direction, 15.0f, 10, 1.3f, (int)Tags.PROJECTILE, (int)Tags.PLAYERATTACK) {
			sprite.CenterOrigin();
			Graphic = sprite;
			sprite.Scale = 0.3f;

			light = new Light();
			light.SetAlpha(0.5f, 0.7f);
			light.SetAlphaSpan(0.5f);
			light.SetColor(new Color("FFD700"));
			light.SetColorSpan(5.0f);
			light.SetRadius(sprite.Width + 50, sprite.Width + 80);
			light.SetRadiusSpan(0.1f);
			light.entity = this;
			Level.lights.Add(light);
		}

		public override void Update() {
			base.Update();
			if (sprite.ScaleX < 1) {
				sprite.Scale = Util.Approach(sprite.ScaleX, 1.0f, 0.15f);
			}
		}

		protected override IEnumerator Explosion() {
			// TODO: this
			return base.Explosion();
		}

	}
}
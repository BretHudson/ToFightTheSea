using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class LightningArea : Projectile {

		public LightningArea(float x, float y, Vector2 direction) : base(x, y, direction, 0.1f, 100, 1.7f, (int)Tags.PROJECTILE, (int)Tags.PLAYERATTACK) {
			light = new Light();
			light.SetAlpha(0.5f, 0.7f);
			light.SetAlphaSpan(0.3f);
			light.SetColor(new Color("FFD700"));
			light.SetColorSpan(5.0f);
			light.SetRadius(300, 355);
			light.SetRadiusSpan(0.07f);
			light.entity = this;
			Level.lights.Add(light);
		}

		public override void Added() {
			base.Added();

			Game.Instance.Coroutine.Start(Ripple());
		}

		IEnumerator Ripple() {
			var explosion = Scene.Add(new Explosion(X, Y));
			explosion.SetAlpha(1.0f, 1.0f, 0.0f);
			explosion.SetRadius(1.0f, 80.0f, 550.0f, 450.0f, 400.0f);

			yield return Coroutine.Instance.WaitForFrames((int)(0.3f * 60));

			explosion = Scene.Add(new Explosion(X, Y));
			explosion.SetAlpha(1.0f, 1.0f, 0.0f);
			explosion.SetRadius(1.0f, 80.0f, 420.0f, 330.0f, 310.0f);
		}

		protected override IEnumerator Explosion() {
			light.FadeOut(0.4f);
			yield return Coroutine.Instance.WaitForSeconds(0.4f);
		}

	}
}

using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class EnemySpawner : Entity {

		public static EnemySpawner Instance = null;

		enum GameState {
			START, WAVE1, WAVE2, BOSS1
		};

		enum EnemyType {
			SQUID, LINK, GURNARD, DISK
		};

		private GameState state;

		private List<Entity> enemies = new List<Entity>();

		private bool checkForEnemies = false;

		private Entity player;

		public EnemySpawner(Entity player) {
			this.player = player;
			state = GameState.START;
			checkForEnemies = true;
			Instance = this;
		}

		public override void Update() {
			base.Update();

			foreach (Entity e in enemies) {
				if (!e.IsInScene) {
					enemies.Remove(e);
				}
			}

			if ((checkForEnemies) && (enemies.Count == 0)) {
				++state;
				switch (state) {
					case GameState.WAVE1:
						Game.Coroutine.Start(Wave1());
						break;
					case GameState.WAVE2:
						Game.Coroutine.Start(Wave2());
						break;
					case GameState.BOSS1:
						Game.Coroutine.Start(Boss1());
						break;
				}
			}
		}

		public void AddEnemy(Entity e) {
			enemies.Add(e);
		}

		private void SpawnEnemy(EnemyType type) {
			float x, y;
			x = Rand.Float(300, 1620);
			y = Rand.Float(250, 830);

			switch (type) {
				case EnemyType.DISK:
					break;
				case EnemyType.GURNARD:
					var gurnard = Scene.Add(new FlyingGurnard(player));
					AddEnemy(gurnard);
					break;
				case EnemyType.LINK:
					var babies = Rand.Int(10, 20);
					var link = Scene.Add(new SeaLink(x, y, babies, Vector2.Zero));
					link.target = player;
					AddEnemy(link);
					break;
				case EnemyType.SQUID:
					var squid = Scene.Add(new Squid(x, y));
					squid.target = player;
					AddEnemy(squid);
					break;
			}
		}

		private IEnumerator Wave1() {
			checkForEnemies = false;
			yield return 0;

			// Spawn the first enemy
			checkForEnemies = true;
			yield return 0;
		}

		private IEnumerator Wave2() {
			checkForEnemies = false;
			yield return 0;

			// Spawn the first enemy
			checkForEnemies = true;
			yield return 0;
		}

		private IEnumerator Boss1() {
			checkForEnemies = false;
			yield return 0;

			// Spawn the first enemy
			checkForEnemies = true;
			yield return 0;
		}

	}
}

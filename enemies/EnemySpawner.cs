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
			START, WAVE1, WAVE2, WAVE3, BOSS1,
		};

		public enum EnemyType {
			SQUID, LINK, FATFISH, GURNARD
		};

		private GameState state;

		private List<Enemy> enemies = new List<Enemy>();

		private bool checkForEnemies = false;

		private Entity player;

#if DEBUG
		private int framesBeforeWave = 2;
#else
		private int framesBeforeWave = 120;
#endif

		public EnemySpawner(Entity player) {
			this.player = player;
			state = GameState.START;
			checkForEnemies = true;
			Instance = this;
		}

		public override void Update() {
			base.Update();

			if ((checkForEnemies) && (enemies.Count == 0)) {
				++state;
				switch (state) {
					case GameState.BOSS1:
						Game.Coroutine.Start(Boss1());
						break;
					case GameState.WAVE1:
						Game.Coroutine.Start(Wave1());
						break;
					case GameState.WAVE2:
						Game.Coroutine.Start(Wave2());
						break;
					case GameState.WAVE3:
						Game.Coroutine.Start(Wave3());
						break;
					default:
						((Level)Scene).Victory();
						break;
				}
			}
		}

		public void AddEnemy(Enemy e) {
			enemies.Add(e);
		}

		public void RemoveEnemy(Enemy e) {
			enemies.Remove(e);
			Console.WriteLine(enemies.Count);
		}

		public IEnumerator RemoveAllEnemies() {
			foreach (Enemy e in enemies) {
				e.Kill();
			}

			var alpha = 0.0f;
			while (alpha < 0.98f) {
				alpha = Util.Lerp(alpha, 1.0f, 0.07f);
				yield return 0;
			}
			alpha = 1.0f;
		}

		public void SpawnRandomEnemy() {
			SpawnEnemy((EnemyType)Rand.Int(0, (int)EnemyType.FATFISH));
		}

		public void SpawnEnemy(EnemyType type) {
			float x, y;
			while (true) {
				x = Rand.Float(300, 1620);
				y = Rand.Float(250, 830);

				var distance = new Vector2(x - player.X, y - player.Y);
				if (distance.Length > 400) {
					break;
				}
			}

			switch (type) {
				case EnemyType.FATFISH:
					var fatfish = Scene.Add(new FatFish(x, y));
					break;
				case EnemyType.GURNARD:
					var gurnard = Scene.Add(new FlyingGurnard(player));
					break;
				case EnemyType.LINK:
					var babies = Rand.Int(10, 20);
					var link = Scene.Add(new SeaLink(x, y, babies, Vector2.Zero));
					link.target = player;
					break;
				case EnemyType.SQUID:
					var squid = Scene.Add(new Squid(x, y));
					squid.target = player;
					break;
			}
		}

		private IEnumerator Wave1() {
			Global.musicManager.PlayTrack(MusicManager.Tracks.GAME);
			checkForEnemies = false;
			yield return Coroutine.Instance.WaitForFrames(framesBeforeWave);

			// Spawn the first enemy
			SpawnEnemy(EnemyType.SQUID);
			SpawnEnemy(EnemyType.SQUID);

			checkForEnemies = true;
			yield return 0;
		}

		private IEnumerator Wave2() {
			Global.musicManager.PlayTrack(MusicManager.Tracks.GAME);
			checkForEnemies = false;
			yield return Coroutine.Instance.WaitForFrames(framesBeforeWave);

			SpawnEnemy(EnemyType.LINK);
			SpawnEnemy(EnemyType.LINK);
			SpawnEnemy(EnemyType.SQUID);
			checkForEnemies = true;
			yield return 0;
		}

		private IEnumerator Wave3() {
			Global.musicManager.PlayTrack(MusicManager.Tracks.GAME);
			checkForEnemies = false;
			yield return Coroutine.Instance.WaitForFrames(framesBeforeWave);

			SpawnEnemy(EnemyType.LINK);
			SpawnEnemy(EnemyType.LINK);
			SpawnEnemy(EnemyType.LINK);
			SpawnEnemy(EnemyType.SQUID);
			SpawnEnemy(EnemyType.SQUID);

			// Spawn the first enemy
			checkForEnemies = true;
			yield return 0;
		}

		private IEnumerator Boss1() {
			checkForEnemies = false;
			yield return Coroutine.Instance.WaitForFrames(framesBeforeWave);

			Global.musicManager.PlayTrack(MusicManager.Tracks.BOSS);
			SpawnEnemy(EnemyType.GURNARD);
			checkForEnemies = true;
			yield return 0;
		}

	}
}

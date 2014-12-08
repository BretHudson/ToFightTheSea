using Otter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD31 {
	class MusicManager {

		public enum Tracks {
			GAME, BOSS
		};

		private Music gameMusic = new Music("assets/sfx/Where The Light Fades.wav", true);
		private Music bossMusic = new Music("assets/sfx/To Fight The Sea.wav", true);

		private Music currentTrack = null;

		public MusicManager() {
#if DEBUG
			Music.GlobalVolume = 1;
			PlayTrack(Tracks.GAME);
#else
			PlayTrack(Tracks.GAME);
#endif
		}

		public void PlayTrack(Tracks track) {
			Music newTrack = null;

			Console.WriteLine(track);
			
			switch (track) {
				case Tracks.GAME:
					newTrack = gameMusic;
					break;
				case Tracks.BOSS:
					newTrack = bossMusic;
					break;
			}

			if ((newTrack == currentTrack) || (newTrack == null)) {
				return;
			}

			if (currentTrack == null) {
				newTrack.Volume = 1;
				newTrack.Play();
			} else {
				Game.Instance.Coroutine.Start(FadeTracks(currentTrack, newTrack));
			}

			currentTrack = newTrack;
		}

		private IEnumerator FadeTracks(Music oldTrack, Music newTrack) {
			while (oldTrack.Volume > 0.7f) {
				oldTrack.Volume = Util.Lerp(oldTrack.Volume, 0.0f, 0.03f);
				newTrack.Volume = 1.0f - oldTrack.Volume;
				yield return 0;
			}

			newTrack.Play();
			
			while (oldTrack.Volume > 0.01f) {
				oldTrack.Volume = Util.Lerp(oldTrack.Volume, 0.0f, 0.03f);
				newTrack.Volume = 1.0f - oldTrack.Volume;
				yield return 0;
			}

			oldTrack.Stop();
			oldTrack.Volume = 0.0f;
			newTrack.Volume = 1.0f;
		}

	}
}

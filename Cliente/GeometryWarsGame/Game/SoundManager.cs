using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game
{
    public class SoundManager
    {
        private static System.Media.SoundPlayer? player = new System.Media.SoundPlayer();

        private static uint Volume = 0;

        public static SoundTrack? CurrentTrack { get; set; } = null;

        public static List<SoundTrack> SoundTracks = new List<SoundTrack>()
        {
            new SoundTrack(1, "Suspicious Bytes", Properties.Resources.lobby1),
            new SoundTrack(4, "Geometry Orgy", Properties.Resources.game3),
        };

        [DllImport("winmm.dll")]
        private static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        private static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        public static void SetVolume(int volume)
        {
            if (Volume == 0)
            {
                waveOutGetVolume(IntPtr.Zero, out uint currentVolume);
                Volume = (currentVolume & 0x0000ffff) / (ushort.MaxValue / 10);
            }

            int newVolume = ushort.MaxValue / 10 * volume;
            waveOutSetVolume(IntPtr.Zero, ((uint)newVolume & 0x0000ffff) | ((uint)newVolume << 16));
            Volume = (uint)newVolume;
        }

        private static void PlayTrack(int trackIndex)
        {
            if (trackIndex >= SoundTracks.Count)
            {
                return;
            }

            if (player == null)
            {
                return;
            }

            SoundTrack track = SoundTracks[trackIndex];
            player.Stream = track.Audio;

            Task.Run(() =>
            {
                if (!player.IsLoadCompleted)
                {
                    player.LoadAsync();
                }

                CurrentTrack = track;
                player.Play();
            });
        }

        private static void PlayTrackSync(int trackIndex)
        {
            if (trackIndex >= SoundTracks.Count)
            {
                return;
            }

            if (player == null)
            {
                return;
            }

            SoundTrack track = SoundTracks[trackIndex];
            player.Stream = track.Audio;

            if (!player.IsLoadCompleted)
            {
                player.Load();
            }

            CurrentTrack = track;
            player.PlaySync();
        }

        private static void PlayTrackLoop(int trackIndex)
        {
            if (trackIndex >= SoundTracks.Count)
            {
                return;
            }

            if (player == null)
            {
                return;
            }

            SoundTrack track = SoundTracks[trackIndex];
            player.Stream = track.Audio;

            if (!player.IsLoadCompleted)
            {
                player.Load();
            }

            CurrentTrack = track;
            player.PlayLooping();
        }

        public static void PlayLobby()
        {
            SetVolume(1);
            PlayTrackLoop(0);
        }

        public static void PlayIngame()
        {
            SetVolume(1);
            PlayTrackLoop(1);
        }

        public static void StopQueue()
        {
            if (player == null)
            {
                return;
            }

            player.Stop();
            CurrentTrack = null;
        }

        public static void Unload()
        {
            if (player == null)
            {
                return;
            }

            StopQueue();
            player.Dispose();
            player = null;
        }

        public static bool IsPlaying()
        {
            return player != null && CurrentTrack != null && Volume > 0;
        }

        public static bool IsPlayerReady()
        {
            return player != null && player.IsLoadCompleted && player.Stream.Position > 0;
        }

        public static bool IsPlayerUnloaded()
        {
            return player == null;
        }

    }

    public class SoundTrack
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Stream Audio { get; set; }

        public SoundTrack(int id, string name, Stream audio)
        {
            Id = id;
            Name = name;
            Audio = audio;
        }
    }
}

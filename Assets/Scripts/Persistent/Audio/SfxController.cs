using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Connexion.Audio
{
    public class SfxController : MonoBehaviour
    {
        public class PlayCommand
        {
            public AudioClip clip { get; set; }
            public float volume { get; set; }
            public float pitch { get; set; }

            public PlayCommand(AudioClip clip, float vol, float pitch = 1f)
            {
                this.clip = clip; 
                this.volume = vol;
                this.pitch = pitch;
            }
        }

        private AudioSource _audio;

        // Event Queue
        public Queue<PlayCommand> queue;

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
            queue = new Queue<PlayCommand>();
        }

        private void FixedUpdate()
        {
            while (queue.Count > 0)
            {
                PlayCommand cmd = queue.Dequeue();
                while (queue.Count > 0 && cmd.clip == queue.Peek().clip && cmd.pitch == queue.Peek().pitch)
                {
                    cmd.volume = Mathf.Max(cmd.volume, queue.Peek().volume);
                    queue.Dequeue();
                }
                _audio.pitch = cmd.pitch;
                _audio.PlayOneShot(cmd.clip, cmd.volume);
            }
        }

        public void PlaySfx(AudioClip audioClip, float volume = -1, float pitch = 1f)
        {
            queue.Enqueue(new PlayCommand(audioClip, volume == -1 ? Persistent.Instance.gameManager.sfxVolume : volume, pitch));
        }

        public void SyncVolume() { _audio.volume = Persistent.Instance.gameManager.sfxVolume; }
    }
}
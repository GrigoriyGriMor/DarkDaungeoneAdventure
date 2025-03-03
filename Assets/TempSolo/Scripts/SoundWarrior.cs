using Game.Core;
using SoundSystem;
using UnityEngine;

[System.Serializable]
public class SoundWarrior { 
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClipPatrol;
    [SerializeField] private AudioClip audioClipDetect;
    [SerializeField] private AudioClip audioClipStartAttack;
    [SerializeField] private AudioClip audioClipAttack;
    [SerializeField] private AudioClip audioClipDamage;
    
    private GameManager _gameManager;
    private SoundManagerAllControll _soundManager;

    public void Initialize() {
        _gameManager = GameManager.Instance;
        _soundManager = _gameManager.GetManager<SoundManagerAllControll>();
    }
    
    public void PlayPatrol(Vector3 position) {
        _soundManager.ClipLoopAndPlay(audioClipPatrol, audioSource);
    }

    public void PlayDetect(Vector3 position) {
        _soundManager.ClipPlay(audioClipDetect, position, SoundPriority.Fire);
    }

    public void PlayStartAttack(Vector3 position) {
        _soundManager.ClipPlay(audioClipStartAttack, position, SoundPriority.Fire);
    }

    public void PlayAttack(Vector3 position) {
        _soundManager.ClipPlay(audioClipAttack, position, SoundPriority.Fire);
    }

    public void PlayDamage(Vector3 position) {
        _soundManager.ClipPlay(audioClipDamage, position, SoundPriority.Damage);
    }
    
}
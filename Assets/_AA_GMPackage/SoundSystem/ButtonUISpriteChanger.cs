using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace SoundSystem
{
    public class ButtonUISpriteChanger : MonoBehaviour
    {
        [Header("Image")]
        [SerializeField] private Image _imageVisual;

        [Header("Visual Button")]
        [SerializeField] private Sprite _soundActiveSprite;
        [SerializeField] private Sprite _soundDeactiveSprite;

        void Start()
        {
            GameManager.Instance.GetManager<SoundManagerAllControll>().SoundButtonInit(this);
        }

        public void ChangeSprite(bool On)
        {
            _imageVisual.sprite = On ? _soundActiveSprite : _soundDeactiveSprite;
        }
    }
}
using System.Collections;
using UnityEngine;

public class ZoneDamage1 : MonoBehaviour {
   [SerializeField] private Transform zoneSmall;
   [SerializeField] private Transform zoneBig;
   [SerializeField] private Transform cast;
   [SerializeField] private ParticleSystem particleSys;

   [SerializeField] private float _animationTime;
   [SerializeField] private float _stepInterpolate;
   [SerializeField] private Vector3 _maxLocalScale;


   private float _timeAnimation;
   private Vector3 _startScale;
   private Vector3 _currentLocalScale;


   public void Initialize() {
      transform.parent = null;
      _startScale = zoneBig.localScale;
      Hide();
   }

   public void Show() {
      gameObject.SetActive(true);
      StartCoroutine(ScaleZoneDamage());
   }

   public void Hide() {
      gameObject.SetActive(false);
      cast.gameObject.SetActive(false);
      //particleSys.gameObject.SetActive(false);
   }

   public void SetPosition(Vector3 positionPlayer) {
      transform.position = positionPlayer;
   }

   public void Damage() {
      cast.gameObject.SetActive(true);
      particleSys.Play();
      //particleSys.gameObject.SetActive(true);
   }

   private IEnumerator ScaleZoneDamage() {
      _currentLocalScale = _startScale;
      _timeAnimation = 0;
      while (true) {
         yield return null;
         if (_timeAnimation < _animationTime) {
            zoneBig.localScale = Vector3.Lerp(_currentLocalScale, _maxLocalScale, _timeAnimation);
            _timeAnimation += Time.deltaTime * _stepInterpolate;
         }
      }
   }

}
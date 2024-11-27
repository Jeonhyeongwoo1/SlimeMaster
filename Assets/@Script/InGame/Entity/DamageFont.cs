using SlimeMaster.Common;
using SlimeMaster.Managers;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace SlimeMaster.InGame.Entity
{

    public class DamageFont : MonoBehaviour
    {
        TextMeshPro _damageText;

        public void SetInfo(Vector2 pos, float damage = 0, float healAmount = 0, Transform parent = null, bool isCritical = false)
        {
            _damageText = GetComponent<TextMeshPro>();
            transform.position = pos;

            if (healAmount > 0)
            {
                _damageText.text = $"{Mathf.RoundToInt(healAmount)}";
                _damageText.color = Utils.HexToColor("4EEE6F");
            }
            else if (isCritical)
            {
                _damageText.text = $"{Mathf.RoundToInt(damage)}";
                _damageText.color = Utils.HexToColor("EFAD00");
            }
            else
            {
                _damageText.text = $"{Mathf.RoundToInt(damage)}";
                _damageText.color = Color.white;
            }
            _damageText.alpha = 1;
            if (parent != null)
            {
                GetComponent<MeshRenderer>().sortingOrder = 321;
            }
            
            gameObject.SetActive(true);
            DoAnimation();
        }

        private void DoAnimation()
        {
            Sequence seq = DOTween.Sequence();
            transform.localScale = new Vector3(0, 0, 0);
        
            seq.Append(transform.DOScale(1.3f, 0.3f).SetEase(Ease.InOutBounce))
                .Join(transform.DOMove(transform.position + Vector3.up, 0.3f).SetEase(Ease.Linear))
                .Append(transform.DOScale(1.0f, 0.3f).SetEase(Ease.InOutBounce))
                .Join(transform.GetComponent<TMP_Text>().DOFade(0, 0.3f).SetEase(Ease.InQuint))
                .OnComplete(() =>
                {
                    Managers.Manager.I.Pool.ReleaseObject(nameof(DamageFont), gameObject);
                });

        }
    }

}
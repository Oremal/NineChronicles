using Nekoyume.Game.Controller;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI.Module
{
    public class CategoryButton : MonoBehaviour, IToggleable
    {
        public Button button;
        public Image effectImage;
        
        private IToggleListener _toggleListener;

        protected void Awake()
        {
            button.OnClickAsObservable().Subscribe(_ =>
            {
                AudioController.PlayClick();
                _toggleListener?.OnToggle(this);
            }).AddTo(gameObject);
        }
        
        #region IToggleable

        public string Name => name;
        
        public bool IsToggledOn => effectImage.enabled;

        public void SetToggleListener(IToggleListener toggleListener)
        {
            _toggleListener = toggleListener;
        }

        public void SetToggledOn()
        {
            button.interactable = false;
            effectImage.enabled = true;
        }
        
        public void SetToggledOff()
        {
            button.interactable = true;
            effectImage.enabled = false;
        }

        #endregion
    }
}

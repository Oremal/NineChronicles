using Nekoyume.Game.Item;
using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI
{
    public class SelectedItem : MonoBehaviour
    {
        public Text itemName;
        public Image priceIcon;
        public Text price;
        public Image icon;
        public Text info;
        public Text flavour;

        public ItemBase item;

        private bool _considerPrice = false;
        
        #region Mono

        private void Awake()
        {
            if (ReferenceEquals(priceIcon, null) ||
                ReferenceEquals(price, null))
            {
                _considerPrice = false;
            }
            else
            {
                _considerPrice = true;
            }
        }

        #endregion

        public void SetItem(ItemBase itemBase)
        {
            item = itemBase;
            itemName.text = itemBase.Data.name;
            info.text = itemBase.ToItemInfo();
            flavour.text = itemBase.Data.description;

            if (_considerPrice)
            {
                priceIcon.enabled = true;
                price.text = "1";
            }
        }

        public void SetIcon(Sprite sprite)
        {
            icon.overrideSprite = sprite;
            icon.SetNativeSize();
            icon.enabled = true;
        }

        public void Clear()
        {
            item = null;
            itemName.text = "아이템 정보";
            icon.enabled = false;
            info.text = "아이템을 선택하세요";
            flavour.text = "";

            if (_considerPrice)
            {
                priceIcon.enabled = false;
                price.text = "";
            }
        }
    }
}

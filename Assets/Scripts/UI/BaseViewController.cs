using UnityEngine;

namespace Farm
{
    public abstract class BaseViewController : MonoBehaviour
    {
        public virtual void OnShow()
        {
            gameObject.SetActive(true);
        }

        public virtual void OnHide()
        {
            gameObject.SetActive(false);
        }
    }
}

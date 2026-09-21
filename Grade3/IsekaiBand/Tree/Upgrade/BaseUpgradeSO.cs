using UnityEngine;

namespace _Work.CHUH.Code.Tree.Upgrade
{
    public class BaseUpgradeSO : ScriptableObject
    {
        [TextArea] public string effectDescription;

        public virtual void Apply()
        {
        }
    }
}

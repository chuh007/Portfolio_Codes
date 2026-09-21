using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal static class BandPianoProjectileSetup
    {
        public static void DisableBuiltIn(GameObject obj)
        {
            if (obj == null)
                return;

            KeyboardProjectile builtInProjectile = obj.GetComponent<KeyboardProjectile>();
            if (builtInProjectile != null)
                builtInProjectile.enabled = false;

            foreach (Collider2D collider in obj.GetComponents<Collider2D>())
                collider.enabled = false;
        }
    }
}

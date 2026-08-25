using UnityEngine;

namespace Bluff.Combat
{
    /// <summary>
    /// Zac's domain. Twin-stick bullet hell after the card table.
    /// Intentionally empty. Do not implement combat here during card scaffolding.
    ///
    /// Later hook (comments only):
    /// - Read CardRoundBlackboard.PendingCombatBuffStrength / PendingCombatDebuffStrength
    /// - Apply to player / floor enemies before the run starts
    /// - No direct call from CardRoundManager into this type yet
    /// </summary>
    public sealed class CombatRoundPlaceholder : MonoBehaviour
    {
        // Reserved for dungeon gen + twin-stick controller wiring.
    }
}

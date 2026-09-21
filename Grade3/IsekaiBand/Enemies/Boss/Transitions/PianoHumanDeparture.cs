using System.Threading;
using _Work.CHUH.Code.Audio;
using Cysharp.Threading.Tasks;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal static class PianoHumanDeparture
    {
        public static async UniTask<bool> PlayAsync(PianoBossHumanFormTransition settings,
            PianoHumanBody body, CancellationToken cancellationToken)
        {
            if (settings.PhaseOneDefeatPresentation != null
                && await settings.PhaseOneDefeatPresentation.PlayAsync(
                    body.Owner.transform.position,
                    cancellationToken))
                return true;

            UniTask<bool> visualVanish = body.Appearance.PlayVanishAsync(settings, cancellationToken);
            UniTask<bool> bgmFadeOut = SoundManager.FadeOutBgmAsync(
                settings.PhaseOneVanishDuration,
                cancellationToken);
            (bool visualCanceled, bool bgmCanceled) = await UniTask.WhenAll(
                visualVanish,
                bgmFadeOut);
            if (visualCanceled || bgmCanceled)
                return true;

            if (settings.PhaseTransitionDelay > 0f)
            {
                bool delayCanceled = await UniTask.WaitForSeconds(
                        settings.PhaseTransitionDelay,
                        cancellationToken: cancellationToken)
                    .SuppressCancellationThrow();
                if (delayCanceled)
                    return true;
            }

            return false;
        }
    }
}

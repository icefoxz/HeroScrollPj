//
//

namespace Donews.mediation
{

    public interface IInterstitialAdListener
    {

        void InterstitialDidLoadSuccess();

        void InterstitialDidLoadFaild(int errorCode, string errorMsg);

        void InterstitialAdWillVisible();

        void InterstitialAdDidClose();

        void InterstitialAdExposured();

        void InterstitialAdDidClicked();

        void InterstitialAdDidClosedDetails();

    }
}

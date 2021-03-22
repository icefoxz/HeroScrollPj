//
//

namespace Donews.mediation
{
    
    public interface ISplashAdListener
    {

        void SplashAdDidLoadSuccess();

        void SplashAdDidLoadFaild(int errorCode, string errorMsg);

        void SplashAdDidClicked();

        void SplashAdDidClickCloseButton();

        void SplashAdDidClose();

        void SplashAdWillClose();

        void SplashAdExposured();

    }
}
//
//

namespace Donews.mediation
{

    public interface IBannerAdListener
    {

        void BannerAdDidLoadSuccess();

        void BannerAdDidLoadFaild(int errorCode, string errorMsg);

        void BannerAdExposured();

        void BannerAdDidClicked();

        void BannerAdDidClickClose();

        void BannerAdDidShowDetails();

        void BannerAdDidCloseDetails();

    }
}
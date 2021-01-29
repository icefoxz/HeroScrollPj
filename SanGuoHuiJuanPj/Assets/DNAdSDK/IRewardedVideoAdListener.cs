//
//

namespace Donews.mediation
{

    public interface IRewardedVideoAdListener
    {

        void RewardVideoAdDidLoadSuccess();

        void RewardVideoAdDidLoadFaild(int errorCode, string errorMsg);

        void RewardVideoAdVideoDidLoad();

        void RewardVideoAdWillVisible();

        void RewardVideoAdExposured();

        void RewardVideoAdDidClose();

        void RewardVideoAdDidClicked();

        void RewardVideoAdDidRewardEffective();

        void RewardVideoAdDidPlayFinish();

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorrelateLib;

namespace Assets
{
    public static class DtoExtension
    {
        public static void CloneData(this PlayerData data, PlayerDataDto dto)
        {
            data.Level = dto.Level;
            data.Exp = dto.Exp;
            data.YuanBao = dto.YuanBao;
            data.YvQue = dto.YvQue;
            data.Stamina = dto.Stamina;
            data.ForceId = dto.ForceId;
            data.LastJinNangRedeemTime = dto.LastJinNangRedeemTime;
            data.DailyJinNangRedemptionCount = dto.DailyJinNangRedemptionCount;
            data.LastJiuTanRedeemTime = dto.LastJiuTanRedeemTime;
            data.DailyJiuTanRedemptionCount = dto.DailyJiuTanRedemptionCount;
            data.LastFourDaysChestRedeemTime = dto.LastFourDaysChestRedeemTime;
            data.LastWeekChestRedeemTime = dto.LastWeekChestRedeemTime;
            data.LastGameVersion = dto.LastGameVersion;
            data.AdPass = dto.AdPass;
        }

        public static bool IsValidCharacter(this ICharacter c) => !(c.Name == default && c.Nickname == default &&
                                                                    c.Sign == default && c.Avatar == default &&
                                                                    c.Gender == default && c.Rank == default);
    }
}

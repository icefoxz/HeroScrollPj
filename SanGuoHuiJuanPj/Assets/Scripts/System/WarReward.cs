using System.Collections.Generic;

public class WarReward
{
    public string Token { get; }
    public int WarId { get; }
    public int Gold { get; set; }
    public int BaYeExp { get; set; }
    public int Exp { get; set; }
    public int YuanBao { get; set; }
    public int YuQue { get; set; }
    public int Stamina { get; set; }
    public List<int> Chests { get; }
    public Dictionary<int,int> Ling { get; }

    public WarReward(string token,int warId,int staminaTurn)
    {
        Token = token;
        WarId = warId;
        Stamina = staminaTurn;
        Ling = new Dictionary<int, int>();
        Chests = new List<int>();
    }
    /// <summary>
    /// 这个方法仅仅用于旧UI，
    /// 0= Gold,
    /// 1= Exp,
    /// 2= Chest,
    /// 3= YuanBao,
    /// 4= YvQue,
    /// </summary>
    public Dictionary<int, int> ToRewardMap(bool isBaYe)
    {
        return new Dictionary<int, int>
        {
            {0, Gold},
            {1, isBaYe ? BaYeExp : Exp},
            {2, Chests.Count},
            {3, YuanBao},
            {4, YuQue}
        };
    }
}
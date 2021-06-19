using System.Collections.Generic;
using System.Linq;
using CorrelateLib;
using Random= UnityEngine.Random;

namespace Assets.HttpUnitScripts
{
    public class MapService
    {
        public Queue<ICharacter> Characters { get; private set; }
        public IReadOnlyList<WhiteCard> NpcWhiteCards { get; }
        private bool isRequestingCharacter;
        public MapService()
        {
            Characters = new Queue<ICharacter>();
            NpcWhiteCards = DataTable.Hero.Values.Where(h => h.Rarity == 1)
                .Select(h => new WhiteCard(GameCardInfo.GetInfo(GameCardType.Hero, h.Id))).ToList();
        }

        public void Init() => RequestingOnlineCharactersApi();

        private void GenerateCards(CharacterDto[] characters, int repeatAmount = 1)
        {
            Characters.Clear();
            for (int j = 0; j < repeatAmount; j++)
            {
                var list = characters.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    var cha = list[Random.Range(0, list.Count - i)];
                    list.Remove(cha);
                    Characters.Enqueue(cha);
                }
            }
        }

        public ICharacter[] GetCharacters(int amount)
        {
            var list = new List<ICharacter>();
            for (int i = 0; i < amount ; i++)
            {
                if (Characters.Count == 0) return list.ToArray();
                list.Add(Characters.Dequeue());
            }
            return list.ToArray();
        }

        public WhiteCard GetWhiteCard()
        {
            //优先获取角色.没有了再给出数据表的npc
            if (Characters.Any()) return new WhiteCard(Characters.Dequeue());
            if (!isRequestingCharacter)
                RequestingOnlineCharactersApi();
            return NpcWhiteCards[Random.Range(0, NpcWhiteCards.Count)];
        }

        private void RequestingOnlineCharactersApi()
        {
            isRequestingCharacter = true;
            ApiPanel.instance.Invoke(OnCharactersApiRespond,OnFailedToGetCharacters, EventStrings.Req_OnlineCharacters,ViewBag.Instance());
        }

        private void OnFailedToGetCharacters(string failedMessage)
        {
            isRequestingCharacter = false;
            XDebug.Log<MapService>(failedMessage);
        }

        private void OnCharactersApiRespond(ViewBag vb)
        {
            isRequestingCharacter = false;
            GenerateCards(vb.GetCharacterDtos());
        }
    }

    public class WhiteCard : ICharacter
    {
        public bool IsCharacter => Character != null;
        public GameCardInfo CardInfo { get; }
        public ICharacter Character { get; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public int Gender { get; set; }
        public int Avatar { get; set; }
        public string Sign { get; set; }
        public int Settle { get; set; }
        public int Rank { get; set; }

        public WhiteCard(ICharacter c)
        {
            Name = c.Name;
            Nickname = c.Nickname;
            Gender = c.Gender;
            Avatar = c.Avatar;
            Sign = c.Sign;
            Settle = c.Settle;
            Rank = c.Rank;
            Character = c;
        }

        public WhiteCard(GameCardInfo c)
        {
            Name = c.Name;
            Nickname = c.Intro;
            Gender = 1;
            Avatar = c.ImageId;
            Sign = c.About;
            Settle = 0;
            Rank = 0;
            CardInfo = c;
        }
    }
}

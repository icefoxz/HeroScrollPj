using System.Collections.Generic;
using System.Linq;
using CorrelateLib;
using Random= UnityEngine.Random;

namespace Assets.HttpUnitScripts
{
    public class MapService
    {
        public Queue<ICharacter> Characters { get; private set; }

        private bool isRequestingCharacter;
        public MapService() => Characters = new Queue<ICharacter>();

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

        public bool GetCharacterInRandom(int randomValue,out ICharacter cha)
        {
            cha = null;
            if(Random.Range(0, 100) > randomValue) return false;
            cha = GetCharacter();
            return cha != null;
        }

        public ICharacter GetCharacter()
        {
            //优先获取角色.没有了再给出数据表的npc
            if (Characters.Any()) return Characters.Dequeue();
            if (!isRequestingCharacter)
                RequestingOnlineCharactersApi();
            return null;
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
}

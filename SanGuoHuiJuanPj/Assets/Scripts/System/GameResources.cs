using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public enum ForceFlags
{
    蜀 = 0,
    魏 = 1,
    吴 = 2,
    袁 = 3,
    吕 = 4
}

public class GameResources 
{
    private Dictionary<int, Sprite> heroImgMap;
    private Dictionary<int, Sprite> classImgMap;
    private Dictionary<int, Sprite> fuZhuImgMap;
    private Dictionary<int, Sprite> gradeImgMap;
    private Dictionary<int, Sprite> guanQiaEventMap;
    private Dictionary<int, Sprite> frameImgMap;
    private Dictionary<int, Sprite> artWindowMap;
    private Dictionary<int, Sprite> battleBgMap;
    private Dictionary<ForceFlags, Sprite> forceFlagMap;
    private Dictionary<ForceFlags, Sprite> forceNameMap;
    private const string HeroImagesPath = "Image/Cards/Hero/";
    private const string ClassImagesPath = "Image/classImage/";
    private const string FuZhuImagesPath = "Image/Cards/FuZhu/";
    private const string GradeImagesPath = "Image/gradeImage/";
    private const string GuanQiaEventImagesPath = "Image/guanQiaEvents/";
    private const string FrameImagesPath = "Image/frameImage/";
    private const string ArtWindowImagesPath = "Image/ArtWindow/";
    private const string BattleBgImagesPath = "Image/battleBG/";
    private const string EffectsGameObjectPath = "Prefabs/Effects/";
    private const string StateDinPath = "Prefabs/stateDin/";
    private const string ForceFlagsPath = "Image/shiLi/Flag";
    private const string ForceNamePath = "Image/shiLi/Name";
    /// <summary>
    /// Key = heroId, Value = sprite
    /// </summary>
    public IReadOnlyDictionary<int, Sprite> HeroImg => heroImgMap;
    public IReadOnlyDictionary<int, Sprite> ClassImg => classImgMap;
    /// <summary>
    /// key = imgId, value = sprite
    /// </summary>
    public IReadOnlyDictionary<int, Sprite> FuZhuImg => fuZhuImgMap;
    public IReadOnlyDictionary<int, Sprite> GradeImg => gradeImgMap;
    public IReadOnlyDictionary<int, Sprite> GuanQiaEventImg => guanQiaEventMap;
    public IReadOnlyDictionary<int, Sprite> FrameImg => frameImgMap;
    public IReadOnlyDictionary<int, Sprite> ArtWindow => artWindowMap;
    public IReadOnlyDictionary<int, Sprite> BattleBG => battleBgMap;
    public IReadOnlyDictionary<ForceFlags, Sprite> ForceFlag => forceFlagMap;
    public IReadOnlyDictionary<ForceFlags, Sprite> ForceName => forceNameMap;
    public IReadOnlyDictionary<string, GameObject> Effects => effectsMap;
    public IReadOnlyDictionary<string, GameObject> StateDin => stateDinMap;

    private bool isInit;
    private Dictionary<string, GameObject> effectsMap;
    private Dictionary<string, GameObject> stateDinMap;

    public void Init(bool forceReload = false)
    {
        if (isInit && !forceReload) return;
        var heroTable =
            DataTable.HeroData
                .Where(row=>row.Count>16 && !string.IsNullOrWhiteSpace(row[1]))
                .Select(row => new {heroId = int.Parse(row[0]), imageId = int.Parse(row[16])});
         var list = Resources.LoadAll<Sprite>(HeroImagesPath)
             .Select(o => new {imageId = int.Parse(o.name), sprite = o})
            .Join(heroTable, c => c.imageId, t => t.imageId,
                (c, t) => new {t.heroId, c.sprite}).ToList();
        heroImgMap  =  list.ToDictionary(map => map.heroId, map => map.sprite);
        classImgMap = Resources.LoadAll<Sprite>(ClassImagesPath).ToDictionary(s => int.Parse(s.name), s => s);
        fuZhuImgMap = Resources.LoadAll<Sprite>(FuZhuImagesPath).ToDictionary(s => int.Parse(s.name), s => s);
        gradeImgMap = Resources.LoadAll<Sprite>(GradeImagesPath).ToDictionary(s => int.Parse(s.name), s => s);
        guanQiaEventMap = Resources.LoadAll<Sprite>(GuanQiaEventImagesPath).Where(s=>int.TryParse(s.name,out _)).ToDictionary(s => int.Parse(s.name), s => s);
        frameImgMap = Resources.LoadAll<Sprite>(FrameImagesPath).ToDictionary(s => int.Parse(s.name), s => s);
        artWindowMap = Resources.LoadAll<Sprite>(ArtWindowImagesPath).ToDictionary(s => int.Parse(s.name), s => s);
        battleBgMap = Resources.LoadAll<Sprite>(BattleBgImagesPath).ToDictionary(s => int.Parse(s.name), s => s);
        forceFlagMap = Resources.LoadAll<Sprite>(ForceFlagsPath).ToDictionary(s => (ForceFlags) int.Parse(s.name), s => s);
        forceNameMap = Resources.LoadAll<Sprite>(ForceNamePath).ToDictionary(s => (ForceFlags) int.Parse(s.name), s => s);
        effectsMap = Resources.LoadAll<GameObject>(EffectsGameObjectPath).ToDictionary(g => g.name, g => g);
        stateDinMap = Resources.LoadAll<GameObject>(StateDinPath).ToDictionary(g => g.name, g => g);
    }
}
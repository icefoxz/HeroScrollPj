using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameResources 
{
    private Dictionary<int, Sprite> heroImgMap;
    private Dictionary<int, Sprite> classImgMap;
    private Dictionary<int, Sprite> fuZhuImgMap;
    private Dictionary<int, Sprite> gradeImgMap;
    private const string HeroImages = "Image/Cards/Hero/";
    private const string ClassImages = "Image/classImage/";
    private const string FuZhuImages = "Image/Cards/FuZhu/";
    private const string GradeImages = "Image/gradeImage/";
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
    private bool isInit;

    public void Init(bool forceReload = false)
    {
        if (isInit && !forceReload) return;
        var heroTable =
            LoadJsonFile.heroTableDatas.Select(
                row => new {heroId = int.Parse(row[0]), imageId = int.Parse(row[16])});
        heroImgMap = Resources.LoadAll<Sprite>(HeroImages)
            .Select(o => new {imageId = int.Parse(o.name), sprite = o})
            .Join(heroTable, c => c.imageId, t => t.imageId,
                (c, t) => new {t.heroId, c.sprite})
            .ToDictionary(map => map.heroId, map => map.sprite);
        classImgMap = Resources.LoadAll<Sprite>(ClassImages).ToDictionary(s => int.Parse(s.name), s => s);
        fuZhuImgMap = Resources.LoadAll<Sprite>(FuZhuImages).ToDictionary(s => int.Parse(s.name), s => s);
        gradeImgMap = Resources.LoadAll<Sprite>(GradeImages).ToDictionary(s => int.Parse(s.name), s => s);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BarrageUiController : MonoBehaviour
{
    public BarrageUi Prefab;
    public List<BarrageUi> List { get; } = new List<BarrageUi>();

    public void PlayBarrage(string message)
    {
        var ui = List.FirstOrDefault(b => !b.gameObject.activeSelf);
        if (ui == null)
        {
            ui = Instantiate(Prefab, transform);
            List.Add(ui);
        }

        ui.Message.text = message;
        StartCoroutine(PlayBarrage(ui));
    }

    //回收弹幕框
    IEnumerator PlayBarrage(BarrageUi ui)
    {
        ui.transform.localPosition = new Vector2(ui.transform.position.x, Random.Range(-400, 600));
        while (true)
        {
            ui.gameObject.SetActive(true);
            yield return new WaitForSeconds(7.5f);
            ui.transform.localPosition = new Vector2(ui.transform.position.x, 0);
            ui.gameObject.SetActive(false);
        }
    }

}

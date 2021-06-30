using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class ConsumeBtnUi : MonoBehaviour
{
    private enum States
    {
        None,
        Free,
        YuanBao,
        YuQue
    }
    public Image YuQue;
    public Image YuanBao;
    public Text Value;
    public Text Free;
    public Transform ConsumeBody;
    public Button Button;
    private UiStateMapper<Component> mapper;

    public virtual void Init()
    {
        mapper = new UiStateMapper<Component>(
            (States.Free, new Component[] {Free}),
            (States.YuQue, new Component[] {YuQue, ConsumeBody, Value}),
            (States.YuanBao, new Component[] {YuanBao, ConsumeBody, Value}));
        mapper.Set(States.None);
    }

    public void SetFree() => mapper.Set(States.Free);
    public void SetNone() => mapper.Set(States.None);

    public void SetYuanBao(int value)
    {
        mapper.Set(States.YuanBao);
        Value.text = value.ToString();
    }

    public void SetYuQue(int value)
    {
        mapper.Set(States.YuQue);
        Value.text = value.ToString();
    }
}
using UnityEngine;
using UnityEngine.UI;


public class GrenadeCounter : MonoBehaviour
{
    public Image Icon;

    public Text Text;

    public Image Panel;

    private int _count;


    public void SetColor(Color color)
    {
        Icon.color = color;
        Text.color = color;
    }

    public int GetCount()
    {
        return _count;
    }

    public void SetCount(int count)
    {
        _count = count;
        Text.text = $"{count}";
    }

    public void SetSelected(bool isSelected)
    {
        Panel.enabled = isSelected;
    }
}

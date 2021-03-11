using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextMeshProColorPingPong : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Color Color1;
    public Color Color2;
    public float SpeedModifier = 1;

    private void Update()
    {
        text.color = GameUtils.PingPongColors(SpeedModifier, Color1, Color2);
    }
}

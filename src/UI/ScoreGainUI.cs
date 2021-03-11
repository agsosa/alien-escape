using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreGain
{
    public ScoreGainType Type;
    public int Score;
    public float Time;
}

public class ScoreGainUI : MonoBehaviour
{
    public float UpdateRate = 0.1f;
    Queue<ScoreGain> ScoreGainQueue = new Queue<ScoreGain>();
    public List<ScoreGainContainer> Containers = new List<ScoreGainContainer>();

    public void ShowScoreGain(ScoreGainType type, int Score, float Time)
    {
        ScoreGain n = new ScoreGain();
        n.Type = type;
        n.Score = Score;
        n.Time = Time;
        ScoreGainQueue.Enqueue(n);
    }

    float NextUpdate = 0;
    private void Update()
    {
        if (ScoreGainQueue.Count >= 1)
        {
            if (Time.time > NextUpdate)
            {
                foreach(ScoreGainContainer c in Containers)
                {
                    if (c.IsAvailable)
                    {
                        ScoreGain obj = ScoreGainQueue.Dequeue();
                        c.Show(obj);
                        break;
                    }
                }

                NextUpdate = Time.time + UpdateRate;
            }
        }
    }
}

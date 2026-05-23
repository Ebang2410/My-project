using UnityEngine;

public class UiSetting : MonoBehaviour
{
    [SerializeField] GameObject ScoreTime;
    bool _scoreEnable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScoreTime.SetActive(false);
        _scoreEnable = false;
    }

    public void EnableScore()
    {
        _scoreEnable = !_scoreEnable;

        ScoreTime.SetActive(_scoreEnable);
    }
}

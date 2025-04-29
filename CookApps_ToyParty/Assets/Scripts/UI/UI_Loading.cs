using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : UIBase
{
    public const string prefabName = "UI_Loading";

    public Slider _slider = null;
    private const float _loadingTime = 2f;
    private float _time = 0f;

    protected override void Init()
    {
        base.Init();

        _time = 0f;
        if(_slider != null)
        {
            _slider.minValue = 0f;
            _slider.maxValue = 1f;
            _slider.value = _slider.minValue;
        }
    }

    private void Update()
    {
        _time += Time.unscaledDeltaTime;
        if (_slider != null)
            _slider.value = _time / _loadingTime;

        if (_time >= _loadingTime)
        {
            Close();
            return;
        }
    }

}

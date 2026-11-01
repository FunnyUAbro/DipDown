using UnityEngine;
using UnityEngine.UI;

public class SliderPanel : MonoBehaviour
{
    [SerializeField] RectTransform sliderRT;

    Gun gun;

    void Start()
    {
        DisableDisplay();
    }

    void Update()
    {
        SetSliderPosition();
    }

    public void StartDisplay(Gun newGun)
    {
        this.gameObject.SetActive(true);
        gun = newGun;
        CancelInvoke();
    }

    public void StopDisplay()
    {
        const float c_disable_time = 1.5f;
        Invoke("DisableDisplay", c_disable_time);
    }

    void DisableDisplay()
    {
        this.gameObject.SetActive(false);

    }

    public void ShowAmmo(byte ammo)
    {
        //Когда доехали до края
        
    }

    public void SetSliderPosition()
    {
        //Принимаем от 0 до 1 превращаем в позицию слайжера

        sliderRT.anchoredPosition = new Vector2(gun.boltPositionCoef * -790f, 0);
    }
}

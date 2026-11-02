using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderPanel : MonoBehaviour
{
    [SerializeField] RectTransform sliderRT;
    [SerializeField] TMP_Text ammoText;

    Gun gun;

    void Start()
    {
        DisableDisplay();
    }

    void Update()
    {
        SetSliderPosition();

        if (gun.boltPositionCoef > 0.5f)
        {
            ShowAmmo();
        }
        else
        {
            ammoText.text = null;
        }
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

    void ShowAmmo()
    {
        ammoText.text = $"{gun.ammo.ToString()}/{gun.maxAmmo}";
    }

    public void SetSliderPosition()
    {
        sliderRT.anchoredPosition = new Vector2(gun.boltPositionCoef * -790f, 0);
    }
}

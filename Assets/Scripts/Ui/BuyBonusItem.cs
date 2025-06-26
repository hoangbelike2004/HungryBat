using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyBonusItem : MonoBehaviour
{
    [SerializeField] Button btnBuy;
    [SerializeField] TextMeshProUGUI txtName,txtDescription,txtPrice;
    [SerializeField] Transform parent;
    private BonusData bonusdata;
    private CanvasMain canvasmain;
    private void Start()
    {
        btnBuy.onClick.AddListener(() =>
        {
            //kiểm tra và mua item;
            if(GameController.Instance.GetCoin() < bonusdata.price)
            {
                canvasmain.ActiveNotification(false);
            }
            else
            {
                GameController.Instance.SetCoin(bonusdata.price);
                canvasmain.UpdateCoin(GameController.Instance.GetCoin());
                bonusdata.amout += 1;
                canvasmain.ActiveNotification(true);
            }
        });
    }
    public void SetCanvasMain(CanvasMain canvas)
    {
        this.canvasmain = canvas;
    }
    public void SetData(BonusData bonusdata)
    {
        this.bonusdata = bonusdata;
        UpdateUI();
    }
    private void UpdateUI()
    {
        txtName.text = bonusdata.name;
        txtDescription.text = bonusdata.description;
        txtPrice.text = bonusdata.price.ToString();
        GameObject prefab = Instantiate(bonusdata.prefab,parent);
    }
}

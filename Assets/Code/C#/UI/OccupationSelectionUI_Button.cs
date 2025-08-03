using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 职业选择UI按钮
/// </summary>
public class OccupationSelectionUI_Button : MonoBehaviour
{
    [SerializeField] private Transform modleViewPoint;
    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private Button button;
    public int occupationID;

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            PlayerManager.Instance.ChangePlayerOccupation(occupationID);
            UIManager.Instance.HideOccupationSelectionUI();
        });
    }
    public void SetOccupation(int occupationID)
    {
        this.occupationID = occupationID;
        Instantiate(PlayerManager.Instance.GetOccupationDataSO(occupationID).Model, modleViewPoint);
        text.text = PlayerManager.Instance.GetOccupationDataSO(occupationID).CharacterName
            + "\n"
            + PlayerManager.Instance.GetOccupationDataSO(occupationID).CharacterDescription;     
    }

}

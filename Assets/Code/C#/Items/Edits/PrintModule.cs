using System.Collections.Generic;
using UnityEngine;

public class PrintModule : MonoBehaviour, IAble2Edit, IAble2BagInteraction_Scrollbar, IApplyPlayerPrefsData
{
    private int anglesCount;
    private float offsetAngle;
    private float[] angles;
    private float useHonkaiPerSecond;
    private int printCount;
    private int currentAnglesCount = 0;
    private int currentPrintCount = 0;
    private EditableBullet editableBullet;
    public override string ToString()
    {
        return "angles: " + angles + " useHonkaiPerSecond: " + useHonkaiPerSecond + " printCount: " + printCount;
    }

    public Dictionary<string, float> GetScrollbarMultipliers()
    {
        Dictionary<string, float> dict = new Dictionary<string, float>
        {
            { "useHonkaiPerSecond", 60 },
            { "printCount", 10 },
            { "anglesCount", 8 },
            {"offsetAngle" ,360},
            {"angle",360 }
        };
        return dict;
    }

    public void SetScrollbarValues(float[] values)
    {
        useHonkaiPerSecond = values[0];
        printCount = (int)values[1];
        anglesCount = (int)values[2];
        angles = new float[anglesCount];
        offsetAngle = values[3];
        for (int i = 0; i < anglesCount; i++)
        {
            angles[i] = offsetAngle + values[4] * i / anglesCount;
        }

    }

    public void ApplyData()
    {

        useHonkaiPerSecond = PlayerPrefs.GetFloat("useHonkaiPerSecond") * 60;
        printCount = (int)(PlayerPrefs.GetFloat("printCount") * 10);
        anglesCount = (int)(PlayerPrefs.GetFloat("anglesCount") * 8);
        angles = new float[anglesCount];
        offsetAngle = PlayerPrefs.GetFloat("offsetAngle") * 360;
        for (int i = 0; i < anglesCount; i++)
        {
            angles[i] = offsetAngle + PlayerPrefs.GetFloat("angle") * 360 * i / anglesCount;
        }
        //Debug.Log(useHonkaiPerSecond);
    }


    public int Edit(GameObject self, InventorySO inventory, int index)
    {
        editableBullet = self.GetComponent<EditableBullet>();
        editableBullet.printTimer += useHonkaiPerSecond * Time.deltaTime;
        //Debug.Log("printTimer: " + editableBullet.printTimer);
        //Debug.Log(useHonkaiPerSecond);

        if (editableBullet.printTimer > inventory.GetComplexity(index + currentPrintCount + 1) && PlayerManager.Instance.GetPlayer().ChangeMagicAmount(-inventory.GetComplexity(index + currentPrintCount + 1)))
        {
            //Debug.Log("print");
            editableBullet.printTimer -= inventory.GetComplexity(index + currentPrintCount + 1);
            Quaternion quaternion = Quaternion.Euler(0, 0, angles[currentAnglesCount]);
            GameObject obj = inventory.GetGameObject(index + currentPrintCount + 1);

            if (obj && obj.TryGetComponent<BaseBullet>(out BaseBullet bullet))
                BulletManager.Instance.GenerateBullet(inventory.GetGameObject(index + currentPrintCount + 1), self.transform, quaternion, inventory.GetItem(index + currentPrintCount + 1));
            //if (obj && obj.TryGetComponent<BaseTurret>(out BaseTurret turret))
            //{
            //    GameObject go = Instantiate(inventory.GetGameObject(index + currentPrintCount + 1), self.transform.position, quaternion);
            //    //go.SetActive(true);
            //    go.GetComponent<IAble2Right>().OnRight();
            //    Destroy(go);
            //}
            currentPrintCount++;
            if (currentPrintCount >= printCount)
            {
                currentPrintCount = 0;
                currentAnglesCount++;
            }

            if (currentAnglesCount >= angles.Length)
                currentAnglesCount = 0;
        }

        if (self.activeSelf == false)
        {
            currentPrintCount = 0;
            currentAnglesCount = 0;
        }


        return index;
    }

}

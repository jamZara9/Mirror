using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory_Container
{
    [SerializeField]
    private List<GameObject> Container;

    public void Initialize()
    {
        Container = new List<GameObject>(3);
        Debug.Log("Create Container");
    }

    public void Add_Item(GameObject _obj, string _str)
    {
        GameObject Item = Container.Find(x => x.GetComponent<BaseItem>().itemData.name == _str);

        if (null == Item)
            Container.Add(_obj); // 기존 인벤토리에 없던 새로운 아이템 -> 컨테이너에 추가
        else
            Item.GetComponent<BaseItem>().itemData.count++; // 기존 인벤토리에 있던 아이템 -> 갯수 추가
    }

    public GameObject Get_Item(string _name)
    {
        GameObject Item = Container.Find(x => x.GetComponent<BaseItem>().itemData.name == _name);
        if (null != Item)
            return Item;

        //for (int i = 0; i < Container.Count; i++)
        //{
        //    if (Container[i].GetComponent<BaseItem>().itemData.name == _name)
        //        return Container[i];
        //}

        Debug.Log("Item not found");
        return null;
    }

    public void Use_Item(int _idx)
    {
        if (_idx >= Container.Count)
        {
            Debug.Log("index is bigger then Container Size");
            return;
        }

        Container[_idx].GetComponent<BaseItem>().UseItem();

        if (--Container[_idx].GetComponent<BaseItem>().itemData.count <= 0)
            Remove_Item(_idx);
    }

    public void Remove_Item(int _idx)
    {
        //삭제 조건 추후 추가

        Container.Remove(Container[_idx]);
    }
}
using UnityEngine;

// 아이템을 가지고 있는 클래스가 구현해야 하는 인터페이스
public interface IItemContainer
{
    public void AddItem(IInventoryItem item);
    public void RemoveItem(IInventoryItem item);
}

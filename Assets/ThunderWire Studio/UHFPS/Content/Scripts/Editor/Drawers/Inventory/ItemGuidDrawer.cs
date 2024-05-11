using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UHFPS.Runtime;
using UHFPS.Scriptable;
using ThunderWire.Editors;

namespace UHFPS.Editors
{
    [CustomPropertyDrawer(typeof(ItemGuid))]
    public class ItemGuidDrawer : PropertyDrawer
    {
        readonly InventoryAsset inventoryAsset;
        readonly bool hasInvReference;

        public ItemGuidDrawer()
        {
            if (Inventory.HasReference)
            {
                inventoryAsset = Inventory.Instance.inventoryAsset;
                hasInvReference = true;
            }
        }

        public Item GetItemRaw(string guid)
        {
            if (!string.IsNullOrEmpty(guid) && hasInvReference && inventoryAsset != null)
            {
                foreach (var item in inventoryAsset.Items)
                {
                    if (item.guid == guid)
                        return item.item;
                }
            }

            return null;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            SerializedProperty guid = property.FindPropertyRelative("GUID");
            GUIContent buttonContent = new GUIContent("Select Item");

            Item item = GetItemRaw(guid.stringValue);

            if (!hasInvReference)
            {
                buttonContent.text = "<color=#ED213A>Inventory component reference is missing!</color>";
            }
            else if (inventoryAsset == null)
            {
                buttonContent.text = "<color=#ED213A>Inventory asset not defined!</color>";
            }
            else if(item != null)
            {
                buttonContent = EditorGUIUtility.TrTextContentWithIcon(item?.Title, "Prefab On Icon");
            }

            Rect dropdownRect = position;
            dropdownRect.width = 250f;
            dropdownRect.height = 0f;
            dropdownRect.y += EditorGUIUtility.singleLineHeight;
            dropdownRect.x += position.xMax - dropdownRect.width - EditorGUIUtility.singleLineHeight;

            if (EditorDrawing.ObjectField(position, buttonContent))
            {
                ItemPropertyDrawer.ItemPicker itemPicker = new (new AdvancedDropdownState(), inventoryAsset);
                itemPicker.OnItemPressed += obj =>
                {
                    guid.stringValue = obj.guid;
                    property.serializedObject.ApplyModifiedProperties();
                };

                itemPicker.Show(dropdownRect);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Managers
{
    public class PanelManager : MonoBehaviour
    {
        public const int SLOT_COUNT = 5;
        private int _slotSpot = 0;
        [SerializeField] List<TMP_Text> _slotsText;
        [SerializeField] List<Image> _slotImages;
        [SerializeField] List<Slot> _slots;
        [SerializeField] KeyboardManager _keyManager;
        [SerializeField] Color _defaultCol;
        [SerializeField] Color _wrongCol;
        [SerializeField] Color _wrongPosCol;
        [SerializeField] Color _corrCol;
        public enum SlotState { DefaultState, Wrong, WrongPos, Correct };

        void Start()
        {
            _slots = new();
            for (int i = 0; i < SLOT_COUNT; i++)
            {
                _slots.Add(new());
                _slots[i].image = _slotImages[i];
                _slots[i].text = _slotsText[i];
            }
        }

        public void ResetPanel()
        {
            _slotSpot = 0;
            for (int i = 0; i < SLOT_COUNT; i++)
            {
                _slots[i].text.text = " ";
                _slots[i].state = SlotState.DefaultState;
                ChangeSlotColor(i, SlotState.DefaultState);
            }
        }

        public void ChangeSlotColor(int index, SlotState state)
        {
            int cursed = (int)_slots[index].text.text.ToCharArray()[0];
            switch (state)
            {
                case SlotState.Wrong:
                    if ((int)_slots[index].state < 2)
                    {
                        _slots[index].image.color = _wrongCol;
                        _keyManager.ChangeColor(KeyboardManager.KeyState.wrong, cursed);
                    }
                    break;

                case SlotState.WrongPos:
                    if (_slots[index].state != SlotState.Correct)
                    {
                        _slots[index].image.color = _wrongPosCol;
                        _keyManager.ChangeColor(KeyboardManager.KeyState.wrongPos, cursed);
                    }
                    break;

                case SlotState.Correct:
                    _slots[index].image.color = _corrCol;
                    _keyManager.ChangeColor(KeyboardManager.KeyState.correct, cursed);
                    break;

                default:
                    _slots[index].image.color = _defaultCol;
                    break;
            }
        }

        public void ChangeChar(string input, bool add)
        {
            //_slotsText[_slotSpot].text = input;

            if (add)
            {
                _slots[_slotSpot].text.text = input;
                _slotSpot++;
            }
            else
            {
                _slotSpot--;
                _slots[_slotSpot].text.text = " ";
            }
        }

        private class Slot
        {
            public TMP_Text text;
            public Image image;
            public SlotState state = SlotState.DefaultState;
        }
    }
}
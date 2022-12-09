using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class KeyboardManager : MonoBehaviour
    {
        private string _word = "";
        private const int NUM_OF_KEYS = 26;
        [SerializeField] private Button[] keyBoard;
        [SerializeField] private GameManager _gM;
        [SerializeField] WordList _wordList;
        [SerializeField] Color _defaultCol;
        [SerializeField] Color _wrong;
        [SerializeField] Color _wrongPos;
        [SerializeField] Color _correctPos;

        public enum KeyState { defaultState, wrong, wrongPos, correct }

        public void Reset()
        {
            for (int i = 0; i < NUM_OF_KEYS; i++)
            {
                Image image = keyBoard[i].GetComponent<Image>();
                image.color = _defaultCol;
            }
        }

        public void ChangeColor(KeyState state, int index)
        {
            if (index == 32)
                return;
            index -= 65;

            Image image = keyBoard[index].GetComponent<Image>();
            switch (state)
            {
                case KeyState.correct:
                    keyBoard[index].GetComponent<Image>().color = _correctPos;
                    break;

                case KeyState.wrongPos:
                    if (image.color != _correctPos)
                        image.color = _wrongPos;
                    break;

                case KeyState.wrong:
                    if (image.color != _correctPos && image.color != _wrongPos)
                        image.color = _wrong;
                    break;

                default:
                    image.color = _defaultCol;
                    break;
            }
        }

        public void KeyInput(string car)
        {
            if (_word.Length < 5)
            {
                _word += car;
                _gM.AddCharToUi(car);
            }
        }

        public void DeleteInput()
        {
            if (_word.Length > 0)
            {
                _word = _word.Substring(0, _word.Length - 1);
                _gM.RemoveCharFromUi();
            }
        }

        public void Enter()
        {
            if (_word.Length == 5 && _wordList.WordExists(_word.ToLower()))
            {
                _gM.GuessWord(_word.ToLower());
                _word = "";
            }
            else
                Debug.Log("Word doesn't exist");
        }
    }
}
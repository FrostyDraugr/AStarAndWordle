using UnityEngine;
using System.Collections.Generic;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private const int PANEL_COUNT = 5;
        [SerializeField] List<PanelManager> _panelManagers;
        [SerializeField] WordList _wordList;
        [SerializeField] KeyboardManager _keyManager;
        [SerializeField] List<GameObject> _panels;
        private int _panelIndex = 0;
        private string _chosenWord;
        [SerializeField] GameObject _victoryScreen;
        [SerializeField] GameObject _defeatScreen;
        [SerializeField] GameObject _keyBoard;


        private void Start()
        {
            _chosenWord = _wordList.GetWord();
            var hashSet = new HashSet<string>();
            Debug.Log(":" + _chosenWord + ":");
            _panelIndex = 0;
        }

        public void Reset()
        {
            _victoryScreen.SetActive(false);
            _defeatScreen.SetActive(false);
            _panelIndex = 0;
            for (int i = 0; i < PANEL_COUNT; i++)
            {
                _panelManagers[i].ResetPanel();
            }
            _keyManager.Reset();
            _keyBoard.SetActive(true);
            _chosenWord = _wordList.GetWord();
        }

        public void GuessWord(string input)
        {
            if (input == _chosenWord)
            {
                for (int i = 0; i < 5; i++)
                {
                    _panelManagers[_panelIndex].ChangeSlotColor(i, PanelManager.SlotState.Correct);
                    _keyBoard.SetActive(false);
                    _victoryScreen.SetActive(true);
                }
            }
            else if (_panelIndex < PANEL_COUNT)
            {
                //Check letters and positions in panel
                char[] charArr = input.ToCharArray();
                char[] corrArr = _chosenWord.ToCharArray();
                for (int i = 0; i < charArr.Length; i++)
                {
                    if (charArr[i] == corrArr[i])
                    {
                        _panelManagers[_panelIndex].ChangeSlotColor(i, PanelManager.SlotState.Correct);
                    }
                    else
                    {
                        for (int j = 0; j < corrArr.Length; j++)
                        {
                            if (charArr[i] == corrArr[j])
                            {
                                _panelManagers[_panelIndex].ChangeSlotColor(i, PanelManager.SlotState.WrongPos);
                                break;
                            }

                            if (j == corrArr.Length - 1)
                                _panelManagers[_panelIndex].ChangeSlotColor(i, PanelManager.SlotState.Wrong);
                        }
                    }
                }
                _panelIndex++;
                if (_panelIndex >= PANEL_COUNT)
                {
                    _keyBoard.SetActive(false);
                    _defeatScreen.SetActive(true);
                    _defeatScreen.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text =
                    "You lost, the word was: " + _chosenWord.ToUpper();
                }
            }

        }

        public void AddCharToUi(string car)
        {
            _panelManagers[_panelIndex].ChangeChar(car, true);
        }

        public void RemoveCharFromUi()
        {
            _panelManagers[_panelIndex].ChangeChar("", false);
        }
    }
}
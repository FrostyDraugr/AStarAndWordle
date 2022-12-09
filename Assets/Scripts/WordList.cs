using UnityEngine;
using System.Collections.Generic;
using System.Collections;
namespace Managers
{
    public class WordList : MonoBehaviour
    {
        [SerializeField] private string[] _wordArr = new string[0];
        [SerializeField] TextAsset _wordDoc;
        [SerializeField] KeyboardManager _keyManager;
        [SerializeField] GameObject _invalidGuess;
        [SerializeField] GameObject _canvas;

        private HashSet<string> _hashSet;

        public string GetWord()
        {
            if (_wordArr.Length == 0)
            {
                _wordArr = _wordDoc.text.Split('\n');
                for (int i = 0; i < _wordArr.Length; i++)
                {
                    _wordArr[i] = _wordArr[i].Trim();
                }

                _hashSet = new HashSet<string>(_wordArr);
            }

            return _wordArr[Random.Range(0, _wordArr.Length)];
        }

        public bool WordExists(string input)
        {
            bool returnThis = _hashSet.Contains(input);
            if (!returnThis)
            {
                StartCoroutine(InvalidGuess(2));
            }
            return returnThis;
        }

        IEnumerator InvalidGuess(float t)
        {
            GameObject obj = Instantiate(_invalidGuess, new(0, -4f, 0), Quaternion.identity, _canvas.transform);
            yield return new WaitForSeconds(t);
            Destroy(obj);
        }
    }
}
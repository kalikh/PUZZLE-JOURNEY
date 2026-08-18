using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CaravanSecrets.Features.Gameplay
{
    public sealed class HoldMoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private Action _move;
        private Coroutine _repeat;

        public void Bind(Action move) => _move = move;

        public void OnPointerDown(PointerEventData eventData)
        {
            _move?.Invoke();
            if (_repeat != null) StopCoroutine(_repeat);
            _repeat = StartCoroutine(Repeat());
        }

        public void OnPointerUp(PointerEventData eventData) => StopRepeating();
        public void OnPointerExit(PointerEventData eventData) => StopRepeating();

        private IEnumerator Repeat()
        {
            yield return new WaitForSecondsRealtime(0.42f);
            while (true)
            {
                _move?.Invoke();
                yield return new WaitForSecondsRealtime(0.27f);
            }
        }

        private void StopRepeating()
        {
            if (_repeat != null) StopCoroutine(_repeat);
            _repeat = null;
        }
    }
}

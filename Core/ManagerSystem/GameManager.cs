using GM.GameEventSystem;
using System.Collections;
using UnityEngine;

namespace GM.Managers
{
    public class GameManager : MonoBehaviour, IManagerable
    {
        [SerializeField] private GameEventChannelSO _gameCycleChannel;

        [SerializeField] private double _dayTime = 5;
        private double _currentDayTime = 0;

        private bool _isDayTimer;
        private bool _isStopDayTimer;
        private bool _isStopCustomer;

        private float duration;
        public float Duration => duration;

        private void Awake()
        {
            _gameCycleChannel.AddListener<ReadyToRestourant>(HandleReadyToRestourant);
        }

        public void Initialized()
        {
            _currentDayTime = 0;
            _isDayTimer = false;
            _isStopDayTimer = false;
            _isStopCustomer = false;
        }

        public void Clear()
        {
            _gameCycleChannel.RemoveListener<ReadyToRestourant>(HandleReadyToRestourant);
        }

        public double GetDayTime() => _currentDayTime;

        public void StopTimer() => _isStopDayTimer = true;
        public void PlayTimer() => _isStopDayTimer = false;

        public void RestourantOpen()
        {
            // Avoid duplication
            if (GameCycleEvents.RestourantCycleEvent.open == true) return;

            GameCycleEvents.RestourantCycleEvent.open = true;
            _gameCycleChannel.RaiseEvent(GameCycleEvents.RestourantCycleEvent);
        }

        private void HandleReadyToRestourant(ReadyToRestourant evt)
        {
            StartDayTimer();
        }

        private void StartDayTimer()
        {
            // Avoid duplication
            if (_isDayTimer == true) return;

            StartCoroutine(DayTimer());
        }

        private IEnumerator DayTimer()
        {
            _isDayTimer = true;
            _isStopCustomer = false;
            _currentDayTime = 0;

            while (_currentDayTime <= _dayTime)
            {
                yield return null;
                if (_isStopDayTimer == true) continue;

                _currentDayTime += Time.deltaTime;

                duration = (float)(_currentDayTime / _dayTime);
                if (duration > 0.7f && !_isStopCustomer)
                {
                    // 마감시간(손님 생성 멈춤)
                    _gameCycleChannel.RaiseEvent(GameCycleEvents.RestourantClosingTimeEvent);
                    _isStopCustomer = true;
                }
            }

            yield return new WaitUntil(() => ManagerHub.Instance.GetManager<DataManager>().CustomerCnt == 0);
            // Close(실질적 레스토랑 영업 종료)
            GameCycleEvents.RestourantCycleEvent.open = false;
            _gameCycleChannel.RaiseEvent(GameCycleEvents.RestourantCycleEvent);
            _isDayTimer = false;
        }
    }
}

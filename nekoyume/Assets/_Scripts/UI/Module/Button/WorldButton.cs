using System;
using DG.Tweening;
using Nekoyume.Game.Controller;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI.Module
{
    public class WorldButton : MonoBehaviour
    {
        private enum State
        {
            Unlocked,
            Locked
        }

        private enum AnimationState
        {
            None,
            Idle,
            Hover
        }

        [SerializeField]
        private Button button;

        [SerializeField]
        private Image grayImage;

        [SerializeField]
        private Image colorImage;

        [SerializeField]
        private Image nameImage;

        [SerializeField, Header("Direction"), Tooltip("대기 상태일 때 월드 이름이 스케일 되는 크기")]
        private float idleNameScaleTo = 1.1f;

        [SerializeField, Tooltip("대기 상태일 때 월드 이름이 스케일 되는 속도")]
        private float idleNameScaleSpeed = 0.7f;

        [SerializeField, Tooltip("마우스 호버 상태일 때 월드 버튼이 스케일 되는 크기")]
        private float hoverScaleTo = 1.1f;

        [SerializeField, Tooltip("마우스 호버 상태일 때 월드 버튼이 스케일 되는 속도")]
        private float hoverScaleSpeed = 0.7f;

        [SerializeField]
        private Image hasNotificationImage = null;

        private readonly ReactiveProperty<State> _state = new ReactiveProperty<State>(State.Locked);

        private readonly ReactiveProperty<AnimationState> _animationState =
            new ReactiveProperty<AnimationState>(AnimationState.None);

        private Tweener _tweener;

        public readonly Subject<WorldButton> OnClickSubject = new Subject<WorldButton>();
        public readonly ReactiveProperty<bool> HasNotification = new ReactiveProperty<bool>(false);

        private bool IsLocked => _state.Value == State.Locked;

        private void Awake()
        {
            var go = gameObject;
            go.AddComponent<ObservablePointerEnterTrigger>()
                .OnPointerEnterAsObservable()
                .Subscribe(x =>
                {
                    _animationState.SetValueAndForceNotify(IsLocked
                        ? AnimationState.None
                        : AnimationState.Hover);
                })
                .AddTo(go);

            go.AddComponent<ObservablePointerExitTrigger>()
                .OnPointerExitAsObservable()
                .Subscribe(x =>
                {
                    _animationState.SetValueAndForceNotify(IsLocked
                        ? AnimationState.None
                        : AnimationState.Idle);
                })
                .AddTo(go);

            button.OnClickAsObservable().Subscribe(OnClick).AddTo(go);
            HasNotification.SubscribeTo(hasNotificationImage).AddTo(go);
            _state.Subscribe(OnState).AddTo(go);
            _animationState.Subscribe(OnAnimationState).AddTo(go);
        }

        private void OnEnable()
        {
            _state.SetValueAndForceNotify(_state.Value);
            _animationState.SetValueAndForceNotify(_animationState.Value);
        }

        private void OnDisable()
        {
            _tweener?.Kill();
            _tweener = null;
        }

        public void Unlock()
        {
            _state.SetValueAndForceNotify(State.Unlocked);
        }

        public void Lock()
        {
            _state.SetValueAndForceNotify(State.Locked);
        }

        private void OnClick(Unit unit)
        {
            AudioController.PlayClick();
            OnClickSubject.OnNext(this);
        }

        private void OnState(State state)
        {
            switch (state)
            {
                case State.Unlocked:
                    button.interactable = true;
                    grayImage.enabled = false;
                    colorImage.enabled = true;
                    nameImage.enabled = true;
                    _animationState.SetValueAndForceNotify(AnimationState.Idle);
                    break;
                case State.Locked:
                    button.interactable = false;
                    grayImage.enabled = true;
                    colorImage.enabled = false;
                    nameImage.enabled = false;
                    _animationState.SetValueAndForceNotify(AnimationState.None);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void OnAnimationState(AnimationState state)
        {
            _tweener?.Kill();
            _tweener = null;

            transform.localScale = Vector3.one;
            nameImage.transform.localScale = Vector3.one;

            if (_state.Value == State.Locked)
            {
                return;
            }

            switch (state)
            {
                case AnimationState.None:
                    break;
                case AnimationState.Idle:
                    _tweener = nameImage.transform
                        .DOScale(idleNameScaleTo, 1f / idleNameScaleSpeed)
                        .SetEase(Ease.Linear)
                        .SetLoops(-1, LoopType.Yoyo);
                    break;
                case AnimationState.Hover:
                    _tweener = transform
                        .DOScale(hoverScaleTo, 1f / hoverScaleTo)
                        .SetEase(Ease.Linear)
                        .SetLoops(-1, LoopType.Yoyo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}

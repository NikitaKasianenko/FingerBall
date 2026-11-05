using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameHud : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private RectTransform _massFill;
        [SerializeField] private CanvasGroup _hint;
        [SerializeField] private CanvasGroup _resultScreen;
        [SerializeField] private TMP_Text _resultTitle;
        [SerializeField] private TMP_Text _resultMessage;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _restartButton;

        [Header("Texts")]
        [SerializeField] private string _levelFormat = "LEVEL {0}";
        [SerializeField] private string _winTitle = "Level complete!";
        [SerializeField] private string _loseTitle = "Game over";
        [SerializeField] private string _winMessage = "The ball made it through the door.";
        [SerializeField] private string _overchargedMessage = "The whole ball went into the shot. Release earlier!";
        [SerializeField] private string _outOfMassMessage = "Not enough ball left to clear the path.";

        [Header("Colors")]
        [SerializeField] private Color _winColor = new(1f, 0.85f, 0.35f);
        [SerializeField] private Color _loseColor = new(1f, 0.45f, 0.4f);

        [Header("Animation")]
        [SerializeField, Min(0f)] private float _fadeDuration = 0.25f;
        [SerializeField, Min(0f)] private float _titlePopDuration = 0.4f;
        [SerializeField, Min(0f)] private float _massTweenDuration = 0.15f;

        public event Action ButtonClicked;

        private void Awake()
        {
            _hint.alpha = 0f;
            SetVisible(_resultScreen, isVisible: false, instant: true);
        }

        public void ShowLevel(int levelNumber, float mass)
        {
            _levelLabel.text = string.Format(_levelFormat, levelNumber);
            _massFill.DOKill();
            SetMassAnchor(mass);
        }

        public void SetMass(float mass)
        {
            _massFill.DOKill();
            DOTween.To(() => _massFill.anchorMax.x, SetMassAnchor, Mathf.Clamp01(mass), _massTweenDuration)
                .SetTarget(_massFill);
        }

        public void SetHintVisible(bool isVisible)
        {
            _hint.DOKill();
            _hint.DOFade(isVisible ? 1f : 0f, _fadeDuration);
        }

        public void ShowResult(LevelResult result)
        {
            bool isWin = result == LevelResult.Win;
            _resultTitle.text = isWin ? _winTitle : _loseTitle;
            _resultTitle.color = isWin ? _winColor : _loseColor;
            _resultMessage.text = MessageFor(result);
            _nextButton.gameObject.SetActive(isWin);

            SetVisible(_resultScreen, isVisible: true, instant: false);
            _resultTitle.transform.DOKill();
            _resultTitle.transform.localScale = Vector3.one * 0.6f;
            _resultTitle.transform.DOScale(1f, _titlePopDuration).SetEase(Ease.OutBack);
        }

        public void HideResult() =>
            SetVisible(_resultScreen, isVisible: false, instant: false);

        public async UniTask<LevelChoice> WaitForChoiceAsync(CancellationToken cancellationToken)
        {
            int clicked = await UniTask.WhenAny(
                _nextButton.OnClickAsync(cancellationToken),
                _restartButton.OnClickAsync(cancellationToken));

            ButtonClicked?.Invoke();
            return clicked == 0 ? LevelChoice.Next : LevelChoice.Restart;
        }

        private string MessageFor(LevelResult result) =>
            result switch
            {
                LevelResult.Win => _winMessage,
                LevelResult.Overcharged => _overchargedMessage,
                _ => _outOfMassMessage
            };

        private void SetMassAnchor(float value) =>
            _massFill.anchorMax = new Vector2(value, _massFill.anchorMax.y);

        private void SetVisible(CanvasGroup group, bool isVisible, bool instant)
        {
            group.DOKill();
            group.interactable = isVisible;
            group.blocksRaycasts = isVisible;
            if (instant)
                group.alpha = isVisible ? 1f : 0f;
            else
                group.DOFade(isVisible ? 1f : 0f, _fadeDuration);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CacullationItem : MonoBehaviour
{
    [SerializeField] Text caculationText;
    [SerializeField] Text resultText;
    [SerializeField] ParticleSystem starFx;
    [SerializeField] Image correctImg;
    [SerializeField] Image wrongImg;
    [SerializeField] Image dauImg;
    [SerializeField] List<Sprite> sprites;

    int result;
    int id;
    private Vector3 endPos;
    private int tempResult = -1;
    private Tweener punchTween;
    private Vector3 startWrongImgPos;
    private Tween delayTween;

    public Text ResultText { get => resultText; }

    public void AssignItem(int _id, int _firstNumber, int _secondNumber, CaculateType caculateType)
    {
        id = _id;
        tempResult = -1;
        if(_secondNumber > _firstNumber)
        {
            var temp = _firstNumber;
            _firstNumber = _secondNumber;
            _secondNumber = temp;
        }
        dauImg.sprite = sprites[(int)caculateType];
        dauImg.SetNativeSize();
        dauImg.color = Color.black;
        switch (caculateType)
        {
            case CaculateType.Minus:
                caculationText.text = _firstNumber + "      " + _secondNumber + " = ";
                result = _firstNumber - _secondNumber;
                break;
            case CaculateType.Plus:
                caculationText.text = _firstNumber + "      " + _secondNumber + " = ";
                result = _firstNumber + _secondNumber;
                break;
            case CaculateType.Divide:
                caculationText.text = _firstNumber + "      " + _secondNumber + " = ";
                result = _firstNumber / _secondNumber;
                break;
            case CaculateType.Multiple:
                caculationText.text = _firstNumber + "      " + _secondNumber + " = ";
                result = _firstNumber * _secondNumber;
                break;
        }

        wrongImg.gameObject.SetActive(false);
        correctImg.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);

        startWrongImgPos = wrongImg.transform.position;
    }
    public void AssignItem(int _tempResult, System.Action OnComplete = null)
    {
        tempResult = _tempResult;
        OnComplete?.Invoke();
    }
    public void OnCheck(ResultItem resultItem)
    {
        if(tempResult == result)
        {
            resultItem.OnCompleted();
            OnCorrect();
        }
        else
        {
            OnInCorrect(() =>
            {
                if (resultItem == null) return;
                resultItem.OnFail();
            });
        }
    }
    void OnCorrect()
    {
        starFx?.Play();
        correctImg.transform.SetAsLastSibling();
        correctImg.gameObject.SetActive(true);
        wrongImg.gameObject.SetActive(false);
        if (delayTween != null) delayTween?.Kill();
        delayTween = DOVirtual.DelayedCall(1.5f, () =>
        {
            EventManager.OnCorrect?.Invoke();
        });
    }
    void OnInCorrect(System.Action OnComplete = null)
    {
        wrongImg.transform.SetAsLastSibling();
        wrongImg.gameObject.SetActive(true);
        correctImg.gameObject.SetActive(false);
        if(punchTween != null)
        {
            wrongImg.transform.position = startWrongImgPos;
            punchTween?.Kill();
        }
        punchTween = wrongImg.transform.DOPunchPosition(Vector3.right * 10f, 2, 3).OnComplete(() =>
        {
            OnComplete?.Invoke();
            wrongImg.gameObject.SetActive(false);
        });
    }
}

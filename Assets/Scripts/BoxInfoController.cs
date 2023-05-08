using System;
using TMPro;
using UnityEngine;

public class BoxInfoController : MonoBehaviour
{
    [SerializeField] private bool _hideOnAwake = true;

    [SerializeField] private TextMeshProUGUI _id;
    [SerializeField] private TextMeshProUGUI _subject;
    [SerializeField] private TextMeshProUGUI _grade;
    [SerializeField] private TextMeshProUGUI _mastery;
    [SerializeField] private TextMeshProUGUI _domainId;
    [SerializeField] private TextMeshProUGUI _domain;
    [SerializeField] private TextMeshProUGUI _cluster;
    [SerializeField] private TextMeshProUGUI _standardId;
    [SerializeField] private TextMeshProUGUI _standardDescription;

    private StackApiRequest.StackApiDataElement _data;
    private Action _closeButtonCallback;

    private void Awake()
    {
        if (_hideOnAwake)
            Hide();
    }

    public void Set(StackApiRequest.StackApiDataElement data, Action closeButtonCallback)
    {
        _data = data;
        _closeButtonCallback = closeButtonCallback;
        gameObject.SetActive(true);

        Set(_id, data.id.ToString());
        Set(_subject, data.subject);
        Set(_grade, data.grade);
        Set(_mastery, data.mastery.ToString());
        Set(_domainId, data.domainid);
        Set(_domain, data.domain);
        Set(_cluster, data.cluster);
        Set(_standardId, data.standardid);
        Set(_standardDescription, data.standarddescription);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Set(TextMeshProUGUI text, string content)
    {
        if (text)
            text.text = content;
    }

    public void CloseButtonClick()
    {
        _closeButtonCallback?.Invoke();
    }
}
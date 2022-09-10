using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;


[ExecuteInEditMode]
public class HomeUIPanel : MonoBehaviour
{
    public RealityList realityCollection;
    public RealityThumbnailContainer realityThumbnailContainer;

    [Header("Prefabs")]
    [SerializeField] GameObject _prefabTitle;

    [Header("UI Elements")]
    [SerializeField] Transform _verticleScrollContainer;
    [SerializeField] Transform _titleScrollContent;
    [SerializeField] Transform _horizentalScrollContainer;

    Canvas _canvas;

    List<GameObject> _realityThumbnailList = new List<GameObject>();

    int _realityCount;

    int _currentIndex = 0;

    public int CurrentIndex
    {
        get{ return _currentIndex; }
    }

    float _scrollValue;

    float _offset = 4f;

    private void Awake()
    {
        _realityCount = realityCollection.realities.Count;
        _canvas = FindObjectOfType<Canvas>();
        realityThumbnailContainer = FindObjectOfType<RealityThumbnailContainer>();

        DeletePreviousElement(_titleScrollContent);
        DeletePreviousElement(realityThumbnailContainer.transform.Find("Container").transform);

        InitialCoverContent();
    }


    private void Update()
    {
        UpdateScrollValue();
    }

    void InitialCoverContent()
    {
        for (int i = 0; i < _realityCount; i++)
        {
            // create thumbnailPrefabs
            var realityThumbnailGO = Instantiate(realityCollection.realities[i].thumbnailPrefab, realityThumbnailContainer.transform.Find("Container").transform);
            realityThumbnailGO.transform.localPosition = new Vector3(i* _offset, 0, 0);
            realityThumbnailGO.transform.localScale = Vector3.one * 0.28f;
            realityThumbnailGO.tag = "Reality Thumbnail";
            _realityThumbnailList.Add(realityThumbnailGO);

            // create titles
            _prefabTitle.transform.Find("Index").GetChild(0).GetComponent<TMPro.TMP_Text>().text = "Reality " + "#00" + realityCollection.realities[i].realityId;
            _prefabTitle.transform.Find("Title").GetChild(0).GetComponent<TMPro.TMP_Text>().text = realityCollection.realities[i].displayName;
            var titleGO = Instantiate(_prefabTitle, _titleScrollContent);
        }
    }

    void UpdateScrollValue()
    {
        _scrollValue = _horizentalScrollContainer.Find("Scrollbar Horizontal").GetComponent<Scrollbar>().value;
        _scrollValue = Mathf.Clamp01(_scrollValue);
        _currentIndex = Mathf.RoundToInt(_scrollValue * (_realityCount - 1));

        realityThumbnailContainer.currentIndex = _currentIndex;

        // set value to thumbnails
        var thumbnailContainerPosition = _scrollValue * (_realityCount-1) * _offset;
        realityThumbnailContainer.currentPostion = thumbnailContainerPosition;
        // set valut to name container
        _verticleScrollContainer.GetComponent<ScrollRect>().verticalNormalizedPosition = 1 -  _scrollValue;
    }

    void DeletePreviousElement(Transform content)
    {
        var tempList = new List<Transform>();
        for (int i = 0; i < content.childCount; i++)
        {
            tempList.Add(content.GetChild(i));
        }
        foreach (var child in tempList)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(child.gameObject);
            }
            else
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void GoToRealityDetailPanel()
    {
        realityThumbnailContainer._arrowPath.gameObject.SetActive(false);
        transform.Find("HamburgerButton").gameObject.SetActive(false);

        for (int i = 0; i < _realityCount; i++)
        {
            if (i == _currentIndex)
            {
                _realityThumbnailList[i].SetActive(true);
            }
            else
            {
                _realityThumbnailList[i].SetActive(false);
            }
        }

        realityThumbnailContainer._offset = new Vector3(0, 1.7f , 0);
    }

    public void GoToHomePanelLayout()
    {
        realityThumbnailContainer._arrowPath.gameObject.SetActive(true);

        transform.Find("HamburgerButton").gameObject.SetActive(true);

        for (int i = 0; i < _realityCount; i++)
        {
                _realityThumbnailList[i].SetActive(true);
        }

        realityThumbnailContainer._offset = new Vector3(0, 0f, 0);
    }

    public void SetThumbnailState(bool state)
    {
        for (int i = 0; i < _realityCount; i++)
        {
            _realityThumbnailList[i].SetActive(state);
        }
    }

}



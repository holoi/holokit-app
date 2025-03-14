// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using TMPro;

namespace Holoi.Library.HoloKitAppLib.UI
{

    public enum HoloKitAppRealityVideoType
    {
        PreviewVideo = 0,
        TutorialVideo = 1
    }

    public class HoloKitAppUIComponent_RealityDetailPage_Video : MonoBehaviour
    {
        [SerializeField] private HoloKitAppRealityVideoType _videoType;

        [SerializeField] private int _videoIndex = 0;

        [SerializeField] private TMP_Text _text; 

        [SerializeField] private VideoPlayer _videoPlayer;

        [SerializeField] private RawImage _videoRawImage;

        [SerializeField] private GameObject _playButton;

        private RenderTexture _renderTexture;

        private const int VideoWidth = 1170;

        private const int VideoHeight = 2080;

        private void Start()
        {
            var currentReality = HoloKitApp.Instance.CurrentReality;
            VideoClip videoClip = null;
            if (_videoType == HoloKitAppRealityVideoType.PreviewVideo)
            {
                if (currentReality.PreviewVideos.Count <= _videoIndex)
                {
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    videoClip = currentReality.PreviewVideos[_videoIndex];

                    string previewVideoStr = "Preview Video";
                    switch (LocalizationSettings.SelectedLocale.Identifier.Code)
                    {
                        case "en":
                            break;
                        case "zh-Hans":
                            previewVideoStr = "预览视频";
                            break;
                    }
                    if (currentReality.PreviewVideos.Count == 1)
                    {
                        _text.text = previewVideoStr;
                    }
                    else
                    {
                        _text.text = $"{previewVideoStr} {_videoIndex + 1}";
                    }
                }
            }
            else if (_videoType == HoloKitAppRealityVideoType.TutorialVideo)
            {
                if (currentReality.TutorialVideos.Count <= _videoIndex)
                {
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    videoClip = currentReality.TutorialVideos[_videoIndex];

                    string tutorialVideoStr = "Tutorial Video";
                    switch (LocalizationSettings.SelectedLocale.Identifier.Code)
                    {
                        case "en":
                            break;
                        case "zh-Hans":
                            tutorialVideoStr = "教学视频";
                            break;
                    }
                    if (currentReality.TutorialVideos.Count == 1)
                    {
                        _text.text = tutorialVideoStr;
                    }
                    else
                    {
                        _text.text = $"{tutorialVideoStr} {_videoIndex + 1}";
                    }
                    _videoPlayer.frame = 0;
                    StartCoroutine(PauseOnFirstFrame());
                }
            }

            // We first setup the VideoPlayer
            _videoPlayer.clip = videoClip;
            _renderTexture = new RenderTexture(VideoWidth, VideoHeight, 16, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm);
            _renderTexture.Create();
            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            _videoPlayer.targetTexture = _renderTexture;
            _videoPlayer.isLooping = _videoType == HoloKitAppRealityVideoType.PreviewVideo;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            for (ushort i = 0; i < _videoPlayer.audioTrackCount; i++)
            {
                _videoPlayer.SetDirectAudioMute(i, true);
            }

            // Then we setup the RawImage
            _videoRawImage.texture = _renderTexture;
        }

        private IEnumerator PauseOnFirstFrame()
        {
            yield return null;
            _videoPlayer.Pause();
        }

        private void Update()
        {
            if (_videoType == HoloKitAppRealityVideoType.TutorialVideo)
            {
                if (_videoPlayer.isPlaying)
                {
                    _playButton.SetActive(false);
                }
                else
                {
                    _playButton.SetActive(true);
                }
            }
        }

        private void OnDestroy()
        {
            // We release the allocated render texture on destroy
            if (_renderTexture != null)
            {
                _renderTexture.Release();
            }
        }

        public void OnToggleVideoPlay()
        {
            if (_videoType == HoloKitAppRealityVideoType.TutorialVideo)
            {
                if (_videoPlayer.isPlaying)
                {
                    _videoPlayer.Pause();
                }
                else
                {
                    _videoPlayer.Play();
                }
            }
        }
    }
}

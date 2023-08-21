using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SCN.Tutorial;

public class SettingPanel : Panel
{
    [SerializeField] Button backBtn;
    [SerializeField] Button soundBtn;
    [SerializeField] Button musicBtn;
    [SerializeField] List<Sprite> musicSprites;
    [SerializeField] List<Sprite> soundSprites;

    bool music = true;
    bool sfx = true;

    private void Start()
    {
        backBtn.onClick.AddListener(OnBack);
        soundBtn.onClick.AddListener(OnSoundClick);
        musicBtn.onClick.AddListener(OnMusicClick);
    }
    private void OnDestroy()
    {
        musicBtn.onClick.RemoveAllListeners();
        soundBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
    }
    void OnBack()
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        base.Hide();
    }
    void OnSoundClick()
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        sfx = !sfx;
        if (sfx)
        {
            soundBtn.image.sprite = soundSprites[0];
            SoundManager.instance.EnableSound(SoundType.SFx);
        } else
        {
            soundBtn.image.sprite = soundSprites[1];
            SoundManager.instance.MuteSound(SoundType.SFx);
        }
    }

    void OnMusicClick()
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        music = !music;
        if (music)
        {
            musicBtn.image.sprite = musicSprites[0];
            SoundManager.instance.EnableSound(SoundType.Music);
        }
        else
        {
            musicBtn.image.sprite = musicSprites[1];
            SoundManager.instance.MuteSound(SoundType.Music);
        }
    }
}

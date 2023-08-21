using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource sfx;
    [SerializeField] AudioSource speaker;
    [SerializeField] AudioSource music;
    [SerializeField] AudioSource wolfoo;
    [SerializeField] AudioClip homeMusic;
    [SerializeField] List<AudioClip> ingameMusics;
    [SerializeField] List<AudioClip> sfxWolfoo;
    [SerializeField] List<AudioClip> sfxLucy;
    [SerializeField] List<AudioClip> sfxOthers;
    [SerializeField] List<AudioClip> sfxNumbers;
    [SerializeField] List<AudioClip> sfxAlphas;
    [SerializeField] List<AudioClip> otherBacks;

    /// <summary>
    /// S? index trong Các list trên T??ng ?ng v?i index Sound Type
    /// </summary>
    public static SoundManager instance;

    List<int> exceptionIdx = new List<int>();
    private int rd;

    public AudioSource Sfx { get => sfx; }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        EventManager.OnCompletedHalfGame += GetCompletedHalfGame;

        //exceptionIdx.Add((int)SfxWolfooType.Hoow);
        //exceptionIdx.Add((int)SfxWolfooType.Hello);
        //exceptionIdx.Add((int)SfxWolfooType.Sad);

        music.clip = homeMusic;
    //    sfx.clip = sfxOthers[(int)SfxOtherType.Click];
        music.Play();
    }
    private void OnDestroy()
    {
        EventManager.OnCompletedHalfGame -= GetCompletedHalfGame;
    }

    private void GetCompletedHalfGame()
    {
        int rd = Random.Range(0, sfxWolfoo.Count);
        while (exceptionIdx.Contains(rd))
        {
            rd = Random.Range(0, sfxWolfoo.Count);
        }
        sfx.clip = sfxWolfoo[rd];
        sfx.Play();
    }

    public void PlayHome()
    {
        music.clip = homeMusic;
        music.Play();
    }
    public void PlayOtherBackMusic(OtherBackSoundType type)
    {
        music.clip = otherBacks[(int)type];
        music.Play();
    }
    public void PlayIngame()
    {
        music.clip = ingameMusics[Random.Range(0, ingameMusics.Count)];
        music.Play();
    }
    public void PlayWolfooSfx(SfxWolfooType type, CharacterAnimation.SexType sexType)
    {
        rd = Random.Range(0, 2);

        wolfoo.clip = sexType == CharacterAnimation.SexType.Boy ? sfxWolfoo[(int)type] : sfxLucy[(int)type];
        wolfoo.Play();
    }
    public void PlayWolfooSfx(SfxWolfooType type)
    {
        rd = Random.Range(0, 2);

        wolfoo.clip = rd == (int)CharacterAnimation.SexType.Boy ? sfxWolfoo[(int)type] : sfxLucy[(int)type];
        wolfoo.Play();
    }
    public void PlayOtherSfx(SfxOtherType type)
    {
        sfx.clip = sfxOthers[(int)type];
        sfx.Play();
    }
    public void PlayCountNumber(int idx)
    {
        if (sfxNumbers.Count == 0) return;

        speaker.clip = sfxNumbers[idx];
        speaker.Play();
    }
    public void SpeakAlpha(int idx)
    {
        speaker.clip = sfxAlphas[idx];
        speaker.Play();
    }
    public void MuteSound(SoundType type)
    {
        if (type == SoundType.SFx)
        {
            speaker.volume = 0;
            sfx.volume = 0;
            wolfoo.volume = 0;
        }
        else if (type == SoundType.Music)
        {
            music.volume = 0;
        }
    }
    public void EnableSound(SoundType type)
    {
        if (type == SoundType.SFx)
        {
            sfx.volume = 1;
            speaker.volume = 1;
            wolfoo.volume = 1;
        }
        else if (type == SoundType.Music)
        {
            music.volume = 1;
        }
    }
}
public enum SoundType
{
    Music,
    SFx,
}
public enum OtherBackSoundType
{
    Piano
}
public enum SfxWolfooType
{
    Hello,
    Hooray,
    Hoow,
    Sad,
    Wow,
    Thankyou,
    Laugh,
    Complain,
    Goodbye,
    Walk,
    Cool,
    Great,
    Disagree,
    Perfect,
}
public enum SfxOtherType
{
    Click,
    Draw,
    Correct,
    Incorrect,
    Lighting,
    Whistle,
    Collision,
    Congratulation,
    Popup,
    Cutting,
    BongBong,
    DownToGround,
    Magic,
    MagicRainbow,
    Printing,

}

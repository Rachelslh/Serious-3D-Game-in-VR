using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Pnl : MonoBehaviour
{
    public TextMeshProUGUI text;
    public AudioSource clip;
    public GameObject menu;
    public GameObject selector;
    public GameObject canvas;

    private string[] etapesChgm = { "Libérez votre « Pouvoir instantané » par la Programmation neurolinguistique",
                                "Prenez contact avec une sensation de capacité personnelle que vous connaissez bien (une chose que vous pouvez dire « ça je sais bien le faire »). ",
                                "Éprouvez la sensation bienfaisante que procure cette pensée (ce que vous savez bien faire). ",
                                "Posez votre index sur l’un de vos genoux ?",
                                "Choisissez le comportement ou l’habitude nouvelle que vous voulez acquérir.",
                                "Posez-vous la question tout en vous observant :« de quoi aurai-je l’air en l’adoptant (la nouvelle habitude) ? ",
                                "Regardez-vous le faire ! ",
                                "Projetez-vous mentalement dans cette vision et posez à nouveau votre index sur votre genou.",
                                "Votre réussite, votre bonheur, mais aussi votre degré destress dépendent de la façon dont vous communiquezavec vous - même, avec les autres et avec le monde." };
    private string[] etapesAncrage = { "Un ancrage rappelle l’idée d’ancrer. Ancrer en vous un souvenir ou une expérience positive. Par exemple, vous souhaitez ne plus perdre vos moyens en présence de votre patron. ",
                                "Vous choisissez un geste qui va servir d’ancrage (attention un geste par ancrage). Par exemple serrer le poing très fort.",
                                "Maintenant, vous allez vous rappeler en détail un moment ou vous étiez très a l’aise avec une autre personne. Ce peut être avec votre ami, ou en famille. L’important, c’est que vous « reviviez » ce moment « comme si » vous le viviez MAINTENANT. Mettez-y des détails (couleurs, sons, odeurs, ambiances et ÉMOTIONS etc.…)",
                                "Pendant ce temps, faîtes votre ancrage (serrer le poing par exemple)",
                                "Reproduisez cette expérience 3 fois de suite.",
                                "Chaque jour, pendant 5 minutes, vous recommencerez cet exercice jusqu’à réussite.",
                                "Visualisez maintenant la situation redoutée en imaginant que tout se passe bien pour vous ; mais en serrant le poing très fort. Reproduisez cela 3 fois.",
                                "Quand la situation redoutée se manifeste, vous allez refaire le geste (votre ancrage). Et, ceci tout le temps de l’entretien.",
                                "Vous constaterez que votre émotivité (sur le cas choisi) va diminuer ou disparaître." };
    private int i = 0;
    private List<AudioClip> clips = new List<AudioClip>();

    private void LoadChgmClips()
    {
        clips.Add(Resources.Load<AudioClip>("Audio Clips/ChgmRapide/ttsMP3.com_VoiceText_2019-5-17_1_30_29"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/ChgmRapide/ttsMP3.com_VoiceText_2019-5-17_1_17_22"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/ChgmRapide/ttsMP3.com_VoiceText_2019-5-17_1_18_0"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/ChgmRapide/ttsMP3.com_VoiceText_2019-5-17_1_18_25"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/ChgmRapide/ttsMP3.com_VoiceText_2019-5-17_1_20_3"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/ChgmRapide/ttsMP3.com_VoiceText_2019-5-17_1_20_27"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/ChgmRapide/ttsMP3.com_VoiceText_2019-5-17_1_20_59"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/ChgmRapide/ttsMP3.com_VoiceText_2019-5-17_1_21_20"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/ChgmRapide/ttsMP3.com_VoiceText_2019-5-17_1_22_27"));
    
    }

    private void LoadAncrageClips()
    {
        clips.Add(Resources.Load<AudioClip>("Audio Clips/Ancrage/ttsMP3.com_VoiceText_2019-5-22_2_47_28"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/Ancrage/ttsMP3.com_VoiceText_2019-5-22_2_47_44"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/Ancrage/ttsMP3.com_VoiceText_2019-5-22_2_48_37"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/Ancrage/ttsMP3.com_VoiceText_2019-5-22_2_48_58"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/Ancrage/ttsMP3.com_VoiceText_2019-5-22_2_49_11"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/Ancrage/ttsMP3.com_VoiceText_2019-5-22_2_49_44"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/Ancrage/ttsMP3.com_VoiceText_2019-5-22_2_49_57"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/Ancrage/ttsMP3.com_VoiceText_2019-5-22_2_50_54"));
        clips.Add(Resources.Load<AudioClip>("Audio Clips/Ancrage/ttsMP3.com_VoiceText_2019-5-22_2_51_9"));

    }

    IEnumerator LoadChgm()
    {
        LoadChgmClips();

        text.text = etapesChgm[i];
        int totalVisibleCharacters = text.text.Length;
        int counter = 0;
        bool playing = true;

        while (true)
        {
            int visibleCount = counter % (totalVisibleCharacters + 1);
            text.maxVisibleCharacters = visibleCount;

            if (playing)
            {
                playing = false;
                LoadClip(clips[i]);
            }

            if (visibleCount >= totalVisibleCharacters)
            {
                playing = true;
                i++;
                yield return new WaitForSeconds(10.0f);
                text.text = etapesChgm[i];
                totalVisibleCharacters = text.text.Length;
                counter = 0;
            }

            counter += 1;
            yield return new WaitForSeconds(0.04f);

            if (i == etapesChgm.Length - 1)
                text.enabled = false;
        }
    }

    IEnumerator LoadAncrage()
    {
        LoadAncrageClips();

        text.text = etapesAncrage[i];
        int totalVisibleCharacters = text.text.Length;
        int counter = 0;
        bool playing = true;

        while (true)
        {
            int visibleCount = counter % (totalVisibleCharacters + 1);
            text.maxVisibleCharacters = visibleCount;

            if (playing)
            {
                playing = false;
                LoadClip(clips[i]);
            }

            if (visibleCount >= totalVisibleCharacters)
            {
                playing = true;
                i++;
                yield return new WaitForSeconds(10.0f);
                text.text = etapesAncrage[i];
                totalVisibleCharacters = text.text.Length;
                counter = 0;
            }

            counter += 1;
            yield return new WaitForSeconds(0.04f);

            if (i == etapesAncrage.Length - 1)
                text.enabled = false;
        }
    }

    void LoadClip(AudioClip Clip)
    {
        clip.clip = Clip;
        clip.Play();
    }

    public void loadChgmRapide()
    {
        menu.active = false;
        selector.active = false;
        StartCoroutine(LoadChgm());
    }

    public void loadAncrage()
    {
        menu.active = false;
        selector.active = false;
        StartCoroutine(LoadAncrage());
    }

}

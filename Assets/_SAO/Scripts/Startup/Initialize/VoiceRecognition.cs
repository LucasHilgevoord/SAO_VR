using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows.Speech;

[Serializable]
public class Keyword
{
    public string keyword;
    public List<string> keywordDifferences;
    public UnityEvent recognizedCallbacks;
}

public class VoiceRecognition : MonoBehaviour
{
    [SerializeField] private Keyword[] keywords;
    private bool recognizedLink;
    private KeywordRecognizer m_Recognizer;

    public static event Action PhraseTriggered;

    internal bool Init()
    {
        if (PhraseRecognitionSystem.isSupported)
        {
            List<string> allKeywords = new List<string>();
            foreach (Keyword keyword in keywords)
            {
                foreach (string word in keyword.keywordDifferences)
                    allKeywords.Add(word);

                if (allKeywords.Contains(keyword.keyword) == false)
                    allKeywords.Add(keyword.keyword);
            }

            m_Recognizer = new KeywordRecognizer(allKeywords.ToArray());
            m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;
            m_Recognizer.Start();
            return true;
        } else
        {
            Debug.Log("Speech recognition is not supported on this machine");
            return false;
        }
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        foreach (Keyword keyword in keywords)
        {
            if (keyword.keywordDifferences.Contains(args.text) || keyword.keyword == args.text)
            {
                Debug.Log(args.text + " Recognized");
                keyword.recognizedCallbacks?.Invoke();
                break;
            }
        }
    }

    public void OnLinkRecognized() { recognizedLink = true; }

    public void OnStartRecognized()
    {
        if (recognizedLink == true)
            StartSequence();
    }

    public void OnLinkStartRecognized() { StartSequence(); }

    private void StartSequence()
    {
        Debug.Log("Linkuh STARTOOOH");

        m_Recognizer.Stop();
        recognizedLink = false;

        PhraseTriggered?.Invoke();
    }
}

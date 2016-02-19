using UnityEngine;
using System.Collections;

public class SpeechRecognizer : MonoBehaviour {

    private Queue sentenceQueue = new Queue();
    private Queue alertQueue = new Queue();
    public string displaySentence, displayAlert;
    public float horizontalValue = 0.0f;
    public float verticalValue = 0.0f;
    public bool crouch;
    public bool jump;

    private bool flag = true;

    void Awake()
    {
        Debug.Log("I'm awake.");
        SenseInputRecognition.Instance.OnRecData += OnSentenceDetected;
        SenseInputRecognition.Instance.OnAlertData += OnAlertDetected;
        SenseInputRecognition.Instance.OnShutdown += OnShutDown;

        if (SenseInputRecognition.Instance.mode == SenseInputRecognition.RecognitionType.CommandControl)
        {
            Debug.Log("Speak Now");
        }
    }

    void OnSentenceDetected(string _sentence)
    {
        lock (sentenceQueue) sentenceQueue.Enqueue(_sentence);
    }

    void OnAlertDetected(string _alert)
    {
        lock (alertQueue) alertQueue.Enqueue(_alert);
    }

    // Unsubscribe to SenseInput Events
    void OnShutDown()
    {
        SenseInputRecognition.Instance.OnRecData -= OnSentenceDetected;
        SenseInputRecognition.Instance.OnAlertData -= OnAlertDetected;
        SenseInputRecognition.Instance.OnShutdown -= OnShutDown;
    }

    void setFlag()
    {

    }

    void FixedUpdate()
    {
       


        lock (sentenceQueue)
            if (sentenceQueue.Count > 0)
                displaySentence = sentenceQueue.Dequeue().ToString();

        if ((string)displaySentence == "forward")
        {
            verticalValue = -1.0f;

        }
        else if ((string)displaySentence == "back")
        {
            verticalValue = 1.0f;
        }
        else if ((string)displaySentence == "left")
        {
            horizontalValue = 1.0f;
        }
        else if ((string)displaySentence == "right")
        {
            horizontalValue = -1.0f;
        }
        else if ((string)displaySentence == "jump")
        {
            jump = true;
        }
        else if ((string)displaySentence == "stay")
        {
            horizontalValue = 0.0f;
            verticalValue = 0.0f;
        }
        else if ((string)displaySentence == "duck")
        {
            crouch = true;
        }
        else if ((string)displaySentence == "up")
        {
            crouch = false;
        }
        else if ((string)displaySentence == "jump")
        {
            jump = true;
        }

    }

}

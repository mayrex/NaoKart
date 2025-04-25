using System.Diagnostics;
using UnityEngine;
using Whisper.Utils;
using TMPro;
using System.IO;
using System.Text;


using M2MqttUnity.Examples;
namespace Whisper.Samples
{
    /// <summary>
    /// Record audio clip from microphone and make a transcription.
    /// </summary>
    public class MicrophoneDemo : MonoBehaviour
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;
        public bool streamSegments = true;
        public bool printLanguage = true;
        public M2MqttUnityTest mqttClient;

        public TextMeshProUGUI outputText;

        private string _buffer;

        private void Awake()
        {
            
            whisper.OnNewSegment += OnNewSegment;
            whisper.OnProgress += OnProgressHandler;
            microphoneRecord.OnRecordStop += OnRecordStop;
        }

        private void Update()
        {
            // Rileva la pressione del pulsante "Triangle"
            if (Input.GetKeyDown(KeyCode.JoystickButton3))
            {
                OnButtonPressed();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnButtonPressed();
            }
        }

        private void OnButtonPressed()
        {
            if (!microphoneRecord.IsRecording)
            {
                microphoneRecord.StartRecord();
            }
            else
            {
                microphoneRecord.StopRecord();
            }
        }

        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            _buffer = "";

            var sw = new Stopwatch();
            sw.Start();

            var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
            if (res == null || !outputText)
                return;

            var text = res.Result;
          
            if (mqttClient != null)
            {
                mqttClient.AiMessage(text); 
            }

            if (printLanguage)
                text += $"\n\nLanguage: {res.Language}";

            outputText.text = text;

        }

        private void OnProgressHandler(int progress)
        {
            // Eliminato aggiornamento UI non necessario
        }

        private void OnNewSegment(WhisperSegment segment)
        {
            if (!streamSegments || !outputText)
                return;

            _buffer += segment.Text;
            outputText.text = _buffer + "...";
        }
    }
}

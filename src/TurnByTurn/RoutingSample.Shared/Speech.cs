﻿using System;
using System.Threading.Tasks;
#if XAMARIN
using Xamarin.Essentials;
#elif WINDOWS_UWP
using System.Linq;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
#elif WINDOWS_WPF
using System.Speech.Synthesis;
#endif


namespace RoutingSample
{
    public static class Speech
    {
#if XAMARIN
        private static SpeechOptions _speechOptions;
#elif WINDOWS_WPF || WINDOWS_UWP
        private static readonly SpeechSynthesizer _speechSynthesizer;
#endif

        static Speech()
        {
            // Create the synthesizer with a female voice
#if XAMARIN
            _speechOptions = new SpeechOptions
            {
                Volume = 0.95f,
                Pitch = 1.0f,
            };
#elif WINDOWS_UWP
            _speechSynthesizer = new SpeechSynthesizer();
            _speechSynthesizer.Voice = SpeechSynthesizer.AllVoices.Where(voice => voice.Gender == VoiceGender.Female)
                .FirstOrDefault() ?? SpeechSynthesizer.DefaultVoice;
#elif WINDOWS_WPF
            _speechSynthesizer = new SpeechSynthesizer();
            _speechSynthesizer.SelectVoiceByHints(VoiceGender.Female);
#endif
        }

        /// <summary>
        /// Asynchronously says the contents of a string.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        /// <returns></returns>
        public static async Task SayAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

#if XAMARIN
            await TextToSpeech.SpeakAsync(text, _speechOptions);
#elif WINDOWS_UWP
            // Doesn't seem to work all the time.
            using (var stream = await _speechSynthesizer.SynthesizeTextToStreamAsync(text))
            using (var source = MediaSource.CreateFromStream(stream, stream.ContentType))
            using (var mediaPlayer = new MediaPlayer())
            {
                mediaPlayer.Source = source;
                mediaPlayer.Volume = 0.7;
                mediaPlayer.IsLoopingEnabled = false;
                mediaPlayer.Play();
            }
#elif WINDOWS_WPF
            await Task.Run(() => _speechSynthesizer.Speak(text));
#endif
        }
    }
}

#if UNITY_ANDROID && !UNITY_EDITOR
using RichTap.Common;
using RichTap.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RichTap.Platforms.Mobile
{
    public sealed class AndroidRenderer : GenericAbstractRenderer
    {

#region Device info
        private const string DEVICE_NAME = "Android";
        private const string DESCRIPTION = "Android Device";
        private const string VERSION = "1.0";
#endregion

        public override string Description()
        {
            return DESCRIPTION;
        }

        public override string DeviceName()
        {
            return DEVICE_NAME;
        }
        public override string Version()
        {
            return VERSION;
        }

        public override bool Init()
        {
            Debug.Log("Android Initialize");
            AndroidSDKBridge.Initialize();
            return true;    
        }

        public override bool IsRichtapAvailable()
        {
            return AndroidSDKBridge.IsRichtapEffectSupported();
        }

        public override bool Quit()
        {
            AndroidSDKBridge.Quit();
            isHapticPlaying = false;
            return true;
        }

        public override void StartRendering(RichtapEffect effect)
        {
            if (effect is Source.RichtapClipEffect)
            {
                string content = (effect as Source.RichtapClipEffect).GetClip().GetContent();
                int amplitude = (effect as Source.RichtapClipEffect).GetAmplitude();
                int frequency = (effect as Source.RichtapClipEffect).GetFrequency();
                int loopCount = (effect as Source.RichtapClipEffect).GetLoopCount();
                int loopInterval = (effect as Source.RichtapClipEffect).GetLoopInterval();
                AndroidSDKBridge.Play(content, amplitude, frequency, loopCount, loopInterval);
                isHapticPlaying = true;
            }
            else if (effect is Source.RichtapPresetEffect)
            {
                int preset = (int)(effect as Source.RichtapPresetEffect).GetPreset();
                int amplitude = effect.GetAmplitude();
                AndroidSDKBridge.Play(preset, amplitude);
                isHapticPlaying = true;
            }
        }

        public override void StopRendering()
        {
            AndroidSDKBridge.Stop();
            isHapticPlaying = false;
        }

        public override void Update(UpdateInfo updateInfo)
        {
            AndroidSDKBridge.UpdateParams(updateInfo.amplitude, updateInfo.frequency, updateInfo.loopInterval);
        }
    }
}
#endif
using Game.Core;
using UnityEngine;

namespace Config
{
	[CreateAssetMenu(menuName = "Windows/Configs/Windows Config")]
	public class WindowsConfig : DefaultConfiguration
	{
		public WindowsData[] windowData;

		public WindowsData GetWindowsData(SupportClasses.WindowName _winType)
		{ 
			for (int i = 0; i < windowData.Length; i++)
				if (windowData[i].windowsType == _winType)
					return windowData[i];

			return null;
		}
	}
}
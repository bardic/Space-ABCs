using UnityEditor;
using System;
using System.Collections.Generic;

class BuildScript {
	[MenuItem("File/CommandLineBuild/iOS")]
	static void PerformiOSBuild()
	{
		PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
		iOSBuild();
	}
	
	[MenuItem("File/CommandLineBuild/DebugiOS")]
	static void PerformDebugiOSBUild()
	{
		PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
		iOSBuild();
	}
	
	[MenuItem("File/CommandLineBuild/Android")]
	static void PerformDebugAndroidBUild()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
		
		List<string> scenes = new List<string>();
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if (!scene.enabled) continue;
			scenes.Add(scene.path);
		}
		
		var opts = BuildOptions.AcceptExternalModificationsToPlayer;
     
		BuildPipeline.BuildPlayer(scenes.ToArray(), "./Builds/iOS", BuildTarget.Android, opts);
	}
	
	static void iOSBuild ()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
		
		List<string> scenes = new List<string>();
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if (!scene.enabled) continue;
			scenes.Add(scene.path);
		}
		
		var opts = BuildOptions.AcceptExternalModificationsToPlayer;
     
		BuildPipeline.BuildPlayer(scenes.ToArray(), "./Builds/iOS", BuildTarget.iOS, opts);
	}
}
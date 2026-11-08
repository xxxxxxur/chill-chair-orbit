using BepInEx;
using BepInEx.Logging;
using Bulbul;
using HarmonyLib;
using UnityEngine;

namespace ChillChairOrbit
{
    [BepInPlugin(Id, "Chill Chair Orbit", "1.0.1")]
    public sealed class ChairOrbitPlugin : BaseUnityPlugin
    {
        private const string Id = "local.chillwithyou.chairorbit";
        private Harmony harmony;
        private static ManualLogSource log;
        private static ChairOrbitCamera orbit;

        private void Awake()
        {
            log = Logger;
            harmony = new Harmony(Id);
            harmony.CreateClassProcessor(typeof(RoomPatch)).Patch();
        }

        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
            if (orbit)
                Destroy(orbit);
        }

        [HarmonyPatch(typeof(RoomGameManager), "Initialize")]
        private static class RoomPatch
        {
            private static readonly AccessTools.FieldRef<RoomCameraManager, Camera> RoomCamera =
                AccessTools.FieldRefAccess<RoomCameraManager, Camera>("_cam");

            private static void Postfix(RoomCameraManager ____roomCameraManager)
            {
                if (!____roomCameraManager || orbit)
                    return;

                var camera = RoomCamera(____roomCameraManager);
                if (!camera)
                    return;

                orbit = camera.gameObject.AddComponent<ChairOrbitCamera>();
                log.LogInfo("Middle-mouse orbit attached to " + camera.name);
            }
        }
    }
}

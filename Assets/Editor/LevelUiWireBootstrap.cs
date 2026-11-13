using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using ChickenShooter.Core;
using ChickenShooter.UI;

namespace ChickenShooter.EditorTools
{
    public static class LevelUiWireBootstrap
    {
        public static void Run()
        {
            for (int level = 1; level <= 3; level++)
            {
                var scene = EditorSceneManager.OpenScene("Assets/Scenes/Level" + level + ".unity", OpenSceneMode.Single);

                WireBossBar(level);
                WireHud(level);

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }

            Debug.Log("LevelUiWireBootstrap: done.");
        }

        static T Require<T>(T value, string what, int level) where T : UnityEngine.Object
        {
            if (value == null) throw new InvalidOperationException("LevelUiWireBootstrap: missing " + what + " in level " + level);
            return value;
        }

        static void WireBossBar(int level)
        {
            var canvas = Require(GameObject.Find("BossHealthBarCanvas"), "boss bar canvas", level);
            var root = Require(canvas.transform.Find("BossHealthBarRoot"), "boss bar root", level);
            var bar = Require(root.GetComponent<BossHealthBarUI>(), "boss bar script", level);

            var controllerGo = Require(GameObject.Find("RoundController"), "round controller", level);
            var controller = Require(controllerGo.GetComponent<RoundController>(), "round controller script", level);
            controller.bossHealthBar = bar;
            EditorUtility.SetDirty(controller);
        }

        static void WireHud(int level)
        {
            var canvas = Require(GameObject.Find("HudCanvas"), "hud canvas", level).transform;
            var hudGo = Require(GameObject.Find("HudController"), "hud controller", level);
            var hud = Require(hudGo.GetComponent<HudUI>(), "hud script", level);

            hud.roundText = Require(canvas.Find("RoundText"), "round text", level).GetComponent<Text>();
            hud.scoreText = Require(canvas.Find("ScoreText"), "score text", level).GetComponent<Text>();
            hud.livesText = Require(canvas.Find("LivesText"), "lives text", level).GetComponent<Text>();
            hud.missilesText = Require(canvas.Find("MissilesText"), "missiles text", level).GetComponent<Text>();
            var foodBar = Require(canvas.Find("FoodBar"), "food bar", level);
            hud.foodBarFill = Require(foodBar.Find("Fill"), "food bar fill", level).GetComponent<Image>();
            EditorUtility.SetDirty(hud);
        }
    }
}

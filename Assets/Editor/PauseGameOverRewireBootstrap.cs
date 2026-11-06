namespace ChickenShooter.EditorTools
{
    public static class PauseGameOverRewireBootstrap
    {
        public static void Rewire()
        {
            UIBootstrap.BuildPauseMenu("Assets/Scenes/Level2.unity");
            UIBootstrap.BuildGameOverScreen("Assets/Scenes/Level2.unity");
            UIBootstrap.BuildPauseMenu("Assets/Scenes/Level3.unity");
            UIBootstrap.BuildGameOverScreen("Assets/Scenes/Level3.unity");
        }
    }
}

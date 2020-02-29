using UnityEngine;

namespace Com.NUIGalway.CompGame
{
    public class ClipperGate
    {
        public const float PLAYER_RESPAWN_TIME = 4.0f;
        public const float PLAYER_MAX_HEALTH = 3f;

        public const string PLAYER_HEALTH = "PlayerHealth";
        public const string PLAYER_READY = "IsPlayerReady";
        public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
        public const string CHOSEN_CHARACTER = "SelectedCharacter";

        public static Color GetColor(int colorChoice)
        {
            switch (colorChoice)
            {
                case 0: return new Color(255f / 255f, 71f / 255f, 71f / 255f);
                case 1: return new Color(126f / 255f, 203f / 255f, 66f / 255f);
                case 2: return new Color(21f / 255f, 207f / 255f, 228f / 255f);
                case 3: return new Color(255f / 255f, 238f / 255f, 5f / 255f);
                case 4: return new Color(218f / 255f, 36f / 255f, 151f / 255f);
                case 5: return new Color(110f / 255f, 133f / 255f, 170f / 255f);
                case 6: return new Color(113f / 255f, 115f / 255f, 238f / 255f);
                case 7: return new Color(251f / 255f, 242f / 255f, 207f / 255f);
            }

            return Color.black;
        }

        public static Color GetColor2(int colorChoice)
        {
            switch (colorChoice)
            {
                case 0: return new Color(248f / 255f, 173f / 255f, 98f / 255f);
                case 1: return new Color(97f / 255f, 233f / 255f, 190f / 255f);
                case 2: return new Color(217f / 255f, 112f / 255f, 170f / 255f);
                case 3: return new Color(242f / 255f, 238f / 255f, 125f / 255f);
                case 4: return new Color(251f / 255f, 161f / 255f, 228f / 255f);
                case 5: return new Color(78f / 255f, 125f / 255f, 112f / 255f);
                case 6: return new Color(168f / 255f, 119f / 255f, 232f / 255f);
                case 7: return new Color(207f / 255f, 251f / 255f, 247f / 255f);
            }

            return Color.black;
        }

        public static string GetCharacter(int charChoice)
        {
            switch (charChoice)
            {
                case 0: return "SM_Chr_Boss_Female_01";
                case 1: return "SM_Chr_Business_Female_04";
                case 2: return "SM_Chr_Developer_Female_02";
                case 3: return "SM_Chr_Business_Male_02";
                case 4: return "SM_Chr_Developer_Male_01";
                case 5: return "SM_Chr_Security_Male_01";
            }

            return "SM_Chr_Boss_Female_01";
        }
    }
}

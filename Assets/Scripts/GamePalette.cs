using UnityEngine;

namespace TurnTheOrientation
{
    public static class GamePalette
    {
        public static readonly Color Ink = Hex("#08121F");
        public static readonly Color Backdrop = Hex("#0E2132");
        public static readonly Color Floor = Hex("#17394B");
        public static readonly Color FloorLine = Hex("#24546A");
        public static readonly Color Wall = Hex("#315E73");
        public static readonly Color WallEdge = Hex("#78B7C7");
        public static readonly Color Player = Hex("#FFD166");
        public static readonly Color PlayerSoft = Hex("#FFF0B8");
        public static readonly Color Crate = Hex("#62D6D3");
        public static readonly Color CrateDark = Hex("#267F8A");
        public static readonly Color Plate = Hex("#FF8C61");
        public static readonly Color Accent = Hex("#FF5D73");
        public static readonly Color Violet = Hex("#9B7EDE");
        public static readonly Color Text = Hex("#F4FAFC");
        public static readonly Color MutedText = Hex("#9EC2CE");
        public static readonly Color Success = Hex("#8BE28B");
        public static readonly Color Shadow = Hex("#050A12");

        public static Color Alpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString(value, out Color result);
            return result;
        }
    }
}

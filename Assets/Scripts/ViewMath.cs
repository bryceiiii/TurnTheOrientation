using UnityEngine;

namespace TurnTheOrientation
{
    public enum PuzzleViewMode
    {
        TopDown,
        SideView
    }

    public static class ViewMath
    {
        public static int NormalizeAngle(int angle)
        {
            int normalized = angle % 360;
            return normalized < 0 ? normalized + 360 : normalized;
        }

        public static PuzzleViewMode ModeForAngle(int angle)
        {
            int normalized = NormalizeAngle(angle);
            return normalized % 180 == 0 ? PuzzleViewMode.TopDown : PuzzleViewMode.SideView;
        }

        public static bool IsSideView(int angle)
        {
            return ModeForAngle(angle) == PuzzleViewMode.SideView;
        }

        public static Vector2 ScreenRight(int angle)
        {
            return Quaternion.Euler(0f, 0f, NormalizeAngle(angle)) * Vector2.right;
        }

        public static Vector2 ScreenUp(int angle)
        {
            return Quaternion.Euler(0f, 0f, NormalizeAngle(angle)) * Vector2.up;
        }

        public static Vector2 GravityDown(int angle)
        {
            return -ScreenUp(angle);
        }
    }
}

using Microsoft.Xna.Framework;
using System;

namespace LostHope.Engine.Utils
{
    // This class implements a few general helper functions that might be used by different scripts.
    public static class Utils
    {
        // A lerping function that uses the current position, and lerps it to an end position, using 'maxDistanceDelta'
        // as a step
        public static Vector2 MoveTowards(this Vector2 current, Vector2 end, float maxDistanceDelta, out bool done)
        {
            float diffX = end.X - current.X;
            float diffY = end.Y - current.Y;

            float sqDist = (diffX * diffX) + (diffY * diffY);

            if (sqDist == 0 || (maxDistanceDelta >= 0 && sqDist <= maxDistanceDelta * maxDistanceDelta))
            {
                done = true;
                return end;
            }

            float dist = MathF.Sqrt(sqDist);

            done = false;
            return new Vector2(current.X + (diffX / dist * maxDistanceDelta), current.Y + (diffY / dist * maxDistanceDelta));
        }
    }
}

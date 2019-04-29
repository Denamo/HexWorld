using logic.debug;
using logic.math;
using System.Collections.Generic;

namespace logic.util
{
    
    static class LogicArray2Utils
    {

        public enum BlitMethod
        {
            Add,
            Multiply,
            Replace
        }

        private static List<BlitMethod> sm_blitMethods = new List<BlitMethod>();

        public static void Blit( LogicIntArray2 s, LogicIntArray2 d, int dX, int dY )
        {

            Debugger.doAssert(d!=null,"blit Destination null");
	        Debugger.doAssert(s!=null,"blit Source null");

	        Blit(s, 0, 0, s.width, s.height, d, dX, dY );
        }

        public static void Blit( LogicIntArray2 s, int sX, int sY, int width, int height, LogicIntArray2 d, int dX, int dY)
        {
            Debugger.doAssert(d != null, "blit Destination null");
            Debugger.doAssert(s != null, "blit Source null");

            switch (GetBlitMethod())
            {

                case BlitMethod.Add:
                    {
                        for (int y = 0; y < height; ++y)
                        {
                            for (int x = 0; x < width; ++x)
                            {
                                int value = s.Get(sX + x, sY + y) + d.Get(dX + x, dY + y);
                                d.Set(dX + x, dY + y, value);
                            }
                        }
                    }
                    break;

                case BlitMethod.Multiply:
                    {
                        for (int y = 0; y < height; ++y)
                        {
                            for (int x = 0; x < width; ++x)
                            {
                                int value = s.Get(sX + x, sY + y) * d.Get(dX + x, dY + y);
                                d.Set(dX + x, dY + y, value);
                            }
                        }
                    }
                    break;

                case BlitMethod.Replace:
                default:
                    {
                        for (int y = 0; y < height; ++y)
                        {
                            for (int x = 0; x < width; ++x)
                            {
                                int value = s.Get(sX + x, sY + y);
                                d.Set(dX + x, dY + y, value);
                            }
                        }
                    }
                    break;
            }

        }

        public static string ToString(LogicIntArray2 a)
        {
            string str = "";
            for (int y = 0; y < a.height; ++y)
            {
                for (int x = 0; x < a.width; ++x)
                {
                    str += string.Format("{0},", a.Get(x, y));
                }
                str += "\n";
            }
            return str;
        }


        public static void DebugPrint( LogicIntArray2 a)
        {
            for (int y = 0; y < a.height; ++y)
            {
                string str = "";
                for (int x = 0; x < a.width; ++x)
                {
                    str += string.Format("{0},", a.Get(x, y));
                }
                Debugger.print(str);
            }
        }
        
        public static BlitMethod GetBlitMethod()
        {
            if (sm_blitMethods.Count==0)
                return BlitMethod.Replace;
            else
                return sm_blitMethods[sm_blitMethods.Count - 1];
        }

        public static void PushBlitMethod(BlitMethod method)
        {
            sm_blitMethods.Add(method);
        }

        public static void PopBlitMethod()
        {
            sm_blitMethods.RemoveAt(sm_blitMethods.Count - 1);
        }

        public static bool FindClosest( LogicIntArray2 needle, LogicIntArray2 haystack, LogicPoint2 start, LogicPoint2 closest)
        {
            Debugger.doAssert(needle != null, "findClosest needle null");
            Debugger.doAssert(haystack != null, "blit haystack null");
            Debugger.doAssert(start != null, "findClosest start null");
            Debugger.doAssert(closest != null, "blit closest null");

            int dWidth = haystack.width - needle.width;
            int dHeight = haystack.height - needle.height;

            int distMin = int.MaxValue;
            for (int y = 0; y <= dHeight; ++y)
            {
                for (int x = 0; x <= dWidth; ++x)
                {
                    int dx = x - start.x;
                    int dy = y - start.y;
                    int dist = dx * dx + dy * dy;

                    if (dist < distMin)
                    {

                        bool equal = true;
                        for (int h = 0; h < needle.height && equal; ++h)
                        {
                            for (int w = 0; w < needle.width && equal; ++w)
                            {
                                equal = haystack.Get(x + w, y + h) == needle.Get(w, h);
                            }
                        }

                        if (equal)
                        {
                            distMin = dist;
                            closest.x = x;
                            closest.y = y;
                        }

                    }

                }
            }
            return distMin < int.MaxValue;
        }

        public static bool FindClosestRect(int needleWidth, int needleHeight, int needleValue, LogicIntArray2 haystack, LogicPoint2 start, LogicPoint2 closest)
        {
            Debugger.doAssert(haystack != null, "blit haystack null");
            Debugger.doAssert(start != null, "findClosest start null");
            Debugger.doAssert(closest != null, "blit closest null");

            int dWidth = haystack.width - needleWidth;
            int dHeight = haystack.height - needleHeight;

            int distMin = int.MaxValue;
            for (int y = 0; y <= dHeight; ++y)
            {
                for (int x = 0; x <= dWidth; ++x)
                {
                    int dx = x - start.x;
                    int dy = y - start.y;
                    int dist = dx * dx + dy * dy;

                    if (dist < distMin)
                    {

                        bool equal = true;
                        for (int h = 0; h < needleHeight && equal; ++h)
                        {
                            for (int w = 0; w < needleWidth && equal; ++w)
                            {
                                equal = haystack.Get(x + w, y + h) == needleValue;
                            }
                        }

                        if (equal)
                        {
                            distMin = dist;
                            closest.x = x;
                            closest.y = y;
                        }

                    }

                }
            }

            return distMin < int.MaxValue;
        }
    
        public static void FillRect(int width, int height, int value, LogicIntArray2 d, int dX, int dY)
        {
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    d.Set(dX + x, dY + y, value);
                }
            }
        }


    }




}

using System;

namespace logic.math
{
    public class LogicMath
    {
        public const int INT_MAX_VALUE = 2147483647;
        public const int INT_MIN_VALUE = (-2147483647 - 1);
        
        // signed int ops that can overflow (undefined behaviuour in C)
        public static int overflowAdd(int a, int b)
        {
            return a + b;
        }

        public static int overflowShl(int a, int b)
        {
            return a << (int)b;
        }

        public static int overflowShrUnsigned(int a, int b)
        {
            return a >> (int)b;
        }

        public static int overflowMul(int a, int b)
        {
            return a * b;
        }

        public static long overflowAdd(long a, long b)
        {
            return a + b;
        }

        public static long overflowShl(long a, long b)
        {
            return a << (int)b;
        }

        public static long overflowShrUnsigned(long a, long b)
        {
            return a >> (int)b;
        }

        public static long overflowMul(long a, long b)
        {
            return a * b;
        }
        
        //
        //  LogicMath.cpp
        //  GUT
        //
        //  Created by Markus Aalto on 26.10.2011.
        //  Copyright (c) 2011 Supercell. All rights reserved.
        //
        
        private readonly static int[] DAYS_IN_MONTH = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        private readonly static byte[] BITS_IN_BYTE = {
            0
            ,1,1,2,1,2,2,3,1,2,2,3,2,3,3,4,1
            ,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,1
            ,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,2
            ,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,1
            ,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,2
            ,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,2
            ,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,3
            ,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,1
            ,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,2
            ,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,2
            ,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,3
            ,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,2
            ,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,3
            ,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,3
            ,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,4
            ,5,5,6,5,6,6,7,5,6,6,7,6,7,7,8
        };

        public static int getDaysInMonth(int monthIndex, bool leapYear)
        {
            if (leapYear && monthIndex == 1)
            {
                return 29;
            }

            return DAYS_IN_MONTH[monthIndex];
        }

        public static int max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        public static int min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        public static int clamp(int value, int low, int high)
        {
            if (value <= low)
            {
                return low;
            }
            if (value >= high)
            {
                return high;
            }
            return value;
        }
        
        public static int getBitsInInteger(int value)
        {
            int count = BITS_IN_BYTE[(value >> (int)24) & 0xFF];
            count += BITS_IN_BYTE[(value >> (int)16) & 0xFF];
            count += BITS_IN_BYTE[(value >> (int)8) & 0xFF];
            count += BITS_IN_BYTE[value & 0xFF];

            return count;
        }
        
        public static int abs(int a)
        {
            if (a > 0)
                return a;
            else
                return -a;
        }
        
        public static int sign(int a)
        {
            if (a > 0)
                return 1;
            else if (a < 0)
                return -1;
            else
                return 0;
        }

        public static long max(long a, long b)
        {
            return (a > b) ? a : b;
        }

        public static long min(long a, long b)
        {
            return (a < b) ? a : b;
        }

        public static long clamp(long value, long low, long high)
        {
            if (value <= low)
            {
                return low;
            }
            if (value >= high)
            {
                return high;
            }
            return value;
        }


    }
}
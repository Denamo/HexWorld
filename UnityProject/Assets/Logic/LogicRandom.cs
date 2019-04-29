namespace logic.math
{
    public class LogicRandom
    {
        private int m_iteratedSeed;

        private void initializeMembers()
        {
            m_iteratedSeed = 0;
        }


        public LogicRandom()
        {
            initializeMembers();
        }

        public LogicRandom(int seed)
        {
            initializeMembers();
            setIteratedRandomSeed(seed);
        }

        public void destruct()
        {
            initializeMembers();
        }

        public void setIteratedRandomSeed(int seed)
        {
            m_iteratedSeed = seed;
        }

        public int getIteratedRandomSeed()
        {
            return m_iteratedSeed;
        }

        private int iterateRandomSeed(int seed)
        {
            if (seed == 0) seed = -1;

            seed ^= LogicMath.overflowShl(seed, 13);
            seed ^= (seed >> (int)17);

            return seed ^ LogicMath.overflowShl(seed, 5);
        }

        public int rand(int maxValue)
        {
            if (maxValue > 0)
            {
                m_iteratedSeed = iterateRandomSeed(m_iteratedSeed);

                if (m_iteratedSeed < 0)
                {
                    return (m_iteratedSeed * -1) % maxValue;
                }

                return m_iteratedSeed % maxValue;
            }

            return 0;
        }
        
    }
}
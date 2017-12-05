using System;

namespace buffalo
{
    public class ContentManager
    {
        private static ContentManager _instance;
        public Random random;

        private ContentManager()
        {
            random = new Random();
        }

        public static ContentManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ContentManager();
                return _instance;
            }
        }
    }
}

namespace AccordsBot
{
    class State
    {
        /// <summary>
        /// Состояние
        /// </summary>
        public enum Categories : int
        {
            /// <summary>
            /// Дефолтное
            /// </summary>
            Default = 0,
            /// <summary>
            /// Начальное 
            /// </summary>
            Start = 1,
            /// <summary>
            /// Поиск
            /// </summary>
            Search = 2,
            /// <summary>
            /// Плейлист
            /// </summary>
            Playlist = 3,
            /// <summary>
            /// Настройки
            /// </summary>
            Settings = 4,
            
            Popular = 5
      

        }

        /// <summary>
        /// Стадии
        /// </summary>
        public enum Stage : int
        {
            Default = 0,
            Alpha = 1,
            Omega = 2,
            SearchLimit = 3,
            SendMessage = 4
        }
    }
}

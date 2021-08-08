﻿namespace asagiv.datapush.common.Interfaces
{
    public interface IClientConnectionSettings
    {
        public uint Id { get; }
        public string ConnectionName { get; }
        public string ConnectionString { get; }
        public bool IsPullNode { get; }
    }
}

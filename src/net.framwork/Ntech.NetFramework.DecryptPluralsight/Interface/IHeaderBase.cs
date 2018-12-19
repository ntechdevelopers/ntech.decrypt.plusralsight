namespace Ntech.NetFramework.DecryptPluralsight.Interface
{
    using System;

    public interface IHeaderBase
    {
        event EventHandler<string> SearchTextChangedEvent;
    }
}
